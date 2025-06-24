using System.ComponentModel.DataAnnotations;

namespace QualaApi.Models
 {
    // Entidad Sucursal
    public class Sucursal
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Identificacion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public int MonedaId { get; set; }
        public string? MonedaNombre { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaModificacion { get; set; }
    }

    // Entidad Moneda
    public class Moneda
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Simbolo { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }

    // DTOs para Requests
    public class SucursalCreateDto
    {
        [Required(ErrorMessage = "El código es requerido")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(250, ErrorMessage = "La descripción no puede exceder 250 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(250, ErrorMessage = "La dirección no puede exceder 250 caracteres")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La identificación es requerida")]
        [StringLength(50, ErrorMessage = "La identificación no puede exceder 50 caracteres")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de creación es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }

        [Required(ErrorMessage = "La moneda es requerida")]
        public int MonedaId { get; set; }
    }

    public class SucursalUpdateDto
    {
        [Required(ErrorMessage = "El ID es requerido")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es requerido")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(250, ErrorMessage = "La descripción no puede exceder 250 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(250, ErrorMessage = "La dirección no puede exceder 250 caracteres")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La identificación es requerida")]
        [StringLength(50, ErrorMessage = "La identificación no puede exceder 50 caracteres")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La moneda es requerida")]
        public int MonedaId { get; set; }
    }

    // DTOs para Responses
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }

    // Modelos para autenticación
    public class LoginRequest
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string Usuario { get; set; } = string.Empty;
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
    }

 }