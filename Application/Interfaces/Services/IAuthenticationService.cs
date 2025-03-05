using Application.DTOs.Authentication;
using Application.DTOs.Authentication.Session;

namespace Application.Interfaces.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResponseDTO> AuthenticateOldMobile(AuthenticationRequestDTO authenticationRequest);
    Task<AuthenticationResponseDTO> Authenticate(AuthenticationRequestDTO authenticationRequest);
    Task<AuthenticationResponseDTO> AuthenticateNew(AuthenticationRequestDTO authenticationRequest);
    Task<AuthenticationResponseDTO> AuthenticateForStudent(AuthenticationRequestDTO authenticationRequest);
}