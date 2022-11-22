﻿using API.Attributes;
using Application.DTOs.Users.ChangePassword;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    [Authorize]
    public class AccountsController : BaseController
    {
        private readonly IUserService _userService;

        public AccountsController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest requestModel)
        {
            try
            {
                requestModel.Id = CurrentUser?.Id;

                var response = await _userService.ChangePasswordAsync(requestModel);

                if (!response.IsSuccess)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception exception)
            {
                return HandleException(exception);
            }
        }
    }
}