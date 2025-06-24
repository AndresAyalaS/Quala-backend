using FluentValidation;
using QualaApi.Models;

namespace QualaBackend.Validators
{
    public class SucursalCreateDtoValidator : AbstractValidator<SucursalCreateDto>
    {
        public SucursalCreateDtoValidator()
        {
            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("El código es requerido")
                .GreaterThan(0).WithMessage("El código debe ser mayor a 0");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(250).WithMessage("La descripción no puede exceder 250 caracteres");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección es requerida")
                .MaximumLength(250).WithMessage("La dirección no puede exceder 250 caracteres");

            RuleFor(x => x.Identificacion)
                .NotEmpty().WithMessage("La identificación es requerida")
                .MaximumLength(50).WithMessage("La identificación no puede exceder 50 caracteres");

            RuleFor(x => x.FechaCreacion)
                .NotEmpty().WithMessage("La fecha de creación es requerida")
                .Must(BeAValidDate).WithMessage("La fecha de creación no puede ser anterior a la fecha actual");

            RuleFor(x => x.MonedaId)
                .NotEmpty().WithMessage("La moneda es requerida")
                .GreaterThan(0).WithMessage("Debe seleccionar una moneda válida");
        }

        private bool BeAValidDate(DateTime date)
        {
            return date.Date >= DateTime.Today;
        }
    }

    public class SucursalUpdateDtoValidator : AbstractValidator<SucursalUpdateDto>
    {
        public SucursalUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID es requerido")
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("El código es requerido")
                .GreaterThan(0).WithMessage("El código debe ser mayor a 0");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(250).WithMessage("La descripción no puede exceder 250 caracteres");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección es requerida")
                .MaximumLength(250).WithMessage("La dirección no puede exceder 250 caracteres");

            RuleFor(x => x.Identificacion)
                .NotEmpty().WithMessage("La identificación es requerida")
                .MaximumLength(50).WithMessage("La identificación no puede exceder 50 caracteres");

            RuleFor(x => x.MonedaId)
                .NotEmpty().WithMessage("La moneda es requerida")
                .GreaterThan(0).WithMessage("Debe seleccionar una moneda válida");
        }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Usuario)
                .NotEmpty().WithMessage("El usuario es requerido")
                .MaximumLength(50).WithMessage("El usuario no puede exceder 50 caracteres");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");
        }
    }
}