
# QUALA Sucursales API

API RESTful desarrollada en .NET 6 para la gestión de sucursales y monedas de la empresa QUALA. El proyecto está construido con una arquitectura limpia, autenticación JWT, validaciones con FluentValidation y uso de procedimientos almacenados mediante Dapper.

---

## 📚 Tecnologías utilizadas

- [.NET 6](https://dotnet.microsoft.com/en-us/)
- C#
- ASP.NET Core Web API
- Dapper
- SQL Server
- FluentValidation
- JWT (Json Web Tokens)
- Swagger / OpenAPI
- CORS configurado para Angular

---

## 📁 Estructura del Proyecto

```
QualaBackend/
├── Controllers/
│   ├── AuthController.cs
│   ├── SucursalesController.cs
│   └── MonedasController.cs
├── Interfaces/
│   └── ISucursalRepository.cs
│   └── IMonedaRepository.cs
│   └── IAuthService.cs
├── Models/  (o referenciados desde QualaApi.Models)
├── Repositories/
│   └── SucursalRepository.cs
│   └── MonedaRepository.cs
│   └── UsuarioRepository.cs
├── Services/
│   └── AuthService.cs
│   └── ValidationService.cs
│   └── ResponseService.cs
├── Validators/
│   └── SucursalCreateDtoValidator.cs
│   └── SucursalUpdateDtoValidator.cs
│   └── LoginRequestValidator.cs
├── Configuration/
│   └── JwtSettings.cs
│   └── CorsSettings.cs
├── Program.cs
├── appsettings.json
```

---

## ⚙️ Configuración del entorno

### 1. Requisitos previos

- .NET 6 SDK o superior
- SQL Server
- Visual Studio / VS Code
- Postman (para pruebas)
- [Angular CLI](https://angular.io/cli) (si vas a integrar el frontend)

---

### 2. Configura `appsettings.json`

```json
"JwtSettings": {
  "Key": "clave-secreta-segura",
  "Issuer": "QualaApi",
  "Audience": "QualaFrontend",
  "ExpirationInMinutes": 60
},
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=QualaDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

---

### 3. Scripts y procedimientos almacenados

Este backend utiliza **procedimientos almacenados** en SQL Server, por ejemplo:

- `aa_sp_sucursal_obtener_todas`
- `aa_sp_sucursal_crear`
- `aa_sp_sucursal_actualizar`
- `aa_sp_sucursal_eliminar`
- `aa_sp_usuario_validar`
- `aa_sp_moneda_obtener_todas`

Asegúrate de tenerlos creados en tu base de datos con la estructura adecuada.

---

## 🔐 Autenticación

La autenticación se realiza mediante JWT. Al hacer login en `/api/auth/login`, se recibe un token que debe ser enviado en el header:

```
Authorization: Bearer <tu_token>
```

---

## 🚀 Ejecución del proyecto

1. Clona el repositorio:
```bash
git clone https://github.com/AndresAyalaS/quala-backend.git
cd quala-backend
```

2. Restaura los paquetes:
```bash
dotnet restore
```

3. Compila y ejecuta:
```bash
dotnet run
```

4. Accede a Swagger:
```
http://localhost:5062/index.html
```

---

## ✅ Funcionalidades

- [x] Login con usuario y contraseña
- [x] Autenticación JWT
- [x] CRUD de Sucursales
- [x] Listado de Monedas
- [x] Validaciones con FluentValidation
- [x] Validaciones personalizadas (fecha, moneda existente, código único)
- [x] Swagger documentado
- [x] CORS para Angular

---

## 📬 Endpoints principales

### Auth
- `POST /api/auth/login`

### Sucursales
- `GET /api/sucursales`
- `GET /api/sucursales/{id}`
- `POST /api/sucursales`
- `PUT /api/sucursales/{id}`
- `DELETE /api/sucursales/{id}`

### Monedas
- `GET /api/monedas`
- `GET /api/monedas/{id}`

---


## 🧑‍💻 Autor

**Andrés Ayala Sánchez**  
Desarrollador Full Stack .NET & Angular  
[LinkedIn](https://www.linkedin.com/in/andres-ayala-sanchez/) 


---
