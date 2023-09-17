using System;
using System.Collections.Generic;
using System.Text;

namespace Mx.NET.SDK.Provider.Dtos.API.Account
{
    public class AccountHistoryTokenDto
    {
        public string Address { get; set; }
        public string Balance { get; set; }
        public long Timestamp { get; set; }
        public bool? IsSender { get; set; }
        public string Token { get; set; }
    }
}
