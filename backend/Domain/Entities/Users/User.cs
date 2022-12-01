﻿using Domain.Base;
using Domain.Shared.Enums;

namespace Domain.Entities.Users;

public class User : AuditableEntity<Guid>
{
    public string StaffCode { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FullName => $"{FirstName} {LastName}";

    public string Username { get; set; } = null!;

    public string HashedPassword { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public DateTime JoinedDate { get; set; }

    public UserRole Role { get; set; }

    public Location Location { get; set; }

    public bool IsFirstTimeLogIn { get; set; } = true;
}