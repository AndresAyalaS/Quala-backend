using QualaApi.Models;

namespace QualaBackend.Interfaces
{
    // Interface para el repositorio de sucursales
    public interface ISucursalRepository
    {
        Task<IEnumerable<Sucursal>> GetAllAsync();
        Task<Sucursal?> GetByIdAsync(int id);
        Task<Sucursal> CreateAsync(SucursalCreateDto sucursal);
        Task<Sucursal> UpdateAsync(SucursalUpdateDto sucursal);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsCodigoAsync(int codigo, int? excludeId = null);
    }

    // Interface para el repositorio de monedas
    public interface IMonedaRepository
    {
        Task<IEnumerable<Moneda>> GetAllAsync();
        Task<Moneda?> GetByIdAsync(int id);
    }

    // Interface para el servicio de autenticación
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        string GenerateJwtToken(Usuario usuario);
        Task<Usuario?> ValidateUserAsync(string username, string password);
    }

    // Interface para validaciones personalizadas
    public interface IValidationService
    {
        Task<bool> ValidarFechaCreacionAsync(DateTime fecha);
        Task<bool> ValidarCodigoUnicoAsync(int codigo, int? excludeId = null);
        Task<bool> ValidarMonedaExisteAsync(int monedaId);
    }
}

namespace QualaBackend.Configuration
{
    // Configuración JWT
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationInMinutes { get; set; }
    }

    // Configuración de la base de datos
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int CommandTimeout { get; set; } = 30;
    }

    // Configuración CORS
    public static class CorsSettings
    {
        public const string PolicyName = "AllowAngularApp";
        
        public static void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });
        }
    }
}