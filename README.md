## N5Challenge API

API desarrollada como parte del N5Challenge.

## Tecnologías Utilizadas

- .NET 8
- SQL Server
- Entity Framework Core
- Apache Kafka
- Elasticsearch

## Requisitos Previos

- Docker
- .NET 8 SDK (para desarrollo local)

## Ejecución del Proyecto

1. Clonar el repositorio
2. Navegar al directorio raíz del proyecto
3. Ejecutar: docker compose up --build -d
4. Una vez que los contenedores estén en ejecución, la API estará disponible en: [http://localhost:8080]

## Permitir Conexión con la aplicación cliente
Debe configurar la URL de la aplicación cliente en el archivo `appsettings.Development.json`.

Ej.:
```bash
"Cors": {
  "Origins": [ "http://localhost:5000" ]
}
```

## Logs de Errores 
- Logs de errores (pensado para el entorno de desarrollo): [http://localhost:8080/logs/]

## Documentación de la API (Swagger UI)
- Swagger UI (pensado para el entorno de desarrollo): [http://localhost:8080/swagger/index.html]

