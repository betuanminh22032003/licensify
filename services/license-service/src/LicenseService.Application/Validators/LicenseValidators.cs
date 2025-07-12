using FluentValidation;
using LicenseService.Application.Commands.Licenses;

namespace LicenseService.Application.Validators;

public class CreateLicenseCommandValidator : AbstractValidator<CreateLicenseCommand>
{
    public CreateLicenseCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId is required");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid license type");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiration date must be in the future");

        RuleFor(x => x.MaxUsers)
            .GreaterThan(0)
            .WithMessage("MaxUsers must be greater than zero");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters");
    }
}

public class ActivateLicenseCommandValidator : AbstractValidator<ActivateLicenseCommand>
{
    public ActivateLicenseCommandValidator()
    {
        RuleFor(x => x.LicenseId)
            .NotEmpty()
            .WithMessage("LicenseId is required");
    }
}

public class SuspendLicenseCommandValidator : AbstractValidator<SuspendLicenseCommand>
{
    public SuspendLicenseCommandValidator()
    {
        RuleFor(x => x.LicenseId)
            .NotEmpty()
            .WithMessage("LicenseId is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}

public class RevokeLicenseCommandValidator : AbstractValidator<RevokeLicenseCommand>
{
    public RevokeLicenseCommandValidator()
    {
        RuleFor(x => x.LicenseId)
            .NotEmpty()
            .WithMessage("LicenseId is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}

public class ExtendLicenseCommandValidator : AbstractValidator<ExtendLicenseCommand>
{
    public ExtendLicenseCommandValidator()
    {
        RuleFor(x => x.LicenseId)
            .NotEmpty()
            .WithMessage("LicenseId is required");

        RuleFor(x => x.NewExpirationDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("New expiration date must be in the future");
    }
}

public class ValidateLicenseCommandValidator : AbstractValidator<ValidateLicenseCommand>
{
    public ValidateLicenseCommandValidator()
    {
        RuleFor(x => x.LicenseKey)
            .NotEmpty()
            .WithMessage("License key is required")
            .Length(20, 100)
            .WithMessage("License key must be between 20 and 100 characters");
    }
}
