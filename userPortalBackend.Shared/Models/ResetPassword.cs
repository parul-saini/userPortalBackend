using System;
using System.Collections.Generic;

namespace userPortalBackend.presentation.TempModels;

public partial class ResetPassword
{
    public int Id { get; set; }

    public string? ResetPasswordToken { get; set; }

    public DateTime? ResetPasswordExpiry { get; set; }

    public int? UserId { get; set; }
}
