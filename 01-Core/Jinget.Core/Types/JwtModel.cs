using System;

namespace Jinget.Core.Types
{
    public class JwtModel
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public DateTime NotBefore { get; set; } = DateTime.Now;
    }
}
