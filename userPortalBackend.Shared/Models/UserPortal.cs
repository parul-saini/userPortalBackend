using System;
using System.Collections.Generic;

namespace userPortalBackend.presentation.Data.Models;

public partial class UserPortal
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly? DateOfJoining { get; set; }

    public DateOnly? Dob { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? AlternatePhone { get; set; }

    public string? ImageUrl { get; set; }

    public string? Role { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? Active { get; set; }

    public string? Password { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AddressPortal> AddressPortals { get; set; } =
        new List<AddressPortal>();

    public virtual UserPortal? CreatedByNavigation { get; set; }

    public virtual ICollection<UserPortal> InverseCreatedByNavigation { get; set; } = new List<UserPortal>();
}
