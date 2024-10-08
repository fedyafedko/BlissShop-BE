﻿namespace BlissShop.Common.Requests;

public class ResetPasswordRequest
{
    public string ResetToken { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
