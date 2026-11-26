<#
.SYNOPSIS
    Gestor integral de servicios para APE 04 (NEXORA Autos).
    Permite levantar, detener, reiniciar y supervisar todos los servicios o uno individual.

.EXAMPLE
    .\servicios.ps1 status
    .\servicios.ps1 start
    .\servicios.ps1 start todo
    .\servicios.ps1 stop api-8080
    .\servicios.ps1 start api-8080
    .\servicios.ps1 restart notificaciones
    .\servicios.ps1 stop all
    .\servicios.ps1 menu
#>

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('start', 'stop', 'restart', 'status', 'menu', 'help')]
    [string]$Accion = 'menu',

    [Parameter(Position = 1)]
    [string]$Objetivo = 'all',

    [switch]$SinWT,
    [switch]$SyncNginx,
    [switch]$ConEscritorio
)

$ErrorActionPreference = 'Stop'
$RepoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

# ==================== DETECCION DE HERRAMIENTAS ====================

function Get-NginxExe {
    if ($env:NGINX_HOME) {
        $cand = Join-Path $env:NGINX_HOME 'nginx.exe'
        if (Test-Path $cand) { return $cand }
    }
    $cmd = Get-Command nginx.exe -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }
    foreach ($p in @('C:\nginx\nginx.exe', 'C:\tools\nginx\nginx.exe', 'C:\xampp\nginx\nginx.exe')) {
        if (Test-Path $p) { return $p }
    }
    return $null
}

function Get-WtExe {
    if ($SinWT) { return $null }
    $cmd = Get-Command wt.exe -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }
    return $null
}

# ==================== CARGA SEGURA DE SECRETOS LOCALES ====================

function Resolve-SpringPlaceholder {
    param([string]$Valor)
    $limpio = $Valor.Trim()
    if ($limpio -match '^\$\{([^:}]+)(?::(.*))?\}$') {
        $varName = $Matches[1].Trim()
        $defVal = if ($Matches[2]) { $Matches[2].Trim() } else { '' }
        $envVal = [Environment]::GetEnvironmentVariable($varName)
        if (-not [string]::IsNullOrWhiteSpace($envVal)) { return $envVal.Trim() }
        return $defVal
    }
    return $limpio
}

function Get-OAuthConfig {
    $cfg = @{
        GatewayUrl   = 'http://127.0.0.1:8090'
        ClientId     = ''
        ClientSecret = ''
    }

    # 1. Variables de entorno actuales
    if ($env:APE04_GATEWAY_URL) { $cfg.GatewayUrl = $env:APE04_GATEWAY_URL.Trim() }
    elseif ($env:API_BASE_URL) { $cfg.GatewayUrl = $env:API_BASE_URL.Trim() }

    if ($env:OAUTH_CLIENT_ID) { $cfg.ClientId = $env:OAUTH_CLIENT_ID.Trim() }
    elseif ($env:APE04_OAUTH_CLIENT_ID) { $cfg.ClientId = $env:APE04_OAUTH_CLIENT_ID.Trim() }

    if ($env:OAUTH_CLIENT_SECRET) { $cfg.ClientSecret = $env:OAUTH_CLIENT_SECRET.Trim() }
    elseif ($env:APE04_OAUTH_CLIENT_SECRET) { $cfg.ClientSecret = $env:APE04_OAUTH_CLIENT_SECRET.Trim() }

    # 2. Archivo .env / .env.local en raiz del repo (ignorado por Git)
    foreach ($envFile in @((Join-Path $RepoRoot '.env.local'), (Join-Path $RepoRoot '.env'))) {
        if (Test-Path $envFile) {
            foreach ($line in Get-Content $envFile) {
                $t = $line.Trim()
                if (-not $t -or $t.StartsWith('#')) { continue }
                $idx = $t.IndexOf('=')
                if ($idx -le 0) { continue }
                $k = $t.Substring(0, $idx).Trim()
                $v = $t.Substring($idx + 1).Trim().Trim('"', "'")
                if ($k -in @('APE04_GATEWAY_URL', 'API_BASE_URL') -and $v) { $cfg.GatewayUrl = $v }
                if ($k -in @('OAUTH_CLIENT_ID', 'APE04_OAUTH_CLIENT_ID') -and -not $cfg.ClientId -and $v) { $cfg.ClientId = $v }
                if ($k -in @('OAUTH_CLIENT_SECRET', 'APE04_OAUTH_CLIENT_SECRET') -and -not $cfg.ClientSecret -and $v) { $cfg.ClientSecret = $v }
            }
        }
    }

    # 3. Respaldo desde application.properties local de Spring Boot (ignorado por Git)
    $propsPath = Join-Path $RepoRoot 'venta-vehiculos-api_security\src\main\resources\application.properties'
    if ((-not $cfg.ClientSecret -or -not $cfg.ClientId) -and (Test-Path $propsPath)) {
        foreach ($line in Get-Content $propsPath) {
            $t = $line.Trim()
            if (-not $t -or $t.StartsWith('#')) { continue }
            $idx = $t.IndexOf('=')
            if ($idx -le 0) { continue }
            $k = $t.Substring(0, $idx).Trim()
            $v = $t.Substring($idx + 1).Trim()
            if ($k -eq 'app.security.client-id' -and -not $cfg.ClientId) {
                $cfg.ClientId = Resolve-SpringPlaceholder $v
            }
            if ($k -eq 'app.security.client-secret' -and -not $cfg.ClientSecret) {
                $res = Resolve-SpringPlaceholder $v
                if ($res -and -not $res.StartsWith('CAMBIAR_')) {
                    $cfg.ClientSecret = $res
                }
            }
        }
    }

    if (-not $cfg.ClientId) { $cfg.ClientId = 'vehiculos-client' }
    return $cfg
}

# ==================== UTILIDADES DE PROCESOS Y PUERTOS ====================

function Get-ListeningPid {
    param([int]$Port)
    $conn = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($conn) { return [int]$conn.OwningProcess }
    return $null
}

function Test-PortListening {
    param([int]$Port)
    return ($null -ne (Get-ListeningPid -Port $Port))
}

function Stop-ProcessTreeSmart {
    param([int]$TargetPid)
    if ($TargetPid -le 0) { return }

    $killRoot = $TargetPid
    try {
        $proc = Get-CimInstance Win32_Process -Filter "ProcessId=$TargetPid" -ErrorAction SilentlyContinue
        if ($proc -and $proc.ParentProcessId -gt 0) {
            $parent = Get-CimInstance Win32_Process -Filter "ProcessId=$($proc.ParentProcessId)" -ErrorAction SilentlyContinue
            if ($parent) {
                $pName = $parent.Name.ToLowerInvariant()
                $pCmd  = if ($parent.CommandLine) { $parent.CommandLine.ToLowerInvariant() } else { '' }
                # Si el padre es el reloader de uvicorn, wrapper maven o npm, detenemos desde el padre para que no se reinicie
                $esReloader = ($pName -in @('python.exe', 'cmd.exe', 'node.exe')) -and (
                    $pCmd -match 'uvicorn|mvnw|mvn|spring-boot|npm|vite|server\.mjs'
                )
                if ($esReloader) {
                    $killRoot = [int]$parent.ProcessId
                }
            }
        }
    } catch {
        # Fallback al PID detectado
    }

    & taskkill /PID $killRoot /T /F 2>$null | Out-Null
    if ($killRoot -ne $TargetPid) {
        & taskkill /PID $TargetPid /T /F 2>$null | Out-Null
    }
}

function Launch-ServiceTerminal {
    param(
        [string]$Title,
        [string]$WorkingDir,
        [string]$CommandScript
    )

    $fullScript = @"
`$Host.UI.RawUI.WindowTitle = '$Title'
Set-Location -LiteralPath '$WorkingDir'
Write-Host '==================================================' -ForegroundColor DarkGray
Write-Host '  NEXORA Autos - $Title' -ForegroundColor Yellow
Write-Host '  Dir: $WorkingDir' -ForegroundColor DarkGray
Write-Host '  Presiona Ctrl + C para detener este servicio.' -ForegroundColor Cyan
Write-Host '==================================================' -ForegroundColor DarkGray
$CommandScript
"@
    $bytes = [System.Text.Encoding]::Unicode.GetBytes($fullScript)
    $encoded = [Convert]::ToBase64String($bytes)

    $wt = Get-WtExe
    if ($wt) {
        Start-Process -FilePath $wt -ArgumentList @(
            '-w', 'ape04',
            'new-tab',
            '--title', $Title,
            '-d', $WorkingDir,
            'powershell.exe', '-NoLogo', '-NoExit', '-ExecutionPolicy', 'Bypass', '-EncodedCommand', $encoded
        ) | Out-Null
    } else {
        Start-Process -FilePath 'powershell.exe' -WorkingDirectory $WorkingDir -ArgumentList @(
            '-NoLogo', '-NoExit', '-ExecutionPolicy', 'Bypass', '-EncodedCommand', $encoded
        ) | Out-Null
    }
}

# ==================== DEFINICION DE SERVICIOS ====================

$ServiceNames = @(
    'rabbitmq',
    'api-8080',
    'api-8082',
    'nginx',
    'ventas',
    'worker',
    'notificaciones',
    'monitor',
    'pwa',
    'react',
    'winforms',
    'javafx'
)

$Groups = @{
    'all'      = @('rabbitmq', 'api-8080', 'api-8082', 'nginx', 'ventas', 'notificaciones', 'monitor', 'pwa', 'react')
    'todo'     = @('rabbitmq', 'api-8080', 'api-8082', 'nginx', 'ventas', 'notificaciones', 'monitor', 'pwa', 'react', 'winforms', 'javafx')
    'backend'  = @('rabbitmq', 'api-8080', 'api-8082', 'nginx', 'ventas', 'notificaciones', 'monitor')
    'clientes' = @('pwa', 'react', 'winforms', 'javafx')
    'web'      = @('pwa', 'react')
    'desktop'  = @('winforms', 'javafx')
}

function Resolve-Targets {
    param([string]$Name)
    $n = $Name.Trim().ToLowerInvariant()
    if ($n -eq 'all' -and $ConEscritorio) { return $Groups['todo'] }
    if ($Groups.ContainsKey($n)) { return $Groups[$n] }
    if ($ServiceNames -contains $n) { return @($n) }
    throw "Servicio o grupo desconocido: '$Name'. Validos: $($ServiceNames -join ', '), all, todo, backend, clientes, web, desktop."
}

# ==================== ARRANCAR UN SERVICIO ====================

function Start-OneService {
    param([string]$Name, [hashtable]$OAuth)

    switch ($Name) {
        'rabbitmq' {
            if (Test-PortListening 5672) {
                Write-Host "  [OK] rabbitmq ya esta escuchando en :5672" -ForegroundColor Green
                return
            }
            $rabbitDir = Join-Path $RepoRoot 'rabbitmq'
            Write-Host "  [..] Iniciando rabbitmq (Docker Compose)..." -ForegroundColor Cyan
            Push-Location $rabbitDir
            $prevEap = $ErrorActionPreference
            try {
                $ErrorActionPreference = 'Continue'
                & docker compose up -d 2>&1 | Out-Null
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "  [OK] rabbitmq iniciado (:5672 / UI :15672)" -ForegroundColor Green
                } else {
                    Write-Host "  [AVISO] No se pudo iniciar rabbitmq. Verifica que Docker Desktop este abierto." -ForegroundColor Yellow
                }
            } finally {
                $ErrorActionPreference = $prevEap
                Pop-Location
            }
        }

        'api-8080' {
            if (Test-PortListening 8080) {
                Write-Host "  [OK] api-8080 ya esta escuchando en :8080" -ForegroundColor Green
                return
            }
            $dir = Join-Path $RepoRoot 'venta-vehiculos-api_security'
            Write-Host "  [..] Abriendo terminal para api-8080 (Spring Boot :8080)..." -ForegroundColor Cyan
            Launch-ServiceTerminal -Title 'API-8080' -WorkingDir $dir -CommandScript '.\mvnw.cmd spring-boot:run'
        }

        'api-8082' {
            if (Test-PortListening 8082) {
                Write-Host "  [OK] api-8082 ya esta escuchando en :8082" -ForegroundColor Green
                return
            }
            $dir = Join-Path $RepoRoot 'venta-vehiculos-api_security'
            Write-Host "  [..] Abriendo terminal para api-8082 (Spring Boot :8082)..." -ForegroundColor Cyan
            Launch-ServiceTerminal -Title 'API-8082' -WorkingDir $dir -CommandScript '.\mvnw.cmd spring-boot:run "-Dspring-boot.run.arguments=--server.port=8082"'
        }

        'nginx' {
            if (Test-PortListening 8090) {
                Write-Host "  [OK] nginx ya esta escuchando en :8090" -ForegroundColor Green
                return
            }
            $nginxExe = Get-NginxExe
            if (-not $nginxExe) {
                Write-Host "  [ERROR] No se encontro nginx.exe (buscado en C:\nginx\nginx.exe y PATH)." -ForegroundColor Red
                return
            }
            $nginxDir = Split-Path -Parent $nginxExe
            $repoConf = Join-Path $RepoRoot 'nginx\nginx.conf'
            $destConf = Join-Path $nginxDir 'conf\nginx.conf'
            if ($SyncNginx -or (-not (Test-Path $destConf))) {
                Copy-Item -Path $repoConf -Destination $destConf -Force
                Write-Host "  [INFO] nginx.conf sincronizado en $destConf" -ForegroundColor DarkCyan
            }
            Write-Host "  [..] Validando e iniciando nginx (:8090)..." -ForegroundColor Cyan
            Push-Location $nginxDir
            $prevEap = $ErrorActionPreference
            try {
                $ErrorActionPreference = 'Continue'
                & $nginxExe -t 2>&1 | Out-Null
                if ($LASTEXITCODE -ne 0) {
                    Write-Host "  [ERROR] Error de sintaxis en $destConf. Ejecuta '$nginxExe -t' para ver el detalle." -ForegroundColor Red
                    return
                }
                Start-Process -FilePath $nginxExe -WorkingDirectory $nginxDir -WindowStyle Hidden
                Start-Sleep -Milliseconds 600
                if (Test-PortListening 8090) {
                    Write-Host "  [OK] nginx iniciado en http://127.0.0.1:8090" -ForegroundColor Green
                } else {
                    Write-Host "  [AVISO] nginx se lanzo pero :8090 aun no responde." -ForegroundColor Yellow
                }
            } finally {
                $ErrorActionPreference = $prevEap
                Pop-Location
            }
        }

        'ventas' {
            if (Test-PortListening 8002) {
                Write-Host "  [OK] ventas ya esta escuchando en :8002" -ForegroundColor Green
                return
            }
            $dir = Join-Path $RepoRoot 'ventas-api'
            $py = Join-Path $dir '.venv\Scripts\python.exe'
            $cmd = if (Test-Path $py) {
                '.\.venv\Scripts\python.exe -m uvicorn app.main:app --host 127.0.0.1 --port 8002 --reload'
            } else {
                'python -m uvicorn app.main:app --host 127.0.0.1 --port 8002 --reload'
            }
            Write-Host "  [..] Abriendo terminal para ventas (FastAPI :8002)..." -ForegroundColor Cyan
            Launch-ServiceTerminal -Title 'Ventas-8002' -WorkingDir $dir -CommandScript $cmd
        }

        'worker' {
            $dir = Join-Path $RepoRoot 'ventas-api'
            $py = Join-Path $dir '.venv\Scripts\python.exe'
            $cmd = if (Test-Path $py) {
                '.\.venv\Scripts\python.exe -m app.worker'
            } else {
                'python -m app.worker'
            }
            Write-Host "  [..] Abriendo terminal para worker de conciliacion FastAPI..." -ForegroundColor Cyan
            Launch-ServiceTerminal -Title 'Worker-Ventas' -WorkingDir $dir -CommandScript $cmd
        }

        'notificaciones' {
            if (Test-PortListening 8081) {
                Write-Host "  [OK] notificaciones ya esta escuchando en :8081" -ForegroundColor Green
                return
            }
            $dir = Join-Path $RepoRoot 'notificaciones-service'
            Write-Host "  [..] Abriendo terminal para notificaciones (Spring Boot :8081)..." -ForegroundColor Cyan
            Launch-ServiceTerminal -Title 'Notif-8081' -WorkingDir $dir -CommandScript 'mvn spring-boot:run'
        }

        'monitor' {
            if (Test-PortListening 8083) {
                Write-Host "  [OK] monitor ya esta escuchando en :8083" -ForegroundColor Green
                return
            }
            $dir = Join-Path $RepoRoot 'monitor-balancer-service'
            Write-Host "  [..] Abriendo terminal para monitor (Spring Boot :8083)..." -ForegroundColor Cyan
            Launch-ServiceTerminal -Title 'Monitor-8083' -WorkingDir $dir -CommandScript 'mvn spring-boot:run'
        }

        'pwa' {
            if (Test-PortListening 5173) {
                Write-Host "  [OK] pwa ya esta escuchando en :5173" -ForegroundColor Green
                return
            }
            if (-not $OAuth.ClientSecret) {
                Write-Host "  [AVISO] No se detecto OAUTH_CLIENT_SECRET. Define la variable o .env antes de iniciar sesion en PWA." -ForegroundColor Yellow
            }
            $dir = Join-Path $RepoRoot 'AplicacionMovilPWA\AplicacionMovilPWA'
            $script = @"
`$env:API_BASE_URL = '$($OAuth.GatewayUrl)'
`$env:OAUTH_CLIENT_ID = '$($OAuth.ClientId)'
`$env:OAUTH_CLIENT_SECRET = '$($OAuth.ClientSecret)'
npm start
"@
            Write-Host "  [..] Abriendo terminal para pwa (http://localhost:5173)..." -ForegroundColor Cyan
            Launch-ServiceTerminal -Title 'PWA-5173' -WorkingDir $dir -CommandScript $script
        }

        'react' {
            if (Test-PortListening 5174) {
                Write-Host "  [OK] react ya esta escuchando en :5174" -ForegroundColor Green
                return
            }
            if (-not $OAuth.ClientSecret) {
                Write-Host "  [AVISO] No se detecto OAUTH_CLIENT_SECRET. Define la variable o .env antes de iniciar sesion en React." -ForegroundColor Yellow
            }
            $dir = Join-Path $RepoRoot 'reactweb\react-fetch-vehiculos'
            $script = @"
`$env:VITE_GATEWAY_URL = '$($OAuth.GatewayUrl)'
`$env:OAUTH_CLIENT_ID = '$($OAuth.ClientId)'
`$env:OAUTH_CLIENT_SECRET = '$($OAuth.ClientSecret)'
npm run dev
"@
            Write-Host "  [..] Abriendo terminal para react (http://localhost:5174)..." -ForegroundColor Cyan
            Launch-ServiceTerminal -Title 'React-5174' -WorkingDir $dir -CommandScript $script
        }

        'winforms' {
            $dir = Join-Path $RepoRoot 'ConsultaVehiculosForms\ConsultaVehiculosForms'
            $script = @"
`$env:APE04_GATEWAY_URL = '$($OAuth.GatewayUrl)'
`$env:APE04_OAUTH_CLIENT_ID = '$($OAuth.ClientId)'
`$env:APE04_OAUTH_CLIENT_SECRET = '$($OAuth.ClientSecret)'
dotnet run
"@
            Write-Host "  [..] Iniciando cliente escritorio WinForms..." -ForegroundColor Cyan
            Launch-ServiceTerminal -Title 'WinForms-Admin' -WorkingDir $dir -CommandScript $script
        }

        'javafx' {
            $dir = Join-Path $RepoRoot 'escritorioFX\consultarvehiculojavafx'
            $script = @"
`$env:APE04_NOTIF_BASE_URL = '$($OAuth.GatewayUrl)'
mvn javafx:run
"@
            Write-Host "  [..] Iniciando cliente escritorio JavaFX..." -ForegroundColor Cyan
            Launch-ServiceTerminal -Title 'JavaFX-Monitor' -WorkingDir $dir -CommandScript $script
        }
    }
}

# ==================== DETENER UN SERVICIO ====================

function Stop-ByPort {
    param([string]$Label, [int]$Port)
    $pidFound = Get-ListeningPid -Port $Port
    if (-not $pidFound) {
        Write-Host "  [--] $Label (:$Port) ya estaba detenido." -ForegroundColor DarkGray
        return
    }
    Stop-ProcessTreeSmart -TargetPid $pidFound
    Start-Sleep -Milliseconds 350
    if (-not (Test-PortListening $Port)) {
        Write-Host "  [DOWN] $Label (:$Port) detenido correctamente (PID $pidFound)." -ForegroundColor Yellow
    } else {
        $pid2 = Get-ListeningPid -Port $Port
        if ($pid2) { & taskkill /PID $pid2 /T /F 2>$null | Out-Null }
        Write-Host "  [DOWN] $Label (:$Port) forzado a detenerse." -ForegroundColor Yellow
    }
}

function Stop-OneService {
    param([string]$Name)

    switch ($Name) {
        'rabbitmq' {
            Write-Host "  [..] Deteniendo contenedor ape04-rabbitmq (conserva cola durable)..." -ForegroundColor Cyan
            & docker stop ape04-rabbitmq 2>$null | Out-Null
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  [DOWN] rabbitmq detenido." -ForegroundColor Yellow
            } else {
                Write-Host "  [--] rabbitmq no estaba corriendo." -ForegroundColor DarkGray
            }
        }

        'api-8080'       { Stop-ByPort -Label 'api-8080' -Port 8080 }
        'api-8082'       { Stop-ByPort -Label 'api-8082' -Port 8082 }
        'ventas'         { Stop-ByPort -Label 'ventas' -Port 8002 }
        'notificaciones' { Stop-ByPort -Label 'notificaciones' -Port 8081 }
        'monitor'        { Stop-ByPort -Label 'monitor' -Port 8083 }
        'pwa'            { Stop-ByPort -Label 'pwa' -Port 5173 }
        'react'          { Stop-ByPort -Label 'react' -Port 5174 }

        'nginx' {
            $procs = Get-Process -Name 'nginx' -ErrorAction SilentlyContinue
            if (-not $procs) {
                Write-Host "  [--] nginx (:8090) ya estaba detenido." -ForegroundColor DarkGray
                return
            }
            $nginxExe = Get-NginxExe
            if ($nginxExe) {
                $nginxDir = Split-Path -Parent $nginxExe
                Push-Location $nginxDir
                try { & $nginxExe -s stop 2>$null | Out-Null } finally { Pop-Location }
                Start-Sleep -Milliseconds 300
            }
            Get-Process -Name 'nginx' -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
            Write-Host "  [DOWN] nginx (:8090) detenido." -ForegroundColor Yellow
        }

        'worker' {
            $procs = Get-CimInstance Win32_Process -Filter "Name='python.exe'" -ErrorAction SilentlyContinue |
                Where-Object { $_.CommandLine -match 'app\.worker' }
            if (-not $procs) {
                Write-Host "  [--] worker no estaba corriendo." -ForegroundColor DarkGray
                return
            }
            foreach ($p in $procs) {
                Stop-ProcessTreeSmart -TargetPid ([int]$p.ProcessId)
            }
            Write-Host "  [DOWN] worker detenido." -ForegroundColor Yellow
        }

        'winforms' {
            $procs = Get-Process -Name 'ConsultaVehiculosForms' -ErrorAction SilentlyContinue
            if (-not $procs) {
                Write-Host "  [--] winforms no estaba abierto." -ForegroundColor DarkGray
                return
            }
            $procs | Stop-Process -Force -ErrorAction SilentlyContinue
            Write-Host "  [DOWN] winforms cerrado." -ForegroundColor Yellow
        }

        'javafx' {
            $procs = Get-CimInstance Win32_Process -Filter "Name='java.exe'" -ErrorAction SilentlyContinue |
                Where-Object { $_.CommandLine -match 'ec\.edu\.com\.(Launcher|VehiculosApp)|consultarvehiculojavafx' }
            if (-not $procs) {
                Write-Host "  [--] javafx no estaba abierto." -ForegroundColor DarkGray
                return
            }
            foreach ($p in $procs) {
                Stop-ProcessTreeSmart -TargetPid ([int]$p.ProcessId)
            }
            Write-Host "  [DOWN] javafx cerrado." -ForegroundColor Yellow
        }
    }
}

# ==================== ESTADO GENERAL ====================

function Show-Status {
    Write-Host ''
    Write-Host '  ESTADO DE SERVICIOS - APE 04 (NEXORA Autos)' -ForegroundColor White
    Write-Host '  ---------------------------------------------------------------------' -ForegroundColor DarkGray
    Write-Host ('  {0,-16} {1,-8} {2,-8} {3,-8} {4}' -f 'SERVICIO', 'PUERTO', 'ESTADO', 'PID', 'DESCRIPCION') -ForegroundColor Cyan
    Write-Host '  ---------------------------------------------------------------------' -ForegroundColor DarkGray

    $filas = @(
        @{ Nombre = 'mysql (XAMPP)';  Puerto = 3306; Desc = 'Base de datos MySQL (venta_vehiculos)'; Tipo = 'port' }
        @{ Nombre = 'rabbitmq';       Puerto = 5672; Desc = 'Broker AMQP durable (UI :15672)';       Tipo = 'port' }
        @{ Nombre = 'api-8080';       Puerto = 8080; Desc = 'Spring Boot Vehiculos Instancia A (3x)'; Tipo = 'port' }
        @{ Nombre = 'api-8082';       Puerto = 8082; Desc = 'Spring Boot Vehiculos Instancia B (1x)'; Tipo = 'port' }
        @{ Nombre = 'nginx';          Puerto = 8090; Desc = 'Gateway REST + Balanceador 3:1';        Tipo = 'port' }
        @{ Nombre = 'ventas';         Puerto = 8002; Desc = 'FastAPI Compradores y Ventas';          Tipo = 'port' }
        @{ Nombre = 'notificaciones'; Puerto = 8081; Desc = 'Spring Boot Consumidor RabbitMQ';       Tipo = 'port' }
        @{ Nombre = 'monitor';        Puerto = 8083; Desc = 'Monitor de Balanceo y Alertas';         Tipo = 'port' }
        @{ Nombre = 'pwa';            Puerto = 5173; Desc = 'PWA Movil (Registro de ventas)';        Tipo = 'port' }
        @{ Nombre = 'react';          Puerto = 5174; Desc = 'Cliente Web React (Solo consulta)';     Tipo = 'port' }
        @{ Nombre = 'worker';         Puerto = 0;    Desc = 'Worker conciliacion FastAPI';           Tipo = 'worker' }
        @{ Nombre = 'winforms';       Puerto = 0;    Desc = 'Admin Windows Forms (Escritorio)';      Tipo = 'winforms' }
        @{ Nombre = 'javafx';         Puerto = 0;    Desc = 'Monitor Notificaciones JavaFX';         Tipo = 'javafx' }
    )

    foreach ($f in $filas) {
        $procId = $null
        if ($f.Tipo -eq 'port') {
            $procId = Get-ListeningPid -Port $f.Puerto
        } elseif ($f.Tipo -eq 'worker') {
            $p = Get-CimInstance Win32_Process -Filter "Name='python.exe'" -ErrorAction SilentlyContinue |
                Where-Object { $_.CommandLine -match 'app\.worker' } | Select-Object -First 1
            if ($p) { $procId = [int]$p.ProcessId }
        } elseif ($f.Tipo -eq 'winforms') {
            $p = Get-Process -Name 'ConsultaVehiculosForms' -ErrorAction SilentlyContinue | Select-Object -First 1
            if ($p) { $procId = $p.Id }
        } elseif ($f.Tipo -eq 'javafx') {
            $p = Get-CimInstance Win32_Process -Filter "Name='java.exe'" -ErrorAction SilentlyContinue |
                Where-Object { $_.CommandLine -match 'ec\.edu\.com\.(Launcher|VehiculosApp)|consultarvehiculojavafx' } | Select-Object -First 1
            if ($p) { $procId = [int]$p.ProcessId }
        }

        $puertoTxt = if ($f.Puerto -gt 0) { ":$($f.Puerto)" } else { '-' }
        $estadoTxt = if ($procId) { 'UP' } else { 'DOWN' }
        $pidTxt    = if ($procId) { "$procId" } else { '-' }
        $color     = if ($procId) { 'Green' } else { 'DarkGray' }

        Write-Host ('  {0,-16} {1,-8} ' -f $f.Nombre, $puertoTxt) -NoNewline
        Write-Host ('{0,-8}' -f $estadoTxt) -ForegroundColor $color -NoNewline
        Write-Host (' {0,-8} {1}' -f $pidTxt, $f.Desc)
    }
    Write-Host '  ---------------------------------------------------------------------' -ForegroundColor DarkGray
    Write-Host ''
}

# ==================== AYUDA Y MENU INTERACTIVO ====================

function Show-Help {
    Write-Host ''
    Write-Host 'Uso: .\servicios.ps1 <accion> [servicio|grupo] [-SinWT] [-SyncNginx] [-ConEscritorio]' -ForegroundColor Cyan
    Write-Host ''
    Write-Host 'Acciones:' -ForegroundColor Yellow
    Write-Host '  status                 Muestra que servicios estan UP o DOWN y su PID'
    Write-Host '  start [objetivo]       Inicia todos los servicios (o uno especifico)'
    Write-Host '  stop [objetivo]        Detiene todos los servicios (o uno especifico)'
    Write-Host '  restart <objetivo>     Reinicia un servicio o grupo'
    Write-Host '  menu                   Abre un menu interactivo para pruebas en vivo'
    Write-Host ''
    Write-Host 'Servicios individuales:' -ForegroundColor Yellow
    Write-Host "  $($ServiceNames -join ', ')"
    Write-Host ''
    Write-Host 'Grupos:' -ForegroundColor Yellow
    Write-Host '  all       rabbitmq + backends + nginx + pwa + react'
    Write-Host '  todo      all + winforms + javafx'
    Write-Host '  backend   rabbitmq + api-8080 + api-8082 + nginx + ventas + notificaciones + monitor'
    Write-Host '  web       pwa + react'
    Write-Host '  desktop   winforms + javafx'
    Write-Host '  clientes  pwa + react + winforms + javafx'
    Write-Host ''
    Write-Host 'Ejemplos utiles en defensa:' -ForegroundColor Yellow
    Write-Host '  .\servicios.ps1 start'
    Write-Host '  .\servicios.ps1 stop api-8080          # Simular caida de Instancia A'
    Write-Host '  .\servicios.ps1 start api-8080         # Levantar de nuevo Instancia A'
    Write-Host '  .\servicios.ps1 stop notificaciones    # Probar cola durable en RabbitMQ'
    Write-Host '  .\servicios.ps1 stop all               # Apagar todo'
    Write-Host ''
}

function Run-Menu {
    while ($true) {
        Show-Status
        Write-Host '  MENU RAPIDO DE CONTROL' -ForegroundColor Yellow
        Write-Host '  [1] Iniciar stack principal (all: RabbitMQ + APIs + NGINX + PWA + React)'
        Write-Host '  [2] Iniciar TODO (incluye WinForms y JavaFX)'
        Write-Host '  [3] BAJAR un servicio especifico (ej. api-8080, notificaciones, nginx, rabbitmq)'
        Write-Host '  [4] SUBIR un servicio especifico'
        Write-Host '  [5] REINICIAR un servicio especifico'
        Write-Host '  [6] BAJAR TODOS los servicios (stop all)'
        Write-Host '  [7] Refrescar estado'
        Write-Host '  [0] Salir'
        Write-Host ''
        $op = Read-Host '  Elige una opcion'

        switch ($op) {
            '1' {
                try { Invoke-Start 'all' } catch { Write-Host "  [ERROR] $_" -ForegroundColor Red }
                Read-Host '  Presiona Enter para refrescar el estado' | Out-Null
            }
            '2' {
                try { Invoke-Start 'todo' } catch { Write-Host "  [ERROR] $_" -ForegroundColor Red }
                Read-Host '  Presiona Enter para refrescar el estado' | Out-Null
            }
            '3' {
                Write-Host "  Servicios: $($ServiceNames -join ', ')" -ForegroundColor DarkCyan
                $s = Read-Host '  Nombre del servicio a BAJAR'
                if ($s) {
                    try { Invoke-Stop $s } catch { Write-Host "  [ERROR] $_" -ForegroundColor Red }
                    Read-Host '  Presiona Enter para continuar' | Out-Null
                }
            }
            '4' {
                Write-Host "  Servicios: $($ServiceNames -join ', ')" -ForegroundColor DarkCyan
                $s = Read-Host '  Nombre del servicio a SUBIR'
                if ($s) {
                    try { Invoke-Start $s } catch { Write-Host "  [ERROR] $_" -ForegroundColor Red }
                    Read-Host '  Presiona Enter para continuar' | Out-Null
                }
            }
            '5' {
                Write-Host "  Servicios: $($ServiceNames -join ', ')" -ForegroundColor DarkCyan
                $s = Read-Host '  Nombre del servicio a REINICIAR'
                if ($s) {
                    try { Invoke-Stop $s; Start-Sleep -Seconds 1; Invoke-Start $s } catch { Write-Host "  [ERROR] $_" -ForegroundColor Red }
                    Read-Host '  Presiona Enter para continuar' | Out-Null
                }
            }
            '6' {
                try { Invoke-Stop 'all' } catch { Write-Host "  [ERROR] $_" -ForegroundColor Red }
                Read-Host '  Presiona Enter para continuar' | Out-Null
            }
            '7' { }
            '0' { return }
            default { Write-Host '  Opcion no valida.' -ForegroundColor Red; Start-Sleep -Seconds 1 }
        }
    }
}

# ==================== EJECUTORES PRINCIPALES ====================

function Invoke-Start {
    param([string]$Target)
    $lista = Resolve-Targets -Name $Target
    $oauth = Get-OAuthConfig

    # Verificacion rapida de MySQL cuando se levantan servicios de BD
    if ($lista | Where-Object { $_ -in @('api-8080', 'api-8082', 'ventas') }) {
        if (-not (Test-PortListening 3306)) {
            Write-Host '  [AVISO] MySQL (:3306) no esta escuchando. Inicia MySQL en XAMPP antes de usar las APIs.' -ForegroundColor Yellow
        }
    }

    Write-Host ''
    Write-Host "Iniciando: $($lista -join ', ')" -ForegroundColor White
    foreach ($svc in $lista) {
        try {
            Start-OneService -Name $svc -OAuth $oauth
        } catch {
            Write-Host "  [ERROR] Fallo al iniciar '$svc': $_" -ForegroundColor Red
        }
        if ($lista.Count -gt 1) {
            Start-Sleep -Milliseconds 450
        }
    }
    Write-Host ''
    Write-Host "Listo. Ejecuta '.\servicios.ps1 status' en unos segundos para confirmar que los puertos esten UP." -ForegroundColor Green
}

function Invoke-Stop {
    param([string]$Target)
    $lista = Resolve-Targets -Name $Target
    if ($Target.Trim().ToLowerInvariant() -eq 'all') {
        # Al bajar 'all', incluimos tambien desktop y worker por si estuvieran abiertos
        $lista = @('javafx', 'winforms', 'react', 'pwa', 'monitor', 'notificaciones', 'worker', 'ventas', 'nginx', 'api-8082', 'api-8080', 'rabbitmq')
    }
    Write-Host ''
    Write-Host "Deteniendo: $($lista -join ', ')" -ForegroundColor White
    foreach ($svc in $lista) {
        Stop-OneService -Name $svc
    }
    Write-Host ''
}

# ==================== PUNTO DE ENTRADA ====================

switch ($Accion) {
    'status'  { Show-Status }
    'start'   { Invoke-Start -Target $Objetivo }
    'stop'    { Invoke-Stop -Target $Objetivo }
    'restart' { Invoke-Stop -Target $Objetivo; Start-Sleep -Seconds 1; Invoke-Start -Target $Objetivo }
    'menu'    { Run-Menu }
    'help'    { Show-Help }
}
