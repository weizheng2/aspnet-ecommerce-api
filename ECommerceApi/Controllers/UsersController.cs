using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceApi.DTOs;
using ECommerceApi.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using ECommerceApi.Utils;

namespace ECommerceApi.Controllers
{
    [ApiVersion("1.0")]
    [ApiController, Route("api/v{version:apiVersion}/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UsersController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [EnableRateLimiting("general")]
        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResponseDto>> Register(UserCredentialsDto credentialsDto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _userServices.Register(credentialsDto);
            if (result.IsSuccess)
                return result.Data;

            switch (result.ErrorType)
            {
                case ResultErrorType.ValidationError: return ValidationProblem(result.ErrorMessage);
                default: return BadRequest(result.ErrorMessage);
            }
        }

        [EnableRateLimiting("strict")]
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponseDto>> Login(UserCredentialsDto credentialsDto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _userServices.Login(credentialsDto);
            if (result.IsSuccess)
                return result.Data;
            
            switch (result.ErrorType)
            {
                case ResultErrorType.ValidationError: return ValidationProblem(result.ErrorMessage);
                default: return BadRequest(result.ErrorMessage);
            }
        }

        [EnableRateLimiting("general")]
        [HttpGet("update-token")]
        [Authorize]
        public async Task<ActionResult<AuthenticationResponseDto>> UpdateToken()
        {
            var result = await _userServices.UpdateToken();
            if (result.IsSuccess)
                return result.Data;

            switch (result.ErrorType)
            {
                case ResultErrorType.NotFound: return NotFound();
                default: return BadRequest(result.ErrorMessage);
            }
        }

        [HttpPost("make-admin")]
        [Authorize(Policy = "isAdmin")]
        public async Task<ActionResult> MakeAdmin(EditClaimDto editClaimDto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _userServices.MakeAdmin(editClaimDto);
            if (result.IsSuccess)
                return NoContent();

            switch (result.ErrorType)
            {
                case ResultErrorType.NotFound: return NotFound();
                default: return BadRequest(result.ErrorMessage);
            }
        }

    }
}