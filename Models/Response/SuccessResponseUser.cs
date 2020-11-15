using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSlabon.Models.Entities;

namespace TestSlabon.Models.Response
{
    public class SuccessResponseUser: SuccessResponse
    {
        public UserResponse Data { get; set; }
    }
}
