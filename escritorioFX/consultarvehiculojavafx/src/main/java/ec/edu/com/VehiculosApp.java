package ec.edu.com;

import javafx.animation.KeyFrame;
import javafx.animation.Timeline;
import javafx.application.Application;
import javafx.application.Platform;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.collections.transformation.FilteredList;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.input.Clipboard;
import javafx.scene.input.ClipboardContent;
import javafx.scene.layout.*;
import javafx.scene.paint.Color;
import javafx.stage.Stage;
import javafx.util.Duration;

import java.math.BigDecimal;
import java.text.NumberFormat;
import java.time.LocalTime;
import java.time.format.DateTimeFormatter;
import java.util.HashSet;
import java.util.List;
import java.util.Locale;
import java.util.Set;

/**
 * Cliente de escritorio JavaFX — NEXORA Autos Monitor (Actividad 9 · APE 04).
 * Identidad compartida: Grafito #0B0F17 · Canvas #F8FAFC · Cobre #EA580C/#C2410C.
 * Consume exclusivamente GET /api/notificaciones vía NGINX (:8090),
 * sin acceso directo a RabbitMQ ni a MySQL.
 */
public final class VehiculosApp extends Application {

    private static final DateTimeFormatter HORA_FMT = DateTimeFormatter.ofPattern("HH:mm:ss");
    private static final NumberFormat MONEDA_FMT = NumberFormat.getCurrencyInstance(Locale.US);
    private static final String DEFAULT_BASE_URL =
            System.getenv().getOrDefault("APE04_NOTIF_BASE_URL", "http://127.0.0.1:8090");

    // Datos y detección de nuevas notificaciones
    private final ObservableList<NotificacionVenta> datosNotificaciones = FXCollections.observableArrayList();
    private final FilteredList<NotificacionVenta> datosFiltrados = new FilteredList<>(datosNotificaciones, p -> true);
    private final Set<String> eventosConocidos = new HashSet<>();
    private final Set<String> eventosNuevosRecientes = new HashSet<>();
    private boolean primeraCargaCompletada = false;

    // Controles principales
    private final TextField txtBaseUrl = new TextField(DEFAULT_BASE_URL);
    private final TextField txtFiltro = new TextField();
    private final Button btnActualizar = new Button("Actualizar ahora");
    private final CheckBox chkAutoSondeo = new CheckBox("Auto-sondeo 3 s");

    // Navegación sidebar
    private final Button btnNavMonitor = new Button("  Monitor en vivo");
    private final Button btnNavConexion = new Button("  Conexión y Gateway");
    private final StackPane contenedorVistas = new StackPane();
    private VBox vistaMonitor;
    private VBox vistaConexion;

    // Cabecera dinámica
    private final Label lblTopTitle = new Label("Monitor de Notificaciones");
    private final Label lblTopSub = new Label("Recepción automática desde notificaciones-service vía NGINX (:8090)");

    // Indicadores KPI y estado
    private final Label lblConexionBadge = new Label("CONECTANDO…");
    private final Label lblSidebarHora = new Label("Última sync: —");
    private final Label lblKpiTotal = new Label("0");
    private final Label lblKpiUltima = new Label("Sin ventas");
    private final Label lblKpiUltimaSub = new Label("Esperando eventos de venta");
    private final Label lblKpiMonto = new Label("$0.00");
    private final Label lblKpiMontoSub = new Label("Suma real de avisos recibidos");
    private final Label lblKpiSondeo = new Label("Activo (3 s)");
    private final Label lblKpiSondeoSub = new Label("Sondeo automático HTTP");

    // Detalle del evento seleccionado
    private final Label lblSelResumen = new Label("Seleccione una fila para inspeccionar su eventoId completo.");
    private final Button btnCopiarEvento = new Button("Copiar Evento ID");
    private String eventoIdSeleccionado = "";

    private final Label lblEstado = new Label("Conectando con el servicio de notificaciones vía NGINX (:8090)…");
    private final Label lblResumen = new Label("0 avisos");
    private final TableView<NotificacionVenta> tablaNotificaciones = new TableView<>();

    private Timeline timerSondeo;
    private boolean consultaEnCurso = false;

    public static void main(String[] args) {
        launch(args);
    }

    @Override
    public void start(Stage primaryStage) {
        VBox sidebar = construirSidebar();
        HBox topbar = construirTopbar();

        vistaMonitor = construirVistaMonitor();
        vistaConexion = construirVistaConexion();
        vistaConexion.setVisible(false);
        vistaConexion.setManaged(false);

        contenedorVistas.getChildren().addAll(vistaMonitor, vistaConexion);
        VBox.setVgrow(contenedorVistas, Priority.ALWAYS);

        HBox barraInferior = new HBox(16, lblEstado, lblResumen);
        HBox.setHgrow(lblEstado, Priority.ALWAYS);
        barraInferior.setAlignment(Pos.CENTER_LEFT);
        barraInferior.getStyleClass().add("status-bar");
        lblEstado.getStyleClass().add("status-text");
        lblResumen.getStyleClass().add("badge-graphite");

        VBox derecha = new VBox(0, topbar, contenedorVistas, barraInferior);
        HBox.setHgrow(derecha, Priority.ALWAYS);

        HBox root = new HBox(0, sidebar, derecha);

        configurarEventos();

        // Sondeo automático limitado cada 3 segundos
        timerSondeo = new Timeline(new KeyFrame(Duration.seconds(3), e -> consultarNotificaciones()));
        timerSondeo.setCycleCount(Timeline.INDEFINITE);
        timerSondeo.play();

        Scene scene = new Scene(root, 1260, 700);
        var cssUrl = getClass().getResource("styles.css");
        if (cssUrl != null) {
            scene.getStylesheets().add(cssUrl.toExternalForm());
        }

        primaryStage.setTitle("NEXORA Autos · Monitor de Notificaciones — JavaFX (APE 04)");
        primaryStage.setMinWidth(1040);
        primaryStage.setMinHeight(600);
        primaryStage.setScene(scene);
        primaryStage.show();

        consultarNotificaciones();
    }

    private VBox construirSidebar() {
        VBox brandBox = new VBox(8);
        brandBox.getStyleClass().add("brand-box");
        brandBox.setAlignment(Pos.CENTER);

        var logoStream = getClass().getResourceAsStream("nexora-logo.png");
        if (logoStream != null) {
            ImageView logoView = new ImageView(new Image(logoStream));
            logoView.setFitWidth(182);
            logoView.setPreserveRatio(true);
            logoView.setSmooth(true);
            brandBox.getChildren().add(logoView);
        } else {
            Label fallback = new Label("NEXORA AUTOS");
            fallback.setStyle("-fx-text-fill: white; -fx-font-size: 16px; -fx-font-weight: bold;");
            brandBox.getChildren().add(fallback);
        }

        Label lblPill = new Label("MONITOR JAVAFX · APE 04");
        lblPill.getStyleClass().add("brand-sub-pill");
        brandBox.getChildren().add(lblPill);

        Label lblSecNav = new Label("PLATAFORMA");
        lblSecNav.getStyleClass().add("nav-section-label");

        btnNavMonitor.setMaxWidth(Double.MAX_VALUE);
        btnNavMonitor.getStyleClass().addAll("nav-button", "nav-button-active");

        btnNavConexion.setMaxWidth(Double.MAX_VALUE);
        btnNavConexion.getStyleClass().add("nav-button");

        Region spacer = new Region();
        VBox.setVgrow(spacer, Priority.ALWAYS);

        lblConexionBadge.setMaxWidth(Double.MAX_VALUE);
        lblConexionBadge.setAlignment(Pos.CENTER);
        actualizarBadge("CONECTANDO…", "badge-info");

        lblSidebarHora.getStyleClass().add("sidebar-meta");
        Label lblArqNota = new Label("GET /api/notificaciones\nSin acceso directo a RabbitMQ/BD");
        lblArqNota.getStyleClass().add("sidebar-meta-dim");
        lblArqNota.setWrapText(true);

        VBox gatewayBox = new VBox(7, lblConexionBadge, lblSidebarHora, lblArqNota);
        gatewayBox.getStyleClass().add("sidebar-gateway-box");

        VBox footer = new VBox(gatewayBox);
        footer.getStyleClass().add("sidebar-footer");

        VBox sidebar = new VBox(4, brandBox, lblSecNav, btnNavMonitor, btnNavConexion, spacer, footer);
        sidebar.getStyleClass().add("sidebar");
        return sidebar;
    }

    private HBox construirTopbar() {
        lblTopTitle.getStyleClass().add("page-title");
        lblTopSub.getStyleClass().add("page-subtitle");
        VBox titleBox = new VBox(2, lblTopTitle, lblTopSub);

        Region spacer = new Region();
        HBox.setHgrow(spacer, Priority.ALWAYS);

        Label lblCanal = new Label("NGINX :8090 · Tiempo real");
        lblCanal.getStyleClass().add("badge-graphite");

        chkAutoSondeo.setSelected(true);
        btnActualizar.getStyleClass().add("btn-copper");

        HBox topbar = new HBox(14, titleBox, spacer, lblCanal, chkAutoSondeo, btnActualizar);
        topbar.setAlignment(Pos.CENTER_LEFT);
        topbar.getStyleClass().add("topbar");
        return topbar;
    }

    private VBox construirVistaMonitor() {
        // 4 Tarjetas KPI con borde superior de acento (como en React Web)
        VBox kpi1 = crearKpiCard("NOTIFICACIONES", lblKpiTotal, new Label("Deduplicadas por eventoId"), "kpi-card-graphite");
        VBox kpi2 = crearKpiCard("ÚLTIMA VENTA", lblKpiUltima, lblKpiUltimaSub, "kpi-card-green");
        VBox kpi3 = crearKpiCard("MONTO NOTIFICADO", lblKpiMonto, lblKpiMontoSub, "kpi-card-copper");
        VBox kpi4 = crearKpiCard("SINCRONIZACIÓN", lblKpiSondeo, lblKpiSondeoSub, "kpi-card-blue");

        HBox kpiRow = new HBox(14, kpi1, kpi2, kpi3, kpi4);
        HBox.setHgrow(kpi1, Priority.ALWAYS);
        HBox.setHgrow(kpi2, Priority.ALWAYS);
        HBox.setHgrow(kpi3, Priority.ALWAYS);
        HBox.setHgrow(kpi4, Priority.ALWAYS);

        // Barra de búsqueda rápida local
        txtFiltro.setPromptText("Buscar por Venta ID, Vehículo ID, Evento ID o mensaje…");
        HBox.setHgrow(txtFiltro, Priority.ALWAYS);

        Button btnLimpiarFiltro = new Button("Limpiar");
        btnLimpiarFiltro.getStyleClass().add("btn-secondary");
        btnLimpiarFiltro.setOnAction(e -> txtFiltro.clear());

        HBox filterRow = new HBox(10, txtFiltro, btnLimpiarFiltro);
        filterRow.setAlignment(Pos.CENTER_LEFT);

        configurarTabla();
        VBox.setVgrow(tablaNotificaciones, Priority.ALWAYS);

        // Franja oscura de inspección del evento seleccionado + botón Copiar Evento ID
        Label lblDetCap = new Label("TRAZABILIDAD DEL EVENTO SELECCIONADO");
        lblDetCap.getStyleClass().add("event-detail-label");
        lblSelResumen.getStyleClass().add("event-detail-value");
        VBox detTexts = new VBox(2, lblDetCap, lblSelResumen);
        HBox.setHgrow(detTexts, Priority.ALWAYS);

        btnCopiarEvento.getStyleClass().add("btn-dark-copy");
        btnCopiarEvento.setDisable(true);
        btnCopiarEvento.setOnAction(e -> {
            if (!eventoIdSeleccionado.isBlank()) {
                ClipboardContent content = new ClipboardContent();
                content.putString(eventoIdSeleccionado);
                Clipboard.getSystemClipboard().setContent(content);
                btnCopiarEvento.setText("¡Copiado!");
                Timeline reset = new Timeline(new KeyFrame(Duration.seconds(1.5), ev -> btnCopiarEvento.setText("Copiar Evento ID")));
                reset.play();
            }
        });

        HBox detailBar = new HBox(14, detTexts, btnCopiarEvento);
        detailBar.setAlignment(Pos.CENTER_LEFT);
        detailBar.getStyleClass().add("event-detail-bar");

        VBox vista = new VBox(14, kpiRow, filterRow, tablaNotificaciones, detailBar);
        vista.getStyleClass().add("content-area");
        VBox.setVgrow(vista, Priority.ALWAYS);
        return vista;
    }

    private VBox construirVistaConexion() {
        Label lblUrl = new Label("URL BASE DEL GATEWAY / SERVICIO DE NOTIFICACIONES");
        lblUrl.getStyleClass().add("kpi-label");
        txtBaseUrl.setPromptText("http://127.0.0.1:8090");

        Button btnProbar = new Button("Probar conexión ahora");
        btnProbar.getStyleClass().add("btn-copper");
        btnProbar.setOnAction(e -> {
            mostrarVista("monitor");
            consultarNotificaciones();
        });

        Button btnRestaurar = new Button("Restaurar http://127.0.0.1:8090");
        btnRestaurar.getStyleClass().add("btn-secondary");
        btnRestaurar.setOnAction(e -> txtBaseUrl.setText("http://127.0.0.1:8090"));

        HBox acciones = new HBox(10, btnProbar, btnRestaurar);

        Label lblInfo = new Label(
                "Regla arquitectónica APE 04: JavaFX nunca se conecta directamente a RabbitMQ ni a MySQL. " +
                "Todas las notificaciones provienen del endpoint GET /api/notificaciones expuesto por notificaciones-service a través de NGINX (:8090).");
        lblInfo.setWrapText(true);
        lblInfo.getStyleClass().add("page-subtitle");

        VBox card = new VBox(12, lblUrl, txtBaseUrl, acciones, new Separator(), lblInfo);
        card.getStyleClass().add("card");
        card.setMaxWidth(680);

        VBox vista = new VBox(14, card);
        vista.getStyleClass().add("content-area");
        return vista;
    }

    private VBox crearKpiCard(String etiqueta, Label lblValor, Label lblSub, String accentClass) {
        Label l1 = new Label(etiqueta);
        l1.getStyleClass().add("kpi-label");
        lblValor.getStyleClass().add("kpi-value");
        lblSub.getStyleClass().add("kpi-sub");
        VBox card = new VBox(4, l1, lblValor, lblSub);
        card.getStyleClass().addAll("card", accentClass);
        card.setMaxWidth(Double.MAX_VALUE);
        return card;
    }

    private void configurarEventos() {
        btnNavMonitor.setOnAction(e -> mostrarVista("monitor"));
        btnNavConexion.setOnAction(e -> mostrarVista("conexion"));
        btnActualizar.setOnAction(e -> consultarNotificaciones());
        txtBaseUrl.setOnAction(e -> consultarNotificaciones());

        tablaNotificaciones.getSelectionModel().selectedItemProperty().addListener((obs, oldSel, sel) -> {
            if (sel == null) {
                eventoIdSeleccionado = "";
                lblSelResumen.setText("Seleccione una fila para inspeccionar su eventoId completo.");
                btnCopiarEvento.setDisable(true);
            } else {
                eventoIdSeleccionado = valorSeguro(sel.eventoId());
                lblSelResumen.setText("Venta #" + sel.ventaId() + " · Vehículo #" + sel.vehiculoId()
                        + " · eventoId: " + eventoIdSeleccionado);
                btnCopiarEvento.setDisable(sel.eventoId() == null || sel.eventoId().isBlank());
            }
        });

        txtFiltro.textProperty().addListener((obs, oldVal, newVal) -> {
            String q = newVal == null ? "" : newVal.trim().toLowerCase(Locale.ROOT);
            datosFiltrados.setPredicate(n -> {
                if (q.isEmpty()) return true;
                return String.valueOf(n.ventaId()).contains(q)
                        || String.valueOf(n.vehiculoId()).contains(q)
                        || valorSeguro(n.eventoId()).toLowerCase(Locale.ROOT).contains(q)
                        || valorSeguro(n.mensaje()).toLowerCase(Locale.ROOT).contains(q)
                        || valorSeguro(n.estado()).toLowerCase(Locale.ROOT).contains(q);
            });
        });

        chkAutoSondeo.setOnAction(e -> {
            if (chkAutoSondeo.isSelected()) {
                timerSondeo.play();
                lblKpiSondeo.setText("Activo (3 s)");
                lblKpiSondeo.setStyle("-fx-text-fill: #059669;");
                lblKpiSondeoSub.setText("Sondeo automático HTTP");
                consultarNotificaciones();
            } else {
                timerSondeo.stop();
                lblKpiSondeo.setText("Pausado");
                lblKpiSondeo.setStyle("-fx-text-fill: #D97706;");
                lblKpiSondeoSub.setText("Use 'Actualizar ahora'");
                lblEstado.setTextFill(Color.web("#D97706"));
                lblEstado.setText("Auto-sondeo pausado por el usuario. Use 'Actualizar ahora' para consultar.");
            }
        });
    }

    private void mostrarVista(String nombre) {
        boolean esMonitor = "monitor".equals(nombre);
        vistaMonitor.setVisible(esMonitor);
        vistaMonitor.setManaged(esMonitor);
        vistaConexion.setVisible(!esMonitor);
        vistaConexion.setManaged(!esMonitor);

        lblTopTitle.setText(esMonitor ? "Monitor de Notificaciones" : "Conexión y Gateway");
        lblTopSub.setText(esMonitor
                ? "Recepción automática desde notificaciones-service vía NGINX (:8090)"
                : "Configuración del punto de entrada REST para el consumo de avisos");

        btnNavMonitor.getStyleClass().remove("nav-button-active");
        btnNavConexion.getStyleClass().remove("nav-button-active");
        if (esMonitor) btnNavMonitor.getStyleClass().add("nav-button-active");
        else btnNavConexion.getStyleClass().add("nav-button-active");
    }

    @Override
    public void stop() {
        if (timerSondeo != null) {
            timerSondeo.stop();
        }
    }

    @SuppressWarnings("unchecked")
    private void configurarTabla() {
        TableColumn<NotificacionVenta, Integer> colVentaId = new TableColumn<>("VENTA");
        colVentaId.setCellValueFactory(c -> new SimpleObjectProperty<>(c.getValue().ventaId()));
        colVentaId.setCellFactory(col -> new TableCell<>() {
            @Override
            protected void updateItem(Integer item, boolean empty) {
                super.updateItem(item, empty);
                if (empty || item == null) {
                    setGraphic(null);
                    setText(null);
                } else {
                    Label pill = new Label("#" + item);
                    pill.getStyleClass().add("id-pill");
                    setGraphic(pill);
                    setText(null);
                }
            }
        });
        colVentaId.setPrefWidth(78);

        TableColumn<NotificacionVenta, Integer> colVehiculoId = new TableColumn<>("VEHÍCULO");
        colVehiculoId.setCellValueFactory(c -> new SimpleObjectProperty<>(c.getValue().vehiculoId()));
        colVehiculoId.setCellFactory(col -> new TableCell<>() {
            @Override
            protected void updateItem(Integer item, boolean empty) {
                super.updateItem(item, empty);
                setText(empty || item == null ? null : "Vehículo #" + item);
            }
        });
        colVehiculoId.setPrefWidth(100);

        TableColumn<NotificacionVenta, Integer> colCompradorId = new TableColumn<>("COMPRADOR");
        colCompradorId.setCellValueFactory(c -> new SimpleObjectProperty<>(c.getValue().compradorId()));
        colCompradorId.setCellFactory(col -> new TableCell<>() {
            @Override
            protected void updateItem(Integer item, boolean empty) {
                super.updateItem(item, empty);
                setText(empty || item == null ? null : "#" + item);
            }
        });
        colCompradorId.setPrefWidth(88);

        TableColumn<NotificacionVenta, String> colTipo = new TableColumn<>("TIPO EVENTO");
        colTipo.setCellValueFactory(c -> new SimpleStringProperty(valorSeguro(c.getValue().tipo())));
        colTipo.setPrefWidth(138);

        TableColumn<NotificacionVenta, String> colPrecio = new TableColumn<>("PRECIO");
        colPrecio.setCellValueFactory(c -> {
            String precio = c.getValue().precioAcordado();
            String moneda = c.getValue().moneda() != null ? c.getValue().moneda() : "USD";
            return new SimpleStringProperty(precio != null ? formatearMoneda(precio) + " " + moneda : "—");
        });
        colPrecio.setCellFactory(col -> new TableCell<>() {
            @Override
            protected void updateItem(String item, boolean empty) {
                super.updateItem(item, empty);
                setText(empty ? null : item);
                setAlignment(Pos.CENTER_RIGHT);
                setStyle(empty ? "" : "-fx-font-weight: bold;");
            }
        });
        colPrecio.setPrefWidth(128);

        TableColumn<NotificacionVenta, String> colEstado = new TableColumn<>("ESTADO");
        colEstado.setCellValueFactory(c -> new SimpleStringProperty(valorSeguro(c.getValue().estado())));
        colEstado.setCellFactory(col -> new TableCell<>() {
            @Override
            protected void updateItem(String item, boolean empty) {
                super.updateItem(item, empty);
                if (empty || item == null) {
                    setGraphic(null);
                    setText(null);
                } else {
                    Label pill = new Label(item);
                    String up = item.toUpperCase(Locale.ROOT);
                    if ("COMPLETADA".equals(up) || "DISPONIBLE".equals(up)) {
                        pill.getStyleClass().add("badge-ok");
                    } else if ("PENDIENTE".equals(up) || "EN_REVISION".equals(up) || "RESERVADO".equals(up)) {
                        pill.getStyleClass().add("badge-info");
                    } else if ("FALLIDA".equals(up)) {
                        pill.getStyleClass().add("badge-error");
                    } else {
                        pill.getStyleClass().add("badge-graphite");
                    }
                    setGraphic(pill);
                    setText(null);
                }
            }
        });
        colEstado.setPrefWidth(116);

        TableColumn<NotificacionVenta, String> colFecha = new TableColumn<>("FECHA / HORA");
        colFecha.setCellValueFactory(c -> new SimpleStringProperty(valorSeguro(c.getValue().fechaHora())));
        colFecha.setPrefWidth(160);

        TableColumn<NotificacionVenta, String> colEventoId = new TableColumn<>("EVENTO ID");
        colEventoId.setCellValueFactory(c -> new SimpleStringProperty(valorSeguro(c.getValue().eventoId())));
        colEventoId.setPrefWidth(175);

        TableColumn<NotificacionVenta, String> colMensaje = new TableColumn<>("MENSAJE");
        colMensaje.setCellValueFactory(c -> new SimpleStringProperty(valorSeguro(c.getValue().mensaje())));
        colMensaje.setPrefWidth(210);

        tablaNotificaciones.getColumns().addAll(
                colVentaId, colVehiculoId, colCompradorId, colTipo,
                colPrecio, colEstado, colFecha, colEventoId, colMensaje
        );
        tablaNotificaciones.setItems(datosFiltrados);
        tablaNotificaciones.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY_FLEX_LAST_COLUMN);

        // Resaltado verde suave en filas recién llegadas
        tablaNotificaciones.setRowFactory(tv -> new TableRow<>() {
            @Override
            protected void updateItem(NotificacionVenta item, boolean empty) {
                super.updateItem(item, empty);
                getStyleClass().remove("row-new");
                if (!empty && item != null && item.eventoId() != null && eventosNuevosRecientes.contains(item.eventoId())) {
                    getStyleClass().add("row-new");
                }
            }
        });

        tablaNotificaciones.setPlaceholder(crearPlaceholder(
                "Conectando con el servicio de notificaciones…",
                "#64748B", "#F8FAFC", "#CBD5E1"));
    }

    private void consultarNotificaciones() {
        if (consultaEnCurso) {
            return;
        }
        String baseUrl = txtBaseUrl.getText().trim();
        if (baseUrl.isEmpty()) {
            mostrarErrorServicio("Configure la URL base del gateway (ej. http://127.0.0.1:8090).");
            return;
        }

        consultaEnCurso = true;
        btnActualizar.setDisable(true);

        NotificacionesApiClient.listarNotificaciones(baseUrl)
                .thenAccept(resultado -> Platform.runLater(() -> {
                    consultaEnCurso = false;
                    btnActualizar.setDisable(false);
                    String hora = LocalTime.now().format(HORA_FMT);
                    int code = resultado.codigoHttp();

                    if (code == 200) {
                        List<NotificacionVenta> lista = resultado.notificaciones();

                        if (primeraCargaCompletada) {
                            Set<String> recienLlegados = new HashSet<>();
                            for (NotificacionVenta n : lista) {
                                if (n.eventoId() != null && !eventosConocidos.contains(n.eventoId())) {
                                    recienLlegados.add(n.eventoId());
                                }
                            }
                            if (!recienLlegados.isEmpty()) {
                                eventosNuevosRecientes.clear();
                                eventosNuevosRecientes.addAll(recienLlegados);
                            }
                        }
                        for (NotificacionVenta n : lista) {
                            if (n.eventoId() != null) eventosConocidos.add(n.eventoId());
                        }
                        primeraCargaCompletada = true;

                        datosNotificaciones.setAll(lista);
                        lblKpiTotal.setText(String.valueOf(lista.size()));
                        lblKpiMonto.setText(calcularMontoTotal(lista));
                        lblResumen.setText(lista.size() + (lista.size() == 1 ? " aviso" : " avisos"));
                        lblSidebarHora.setText("Última sync: " + hora);
                        actualizarBadge("CONECTADO · HTTP 200", "badge-ok");

                        if (lista.isEmpty()) {
                            lblKpiUltima.setText("Sin ventas");
                            lblKpiUltimaSub.setText("Cola vacía · Esperando eventos");
                            tablaNotificaciones.setPlaceholder(crearPlaceholder(
                                    "CONECTADO (HTTP 200) — No hay notificaciones de venta registradas todavía.\nRegistre una venta desde la PWA móvil para visualizarla aquí automáticamente.",
                                    "#059669", "#ECFDF5", "#A7F3D0"));
                            lblEstado.setTextFill(Color.web("#059669"));
                            lblEstado.setText("Conectado (HTTP 200) · Sin notificaciones registradas aún · Última verificación: " + hora);
                        } else {
                            NotificacionVenta ultima = lista.get(0);
                            lblKpiUltima.setText("Venta #" + ultima.ventaId());
                            lblKpiUltimaSub.setText("Vehículo #" + ultima.vehiculoId() + " · " + formatearMoneda(ultima.precioAcordado()));
                            lblEstado.setTextFill(Color.web("#059669"));
                            lblEstado.setText("Conectado (HTTP 200) · Última venta notificada: #" + ultima.ventaId()
                                    + " (Vehículo #" + ultima.vehiculoId() + ") · Sincronizado: " + hora);
                        }
                    } else {
                        mostrarErrorServicio("Error HTTP " + code + " desde " + baseUrl + "/api/notificaciones (" + hora + ")");
                    }
                }))
                .exceptionally(error -> {
                    Platform.runLater(() -> {
                        consultaEnCurso = false;
                        btnActualizar.setDisable(false);
                        String hora = LocalTime.now().format(HORA_FMT);
                        Throwable causa = error.getCause() != null ? error.getCause() : error;
                        mostrarErrorServicio("Sin conexión con el servicio de notificaciones (" + hora + "): " + causa.getMessage());
                    });
                    return null;
                });
    }

    private void mostrarErrorServicio(String mensajeError) {
        actualizarBadge("SIN CONEXIÓN · ERROR", "badge-error");
        lblEstado.setTextFill(Color.web("#DC2626"));
        lblEstado.setText(mensajeError);
        if (datosNotificaciones.isEmpty()) {
            tablaNotificaciones.setPlaceholder(crearPlaceholder(
                    "ERROR DE SERVICIO: No se pudo consultar GET /api/notificaciones.\nVerifique que NGINX (:8090) y notificaciones-service (:8081) estén en ejecución.",
                    "#DC2626", "#FEF2F2", "#FECACA"));
        }
    }

    private void actualizarBadge(String texto, String styleClass) {
        lblConexionBadge.setText(texto);
        lblConexionBadge.getStyleClass().removeAll("badge-ok", "badge-info", "badge-error", "badge-warn");
        lblConexionBadge.getStyleClass().add(styleClass);
    }

    private static String calcularMontoTotal(List<NotificacionVenta> lista) {
        BigDecimal total = BigDecimal.ZERO;
        for (NotificacionVenta n : lista) {
            if (n.precioAcordado() != null && !n.precioAcordado().isBlank()) {
                try {
                    total = total.add(new BigDecimal(n.precioAcordado().trim()));
                } catch (NumberFormatException ignored) {
                }
            }
        }
        return MONEDA_FMT.format(total);
    }

    private static String formatearMoneda(String valor) {
        if (valor == null || valor.isBlank()) return "—";
        try {
            return MONEDA_FMT.format(new BigDecimal(valor.trim()));
        } catch (NumberFormatException ex) {
            return "$" + valor;
        }
    }

    private static VBox crearPlaceholder(String texto, String fgHex, String bgHex, String borderHex) {
        Label lbl = new Label(texto);
        lbl.setStyle("-fx-text-fill: " + fgHex + "; -fx-font-size: 13px; -fx-font-weight: bold; -fx-text-alignment: center;");
        lbl.setWrapText(true);
        VBox box = new VBox(lbl);
        box.setAlignment(Pos.CENTER);
        box.setPadding(new Insets(18, 24, 18, 24));
        box.setMaxWidth(580);
        box.setMaxHeight(115);
        box.setStyle(
                "-fx-background-color: " + bgHex + ";" +
                "-fx-border-color: " + borderHex + ";" +
                "-fx-border-radius: 11; -fx-background-radius: 11;");
        return box;
    }

    private static String valorSeguro(String s) {
        return (s == null || s.isBlank()) ? "—" : s;
    }
}
