/*package ec.edu.com;

import javafx.application.Application;
import javafx.application.Platform;
import javafx.beans.property.ReadOnlyObjectWrapper;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.PasswordField;
import javafx.scene.control.TableView;
import javafx.scene.layout.HBox;
import javafx.scene.layout.Priority;
import javafx.scene.layout.VBox;
import javafx.scene.paint.Color;
import javafx.stage.Stage;

// Ventana equivale a FormPrincipal de Windows Forms.
public final class VehiculosApp extends Application {

    private final VehiculosApiClient api = new VehiculosApiClient();
    private final PasswordField txtToken = new PasswordField();
    private final Button btnConsultar = new Button("Consultar");
    private final Label lblEstado = new Label("Listo");
    private final Label lblTotal = new Label("Total de Vehiculos: 0");
    private final TableView<Vehiculo> tabla = new TableView<>();

    @Override
    public void start(Stage ventana) {
        Label titulo = new Label("Ventana Vehiculos Registrados");
        txtToken.setPromptText("AKI VA EL JWT");
        txtToken.setMaxWidth(Double.MAX_VALUE);
        btnConsultar.setMaxWidth(130);
        btnConsultar.setOnAction(evento -> consultar());
        HBox acciones = new HBox(14, txtToken, btnConsultar);
        acciones.setAlignment(Pos.CENTER_LEFT);
        lblEstado.setWrapText(true);

        //grid
        agregarColumna("ID", Vehiculo::idVehiculo, 65);
        agregarColumna("Marca", Vehiculo::marca, 120);
        agregarColumna("Modelo", Vehiculo::modelo, 120);
        agregarColumna("Chasis", Vehiculo::chasis, 150);
        agregarColumna("Año", Vehiculo::anio, 80);
        agregarColumna("Placa", Vehiculo::placa, 100);
        agregarColumna("Color", Vehiculo::color, 100);
        agregarColumna("Precio", Vehiculo::precio, 100);
        agregarColumna("Estado", Vehiculo::estado, 100);

        tabla.setPlaceholder(new Label("No hay vehiculos para mostrar"));



        VBox.setVgrow(tabla, Priority.ALWAYS);
        VBox contenido = new VBox(12, titulo, new Label("Token JWT:"), txtToken, acciones, lblEstado, tabla, lblTotal);
        contenido.setPadding(new Insets(22));
        contenido.setStyle("-fx-background-color: red");
        ventana.setTitle("Consulta Vehiculos");
        ventana.setScene(new Scene(contenido, 800, 600));
        ventana.show();
    }

    private void consultar(){
        String jwt = txtToken.getText().trim();
        if (jwt.regionMatches(true, 0, "Bearer",0,7)) {
            jwt = jwt.substring(7).trim();
        }

        btnConsultar.setDisable(true);

        mostrarEstado("Consultando vehiculos... api java", false);

        api.consultarVehiculo(jwt).whenComplete((resultado, error) ->
            Platform.runLater(() ->
        {
            btnConsultar.setDisable(false);
            if (error != null) {
                mostrarEstado("No se encontro java: " + error.getMessage(), true);
                return;
            }
            if (resultado.codigoHttp() == 401) {
                mostrarEstado("Error en la consulta: HTTP " + resultado.codigoHttp(), true);
                
            } else if (resultado.codigoHttp() == 403) {
                mostrarEstado("HTTP 403: Acceso denegado " + resultado.codigoHttp(), true);
            } else{
                
                tabla.getItems().setAll(resultado.vehiculos());
                lblTotal.setText("Total de Vehiculos: " + resultado.vehiculos().size());
                mostrarEstado("Consulta exitosa: HTTP " + resultado.codigoHttp(), false);
            }
        }));
    }

    private void mostrarEstado(String texto, boolean error){
        lblEstado.setText(texto);
        lblEstado.setTextFill(error? Color.RED : Color.BLUE);
    }
    private <T> void agregarColumna(String titulo, Function<Vehiculo, T> valor, double ancho) {
        TableColumn<Vehiculo, T> columna = new TableColumn<>(titulo);
        columna.setCellValueFactory(fila -> new ReadOnlyObjectWrapper<>(valor.apply(fila.getValue())));
        tabla.getColumns().add(columna);
        columna.setPrefWidth(ancho);
    }
}*/




package ec.edu.com;

import javafx.application.Application;
import javafx.application.Platform;
import javafx.beans.property.SimpleIntegerProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.collections.FXCollections;
import javafx.geometry.Insets;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.control.TextField;
import javafx.scene.layout.HBox;
import javafx.scene.layout.Priority;
import javafx.scene.layout.VBox;
import javafx.stage.Stage;

import java.math.BigDecimal;


public final class VehiculosApp extends Application {


    private final TextField txtToken = new TextField();
    private final Button btnConsultar = new Button("Listar Vehículos");
    private final TableView<Vehiculo> tablaVehiculos = new TableView<>();
    private final Label lblEstado = new Label("Estado: Ingrese su token JWT para comenzar.");

    public static void main(String[] args) {
        launch(args);
    }

    @Override
    public void start(Stage primaryStage) {
        // Texto de ayuda en el campo del token
        txtToken.setPromptText("Pegue su Token JWT aquí...");
        HBox.setHgrow(txtToken, Priority.ALWAYS); 

        // Inicializamos las columnas de la tabla
        configurarTabla();

        // Asignamos la acción al hacer clic en el botón
        btnConsultar.setOnAction(event -> consultarVehiculos());
        txtToken.setOnAction(event -> consultarVehiculos());

        // Contenedor superior (Fila con etiqueta, caja de texto y botón)
        HBox filaSuperior = new HBox(10, new Label("Token JWT:"), txtToken, btnConsultar);
        filaSuperior.setPadding(new Insets(5));

        // Contenedor principal que organiza todos los elementos en vertical
        VBox root = new VBox(10, filaSuperior, tablaVehiculos, lblEstado);
        root.setPadding(new Insets(15));
        VBox.setVgrow(tablaVehiculos, Priority.ALWAYS); // La tabla se expande verticalmente

        // Configuración y muestra de la ventana
        primaryStage.setTitle("Consulta de Vehículos - SOA");
        primaryStage.setScene(new Scene(root, 780, 480));
        primaryStage.show();
    }

    /**
     * Configura cada una de las columnas de la tabla vinculándolas/
     **/
    @SuppressWarnings("unchecked")
    private void configurarTabla() {
        // Columna ID
        TableColumn<Vehiculo, Integer> colId = new TableColumn<>("ID");
        colId.setCellValueFactory(c -> new SimpleIntegerProperty(c.getValue().idVehiculo()).asObject());
        colId.setPrefWidth(60);

        // Columna Marca
        TableColumn<Vehiculo, String> colMarca = new TableColumn<>("Marca");
        colMarca.setCellValueFactory(c -> new SimpleStringProperty(c.getValue().marca()));
        colMarca.setPrefWidth(100);

        // Columna Modelo
        TableColumn<Vehiculo, String> colModelo = new TableColumn<>("Modelo");
        colModelo.setCellValueFactory(c -> new SimpleStringProperty(c.getValue().modelo()));
        colModelo.setPrefWidth(110);

        // Columna Año
        TableColumn<Vehiculo, Integer> colAnio = new TableColumn<>("Año");
        colAnio.setCellValueFactory(c -> new SimpleIntegerProperty(c.getValue().anio()).asObject());
        colAnio.setPrefWidth(65);

        // Columna Placa
        TableColumn<Vehiculo, String> colPlaca = new TableColumn<>("Placa");
        colPlaca.setCellValueFactory(c -> new SimpleStringProperty(c.getValue().placa()));
        colPlaca.setPrefWidth(90);

        // Columna Color
        TableColumn<Vehiculo, String> colColor = new TableColumn<>("Color");
        colColor.setCellValueFactory(c -> new SimpleStringProperty(c.getValue().color()));
        colColor.setPrefWidth(85);

        // Columna Precio
        TableColumn<Vehiculo, BigDecimal> colPrecio = new TableColumn<>("Precio ($)");
        colPrecio.setCellValueFactory(c -> new SimpleObjectProperty<>(c.getValue().precio()));
        colPrecio.setPrefWidth(95);

        // Columna Estado
        TableColumn<Vehiculo, String> colEstado = new TableColumn<>("Estado");
        colEstado.setCellValueFactory(c -> new SimpleStringProperty(c.getValue().estado()));
        colEstado.setPrefWidth(90);

        // Agregamos todas las columnas a la tabla
        tablaVehiculos.getColumns().addAll(colId, colMarca, colModelo, colAnio, colPlaca, colColor, colPrecio, colEstado);
        tablaVehiculos.setPlaceholder(new Label("No hay vehículos para mostrar"));
    }

    /**
     * Valida el token ingresado y realiza la petición a la API.
     **/
    private void consultarVehiculos() {
        String token = txtToken.getText().trim();

        // 1. VALIDACIÓN DEL TOKEN: Verificar que no esté vacío
        if (token.isEmpty()) {
            mostrarAlerta(Alert.AlertType.WARNING, "Token Requerido", "Por favor ingrese o pegue el token JWT antes de consultar.");
            lblEstado.setText("Estado: Ingrese un token para continuar.");
            return;
        }

        // Estado visual de carga y deshabilitar botón para evitar múltiples peticiones
        btnConsultar.setDisable(true);
        lblEstado.setText("Estado: Consultando vehículos a la API...");
        tablaVehiculos.getItems().clear();

        // 2. LLAMADA ASÍNCRONA A LA API
        VehiculosApiClient.consultarVehiculos(token)
            .thenAccept(respuesta -> Platform.runLater(() -> {
                btnConsultar.setDisable(false);
                int codigo = respuesta.codigoHttp();

                if (codigo == 200) {
                    // Cargar la lista de vehículos obtenida en la tabla
                    tablaVehiculos.setItems(FXCollections.observableArrayList(respuesta.vehiculos()));
                    lblEstado.setText("Estado: 200 OK | Total de vehículos cargados: " + respuesta.vehiculos().size());
                } else if (codigo == 401) {
                    // Token rechazado o expirado
                    lblEstado.setText("Estado: Error 401 | Token inválido o expirado.");
                    mostrarAlerta(Alert.AlertType.ERROR, "Error de Autenticación", "El token ingresado no es válido o ya expiró (401 Unauthorized).");
                } else if (codigo == 403) {
                    // Token válido pero sin rol para consultar
                    lblEstado.setText("Estado: Error 403 | Permisos insuficientes.");
                    mostrarAlerta(Alert.AlertType.ERROR, "Acceso Denegado", "El token no cuenta con los permisos necesarios para consultar vehículos (403 Forbidden).");
                } else {
                    lblEstado.setText("Estado: Error en la respuesta HTTP: " + codigo);
                }
            }))
            .exceptionally(error -> {
                // Manejo de errores de conexión (ej. servidor caído o URL inaccesible)
                Platform.runLater(() -> {
                    btnConsultar.setDisable(false);
                    lblEstado.setText("Estado: Error al conectar con la API.");
                    mostrarAlerta(Alert.AlertType.ERROR, "Error de Conexión", "No se pudo comunicar con el servidor backend:\n" + error.getMessage());
                });
                return null;
            });
    }

    /**
     * Método auxiliar para desplegar ventanas de alerta (mensajes informativos o de error).
     */
    private void mostrarAlerta(Alert.AlertType tipo, String titulo, String mensaje) {
        Alert alerta = new Alert(tipo);
        alerta.setTitle(titulo);
        alerta.setHeaderText(null);
        alerta.setContentText(mensaje);
        alerta.showAndWait();
    }
}
