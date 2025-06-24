using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QualaBackend.Configuration;
using QualaBackend.Interfaces;
using QualaApi.Models;
using QualaBackend.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QualaBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UsuarioRepository usuarioRepository,
            IOptions<JwtSettings> jwtSettings)

        {
            _usuarioRepository = usuarioRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var usuario = await ValidateUserAsync(request.Usuario, request.Password);

            if (usuario == null)
                return null;

            var token = GenerateJwtToken(usuario);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            return new LoginResponse
            {
                Token = token,
                Expiration = expiration,
                Usuario = usuario.NombreUsuario
            };
        }

        public string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Email, usuario.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<Usuario?> ValidateUserAsync(string username, string password)
        {
            var usuario = await _usuarioRepository.GetByUsernameAsync(username);

            if (usuario == null || !usuario.Activo)
                return null;

            if (usuario.PasswordHash != password)
                return null;

            return usuario;
        }
    }

    public class ValidationService : IValidationService
    {
        private readonly ISucursalRepository _sucursalRepository;
        private readonly IMonedaRepository _monedaRepository;

        public ValidationService(ISucursalRepository sucursalRepository, IMonedaRepository monedaRepository)
        {
            _sucursalRepository = sucursalRepository;
            _monedaRepository = monedaRepository;
        }

        public async Task<bool> ValidarFechaCreacionAsync(DateTime fecha)
        {
            // La fecha de creación no puede ser anterior a la fecha actual
            return fecha.Date >= DateTime.Today;
        }

        public async Task<bool> ValidarCodigoUnicoAsync(int codigo, int? excludeId = null)
        {
            return !await _sucursalRepository.ExistsCodigoAsync(codigo, excludeId);
        }

        public async Task<bool> ValidarMonedaExisteAsync(int monedaId)
        {
            var moneda = await _monedaRepository.GetByIdAsync(monedaId);
            return moneda != null;
        }
    }

    // Servicio para manejo de errores y respuestas
    public static class ResponseService
    {
        public static ApiResponse<T> Success<T>(T data, string message = "Operación exitosa")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Error<T>(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        public static ApiResponse<T> ValidationError<T>(List<string> errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Error de validación",
                Errors = errors
            };
        }
    }
}