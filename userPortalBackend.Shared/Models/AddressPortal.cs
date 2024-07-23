using System;
using System.Collections.Generic;

namespace userPortalBackend.presentation.Data.Models;

public partial class AddressPortal
{
    public int AddressId { get; set; }

    public int? UserId { get; set; }

    public string? AddressLine1 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? ZipCode { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City2 { get; set; }

    public string? State2 { get; set; }

    public string? Country2 { get; set; }

    public string? ZipCode2 { get; set; }

    public virtual UserPortal? User { get; set; }
}
