using FluentValidation;

namespace OpenMatchFunction.Services;

public sealed class RunRequestValidator : AbstractValidator<RunRequest>
{
    public RunRequestValidator()
    {
        RuleFor(r => r.Profile.Name)
            .NotEmpty();

        RuleFor(r => r.Profile.Pools)
            .NotEmpty();
    }
}