using Application.DTOs.User;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<int> GetUserId(UserRequestDTO userRequest);   

    Task<bool> IsUserAuthenticated(UserRequestDTO userRequest);
    
    Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
}