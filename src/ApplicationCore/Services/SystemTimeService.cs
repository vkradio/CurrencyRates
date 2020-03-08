using ApplicationCore.Interfaces;
using System;

namespace ApplicationCore.Services
{
    public class SystemTimeService : ITimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
