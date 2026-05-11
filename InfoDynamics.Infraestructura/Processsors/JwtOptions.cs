using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDynamics.Infraestructura.Processors
{
    public class JwtOptions
    {
        public const string JwtOptionKey = "JwtOptions";

        public string Secret {  get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationTimeInMinutes { get; set; } = 30; // Default to 30 minutes
    }
}
