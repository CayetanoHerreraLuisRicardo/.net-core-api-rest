using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestSlabon.Models.Response
{
    public class SuccessResponseLogin: SuccessResponse
    {
        public LoginResponse Data { get; set; }
    }
}
