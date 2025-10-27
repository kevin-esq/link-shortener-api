using System.ComponentModel.DataAnnotations;
using MediatR;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record SendVerifyEmailCodeCommand(
        string Email
    ) : IRequest;
}