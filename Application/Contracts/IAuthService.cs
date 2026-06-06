using DTO.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> LoginAsync(LoginModel model);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<string> GetCurrentUserIdAsync();
        Task SignOutAsync();
    }
}