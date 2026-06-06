using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Helper
{
    public class JWT
    {
        public string SecritKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInMinutes { get; set; }
    }
}
