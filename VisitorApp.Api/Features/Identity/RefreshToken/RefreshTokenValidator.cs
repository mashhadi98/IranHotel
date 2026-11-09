using FastEndpoints;
using FluentValidation;
using VisitorApp.Contract.Features.Identity.RefreshToken;

namespace VisitorApp.API.Features.Identity.RefreshToken;

public class RefreshTokenValidator : Validator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required")
            .MinimumLength(10)
            .WithMessage("Invalid refresh token format");

        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .WithMessage("Access token is required")
            .Must(BeValidJwtFormat)
            .WithMessage("Invalid access token format");
    }

    private bool BeValidJwtFormat(string token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        var parts = token.Split('.');
        return parts.Length == 3; // JWT should have 3 parts separated by dots
    }
} 