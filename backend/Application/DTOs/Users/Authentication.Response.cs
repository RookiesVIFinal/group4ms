﻿using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Users;

public class AuthenticationResponse
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Token { get; set; } = null!;
}