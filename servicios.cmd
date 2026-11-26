@echo off
title NEXORA Autos - Gestor de Servicios (APE 04)
if "%~1"=="" (
    powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0servicios.ps1" menu
    if errorlevel 1 pause
) else (
    powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0servicios.ps1" %*
)
