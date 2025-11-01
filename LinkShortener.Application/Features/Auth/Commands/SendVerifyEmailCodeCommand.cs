using System.ComponentModel.DataAnnotations;
using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record SendVerifyEmailCodeCommand(
        string Email
    ) : ICommand;
}