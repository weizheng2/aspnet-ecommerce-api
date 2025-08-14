using ECommerceApi.DTOs;
using ECommerceApi.Models;
using ECommerceApi.Utils;

namespace ECommerceApi.Services
{
    public interface IUserService
    {
        Task<User?> GetUserById(string userId);
        Task<User?> GetUser();
        Task<Result<AuthenticationResponseDto>> Register(UserCredentialsDto credentialsDto);
        Task<Result<AuthenticationResponseDto>> Login(UserCredentialsDto credentialsDto);
        Task<Result<AuthenticationResponseDto>> RefreshToken();
        Task<Result> MakeAdmin(EditClaimDto editClaimDto);
    }
}