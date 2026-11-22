package ec.edu.com;

import javafx.animation.KeyFrame;
import javafx.animation.Timeline;
import javafx.application.Application;
import javafx.application.Platform;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.collections.FXCollections;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.layout.HBox;
import javafx.scene.layout.Priority;
import javafx.scene.layout.VBox;
import javafx.scene.paint.Color;
import javafx.stage.Stage;
import javafx.util.Duration;

import java.time.LocalTime;
import java.time.format.DateTimeFormatter;
import java.util.List;

/**
 * Cliente de escritorio JavaFX (Actividad 9 — APE 04).
 * Consulta periódicamente GET /api/notificaciones desde notificaciones-service (vía NGINX :8090)
 * sin acceder nunca directamente a RabbitMQ ni a MySQL.
 */
public final class VehiculosApp extends Application {

    private static final DateTimeFormatter HORA_FMT = DateTimeFormatter.ofPattern("HH:mm:ss");
    private static final String DEFAULT_BASE_URL =
            System.getenv().getOrDefault("APE04_NOTIF_BASE_URL", "http://127.0.0.1:8090");

    private final TextField txtBaseUrl = new TextField(DEFAULT_BASE_URL);
    private final Button btnActualizar = new Button("Actualizar ahora");
    private final CheckBox chkAutoSondeo = new CheckBox("Auto-sondeo cada 3 s");
    private final Label lblConexionBadge = new Label("INICIANDO...");
    private final Label lblEstado = new Label("Conectando con el servicio de notificaciones...");
    private final Label lblResumen = new Label("Total de notificaciones: 0");
    private final TableView<NotificacionVenta> tablaNotificaciones = new TableView<>();

    private Timeline timerSondeo;
    private boolean consultaEnCurso = false;

    public static void main(String[] args) {
        launch(args);
    }

    @Override
    public void start(Stage primaryStage) {
        Label lblTitulo = new Label("Monitor de Notificaciones de Ventas — JavaFX (APE 04)");
        lblTitulo.setStyle("-fx-font-size: 17px; -fx-font-weight: bold; -fx-text-fill: white;");
        Label lblSubtitulo = new Label("Consumo automático de GET /api/notificaciones vía NGINX (:8090) · Sin acceso directo a RabbitMQ ni MySQL");
        lblSubtitulo.setStyle("-fx-font-size: 11.5px; -fx-text-fill: #d6e4f0;");

        VBox header = new VBox(3, lblTitulo, lblSubtitulo);
        header.setPadding(new Insets(12, 16, 12, 16));
        header.setStyle("-fx-background-color: #182b49;");

        txtBaseUrl.setPromptText("http://127.0.0.1:8090");
        HBox.setHgrow(txtBaseUrl, Priority.ALWAYS);

        chkAutoSondeo.setSelected(true);
        btnActualizar.setStyle("-fx-background-color: #0d6efd; -fx-text-fill: white; -fx-font-weight: bold;");

        lblConexionBadge.setPadding(new Insets(5, 12, 5, 12));
        actualizarBadge("CONECTANDO", "#6c757d");

        HBox barraControl = new HBox(10,
                new Label("Gateway / API:"), txtBaseUrl,
                btnActualizar, chkAutoSondeo, lblConexionBadge);
        barraControl.setAlignment(Pos.CENTER_LEFT);
        barraControl.setPadding(new Insets(10, 14, 6, 14));

        configurarTabla();

        lblEstado.setWrapText(true);
        lblEstado.setStyle("-fx-font-size: 12.5px; -fx-font-weight: bold;");
        lblResumen.setStyle("-fx-font-size: 12.5px; -fx-font-weight: bold; -fx-text-fill: #1f3864;");

        HBox barraInferior = new HBox(16, lblEstado, lblResumen);
        HBox.setHgrow(lblEstado, Priority.ALWAYS);
        barraInferior.setAlignment(Pos.CENTER_LEFT);
        barraInferior.setPadding(new Insets(8, 14, 12, 14));
        barraInferior.setStyle("-fx-background-color: #f4f6f9; -fx-border-color: #dce3ec; -fx-border-width: 1 0 0 0;");

        VBox tablaBox = new VBox(tablaNotificaciones);
        tablaBox.setPadding(new Insets(4, 14, 8, 14));
        VBox.setVgrow(tablaNotificaciones, Priority.ALWAYS);
        VBox.setVgrow(tablaBox, Priority.ALWAYS);

        VBox root = new VBox(0, header, barraControl, tablaBox, barraInferior);
        root.setStyle("-fx-background-color: #ffffff;");

        btnActualizar.setOnAction(e -> consultarNotificaciones());
        txtBaseUrl.setOnAction(e -> consultarNotificaciones());
        chkAutoSondeo.setOnAction(e -> {
            if (chkAutoSondeo.isSelected()) {
                timerSondeo.play();
                consultarNotificaciones();
            } else {
                timerSondeo.stop();
                lblEstado.setText("Auto-sondeo pausado por el usuario. Use 'Actualizar ahora' para consultar.");
                lblEstado.setTextFill(Color.DARKSLATEBLUE);
            }
        });

        // Sondeo automático limitado cada 3 segundos
        timerSondeo = new Timeline(new KeyFrame(Duration.seconds(3), e -> consultarNotificaciones()));
        timerSondeo.setCycleCount(Timeline.INDEFINITE);
        timerSondeo.play();

        primaryStage.setTitle("Notificaciones de Ventas en Tiempo Real — JavaFX (APE 04)");
        primaryStage.setScene(new Scene(root, 1080, 560));
        primaryStage.show();

        // Primera consulta inmediata al abrir la ventana
        consultarNotificaciones();
    }

    @Override
    public void stop() {
        if (timerSondeo != null) {
            timerSondeo.stop();
        }
    }

    @SuppressWarnings("unchecked")
    private void configurarTabla() {
        TableColumn<NotificacionVenta, Integer> colVentaId = new TableColumn<>("Venta ID");
        colVentaId.setCellValueFactory(c -> new SimpleObjectProperty<>(c.getValue().ventaId()));
        colVentaId.setPrefWidth(78);

        TableColumn<NotificacionVenta, Integer> colVehiculoId = new TableColumn<>("Vehículo ID");
        colVehiculoId.setCellValueFactory(c -> new SimpleObjectProperty<>(c.getValue().vehiculoId()));
        colVehiculoId.setPrefWidth(88);

        TableColumn<NotificacionVenta, Integer> colCompradorId = new TableColumn<>("Comprador ID");
        colCompradorId.setCellValueFactory(c -> new SimpleObjectProperty<>(c.getValue().compradorId()));
        colCompradorId.setPrefWidth(95);

        TableColumn<NotificacionVenta, String> colTipo = new TableColumn<>("Tipo");
        colTipo.setCellValueFactory(c -> new SimpleStringProperty(valorSeguro(c.getValue().tipo())));
        colTipo.setPrefWidth(135);

        TableColumn<NotificacionVenta, String> colPrecio = new TableColumn<>("Precio");
        colPrecio.setCellValueFactory(c -> {
            String precio = c.getValue().precioAcordado();
            String moneda = c.getValue().moneda() != null ? c.getValue().moneda() : "USD";
            return new SimpleStringProperty(precio != null ? precio + " " + moneda : "—");
        });
        colPrecio.setPrefWidth(105);

        TableColumn<NotificacionVenta, String> colEstado = new TableColumn<>("Estado");
        colEstado.setCellValueFactory(c -> new SimpleStringProperty(valorSeguro(c.getValue().estado())));
        colEstado.setPrefWidth(98);

        TableColumn<NotificacionVenta, String> colFecha = new TableColumn<>("Fecha / Hora");
        colFecha.setCellValueFactory(c -> new SimpleStringProperty(valorSeguro(c.getValue().fechaHora())));
        colFecha.setPrefWidth(165);

        TableColumn<NotificacionVenta, String> colEventoId = new TableColumn<>("Evento ID");
        colEventoId.setCellValueFactory(c -> new SimpleStringProperty(valorSeguro(c.getValue().eventoId())));
        colEventoId.setPrefWidth(185);

        TableColumn<NotificacionVenta, String> colMensaje = new TableColumn<>("Mensaje");
        colMensaje.setCellValueFactory(c -> new SimpleStringProperty(valorSeguro(c.getValue().mensaje())));
        colMensaje.setPrefWidth(220);

        tablaNotificaciones.getColumns().addAll(
                colVentaId, colVehiculoId, colCompradorId, colTipo,
                colPrecio, colEstado, colFecha, colEventoId, colMensaje
        );
        tablaNotificaciones.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY_FLEX_LAST_COLUMN);
        tablaNotificaciones.setPlaceholder(crearPlaceholder("Consultando notificaciones...", "#495057"));
    }

    private void consultarNotificaciones() {
        if (consultaEnCurso) {
            return;
        }
        String baseUrl = txtBaseUrl.getText().trim();
        if (baseUrl.isEmpty()) {
            mostrarErrorServicio("Configure la URL base del servicio (ej. http://127.0.0.1:8090).");
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
                        tablaNotificaciones.setItems(FXCollections.observableArrayList(lista));
                        lblResumen.setText("Total de notificaciones: " + lista.size());
                        actualizarBadge("CONECTADO (HTTP 200)", "#198754");

                        if (lista.isEmpty()) {
                            tablaNotificaciones.setPlaceholder(
                                    crearPlaceholder("Conectado — No hay notificaciones de venta registradas todavía.", "#198754"));
                            lblEstado.setTextFill(Color.DARKGREEN);
                            lblEstado.setText("Conectado (HTTP 200) · Sin notificaciones registradas aún · Última verificación: " + hora);
                        } else {
                            NotificacionVenta ultima = lista.get(0);
                            lblEstado.setTextFill(Color.DARKGREEN);
                            lblEstado.setText("Conectado (HTTP 200) · Última venta notificada: #" + ultima.ventaId()
                                    + " (Vehículo #" + ultima.vehiculoId() + ") · Actualizado: " + hora);
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
        actualizarBadge("SIN CONEXIÓN / ERROR", "#dc3545");
        lblEstado.setTextFill(Color.CRIMSON);
        lblEstado.setText(mensajeError);
        if (tablaNotificaciones.getItems().isEmpty()) {
            tablaNotificaciones.setPlaceholder(
                    crearPlaceholder("ERROR DE SERVICIO: No se pudo consultar /api/notificaciones.\nVerifique NGINX (:8090) y notificaciones-service (:8081).", "#dc3545"));
        }
    }

    private void actualizarBadge(String texto, String bgHex) {
        lblConexionBadge.setText(texto);
        lblConexionBadge.setStyle(
                "-fx-background-color: " + bgHex + ";" +
                "-fx-text-fill: white; -fx-font-weight: bold; -fx-background-radius: 12; -fx-font-size: 11.5px;");
    }

    private static Label crearPlaceholder(String texto, String colorHex) {
        Label lbl = new Label(texto);
        lbl.setStyle("-fx-text-fill: " + colorHex + "; -fx-font-size: 13px; -fx-font-weight: bold; -fx-text-alignment: center;");
        lbl.setWrapText(true);
        return lbl;
    }

    private static String valorSeguro(String s) {
        return (s == null || s.isBlank()) ? "—" : s;
    }
}
