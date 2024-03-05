using FluentValidation;

namespace StorageService.Application.Configuration;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";
    public string Host { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public sealed class RabbitMqOptionsValidator : AbstractValidator<RabbitMqOptions>
{
    public RabbitMqOptionsValidator()
    {
        RuleFor(x => x.Host).NotEmpty();
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}