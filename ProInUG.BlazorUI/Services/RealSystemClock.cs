using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Services
{
    public class RealSystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow => DateTime.UtcNow;
    }
}
