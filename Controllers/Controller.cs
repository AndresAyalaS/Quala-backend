using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QualaBackend.Interfaces;
using QualaApi.Models;
using QualaBackend.Services;
using Microsoft.Data.SqlClient; 

namespace QualaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ResponseService.ValidationError<LoginResponse>(errors));
                }

                var result = await _authService.LoginAsync(request);
                
                if (result == null)
                {
                    return Unauthorized(ResponseService.Error<LoginResponse>("Usuario o contraseña incorrectos"));
                }

                return Ok(ResponseService.Success(result, "Login exitoso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseService.Error<LoginResponse>($"Error interno del servidor: {ex.Message}"));
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SucursalesController : ControllerBase
    {
        private readonly ISucursalRepository _sucursalRepository;
        private readonly IValidationService _validationService;

        public SucursalesController(ISucursalRepository sucursalRepository, IValidationService validationService)
        {
            _sucursalRepository = sucursalRepository;
            _validationService = validationService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Sucursal>>>> GetAll()
        {
            try
            {
                var sucursales = await _sucursalRepository.GetAllAsync();
                return Ok(ResponseService.Success(sucursales, "Sucursales obtenidas exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseService.Error<IEnumerable<Sucursal>>($"Error al obtener sucursales: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Sucursal>>> GetById(int id)
        {
            try
            {
                var sucursal = await _sucursalRepository.GetByIdAsync(id);
                
                if (sucursal == null)
                {
                    return NotFound(ResponseService.Error<Sucursal>("Sucursal no encontrada"));
                }

                return Ok(ResponseService.Success(sucursal, "Sucursal obtenida exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseService.Error<Sucursal>($"Error al obtener sucursal: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Sucursal>>> Create([FromBody] SucursalCreateDto sucursalDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ResponseService.ValidationError<Sucursal>(errors));
                }

                // Validaciones adicionales
                var validationErrors = new List<string>();

                if (!await _validationService.ValidarFechaCreacionAsync(sucursalDto.FechaCreacion))
                {
                    validationErrors.Add("La fecha de creación no puede ser anterior a la fecha actual");
                }

                if (!await _validationService.ValidarCodigoUnicoAsync(sucursalDto.Codigo))
                {
                    validationErrors.Add("Ya existe una sucursal con el código especificado");
                }

                if (!await _validationService.ValidarMonedaExisteAsync(sucursalDto.MonedaId))
                {
                    validationErrors.Add("La moneda especificada no existe");
                }

                if (validationErrors.Any())
                {
                    return BadRequest(ResponseService.ValidationError<Sucursal>(validationErrors));
                }

                var sucursal = await _sucursalRepository.CreateAsync(sucursalDto);
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = sucursal.Id }, 
                    ResponseService.Success(sucursal, "Sucursal creada exitosamente")
                );
            }
            catch (SqlException ex)
            {
                return BadRequest(ResponseService.Error<Sucursal>($"Error de base de datos: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseService.Error<Sucursal>($"Error al crear sucursal: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Sucursal>>> Update(int id, [FromBody] SucursalUpdateDto sucursalDto)
        {
            try
            {
                if (id != sucursalDto.Id)
                {
                    return BadRequest(ResponseService.Error<Sucursal>("El ID de la URL no coincide con el ID del objeto"));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ResponseService.ValidationError<Sucursal>(errors));
                }

                // Verificar que la sucursal existe
                var existingSucursal = await _sucursalRepository.GetByIdAsync(id);
                if (existingSucursal == null)
                {
                    return NotFound(ResponseService.Error<Sucursal>("Sucursal no encontrada"));
                }

                // Validaciones adicionales
                var validationErrors = new List<string>();

                if (!await _validationService.ValidarCodigoUnicoAsync(sucursalDto.Codigo, id))
                {
                    validationErrors.Add("Ya existe otra sucursal con el código especificado");
                }

                if (!await _validationService.ValidarMonedaExisteAsync(sucursalDto.MonedaId))
                {
                    validationErrors.Add("La moneda especificada no existe");
                }

                if (validationErrors.Any())
                {
                    return BadRequest(ResponseService.ValidationError<Sucursal>(validationErrors));
                }

                var sucursal = await _sucursalRepository.UpdateAsync(sucursalDto);
                return Ok(ResponseService.Success(sucursal, "Sucursal actualizada exitosamente"));
            }
            catch (SqlException ex)
            {
                return BadRequest(ResponseService.Error<Sucursal>($"Error de base de datos: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseService.Error<Sucursal>($"Error al actualizar sucursal: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var sucursal = await _sucursalRepository.GetByIdAsync(id);
                if (sucursal == null)
                {
                    return NotFound(ResponseService.Error<bool>("Sucursal no encontrada"));
                }

                var result = await _sucursalRepository.DeleteAsync(id);
                
                if (result)
                {
                    return Ok(ResponseService.Success(true, "Sucursal eliminada exitosamente"));
                }
                else
                {
                    return BadRequest(ResponseService.Error<bool>("No se pudo eliminar la sucursal"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseService.Error<bool>($"Error al eliminar sucursal: {ex.Message}"));
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MonedasController : ControllerBase
    {
        private readonly IMonedaRepository _monedaRepository;

        public MonedasController(IMonedaRepository monedaRepository)
        {
            _monedaRepository = monedaRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Moneda>>>> GetAll()
        {
            try
            {
                var monedas = await _monedaRepository.GetAllAsync();
                return Ok(ResponseService.Success(monedas, "Monedas obtenidas exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseService.Error<IEnumerable<Moneda>>($"Error al obtener monedas: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Moneda>>> GetById(int id)
        {
            try
            {
                var moneda = await _monedaRepository.GetByIdAsync(id);
                
                if (moneda == null)
                {
                    return NotFound(ResponseService.Error<Moneda>("Moneda no encontrada"));
                }

                return Ok(ResponseService.Success(moneda, "Moneda obtenida exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseService.Error<Moneda>($"Error al obtener moneda: {ex.Message}"));
            }
        }
    }
}