using Application.Contracts;
using Application.Helper;
using ContextLayer;
using Domain.Models;
using DTO.DTO.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Context _context;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, Context context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already registered!" };

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                    errors += $"{error.Description},";
                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "Learner");

            var jwtSecurityToken = await CreateJwtToken(user);
            var refreshToken = CreateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                Username = user.UserName,
                Roles = new List<string> { user.Role },
                RefreshToken = refreshToken.Token,
                ExpiresOn = jwtSecurityToken.ValidTo,
                RefreshTokenExpiration = refreshToken.Expires
            };
        }

        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new AuthModel { Message = "Email or Password is not correct." };

            var jwtSecurityToken = await CreateJwtToken(user);

            var refreshToken = CreateRefreshToken();
            var roles = await _userManager.GetRolesAsync(user);

            var activeRefreshTokens = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            string refreshTokenValue;
            DateTime refreshTokenExpiration;

            if (activeRefreshTokens != null)
            {
                refreshTokenValue = activeRefreshTokens.Token;
                refreshTokenExpiration = activeRefreshTokens.Expires;
            }
            else
            {
                var newRefreshToken = CreateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);
                refreshTokenValue = newRefreshToken.Token;
                refreshTokenExpiration = newRefreshToken.Expires;
            }

            return new AuthModel
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                Username = user.UserName,
                Roles = roles.ToList(),
                RefreshToken = refreshTokenValue, 
                ExpiresOn = jwtSecurityToken.ValidTo,
                RefreshTokenExpiration = refreshTokenExpiration 
            };
        }

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                authModel.Message = "Invalid refresh token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == token);

            if (refreshToken == null || !refreshToken.IsActive)    
            {
                authModel.Message = "Inactive regresh token";
                return authModel;
            }

            refreshToken.Revoked = DateTime.UtcNow;

            var newRefreshToken = CreateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);

            await _userManager.UpdateAsync(user);

            var jwtSecurityToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.RefreshTokenExpiration = newRefreshToken.Expires;

            return authModel;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            //var user = _context.Users.Include(u => u.RefreshTokens).SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            var user = await _userManager.Users.Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null) return false;

            var refreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == token);

            if (refreshToken == null || !refreshToken.IsActive)
                return false;

            refreshToken.Revoked = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid",user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            }.Union(userClaims).Union(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecritKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes > 0 ? _jwt.DurationInMinutes : 60),
                signingCredentials: creds
            );

            return token;
        }

        private RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(10), 
                Created = DateTime.UtcNow
            };
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            return await Task.FromResult(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}