using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestSlabon.Models.Response
{
    public class SuccessResponseUsers:SuccessResponse
    {
        public List<UserResponse> Data { get; set; }
    }
}
