﻿namespace TwoStepAuthentication.Models
{
    public class Verify2FARequest
    {
        public string Code { get; set; }
        public string Username { get; set; }

    }
}
