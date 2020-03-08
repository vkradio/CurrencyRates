using ApplicationCore.Interfaces;
using System;

namespace UnitTests.Builders
{
    public class ManagedTimeService : ITimeService
    {
        public ManagedTimeService(DateTime? now = null) => Now = now ?? DateTime.UtcNow;

        public DateTime Now { get; private set; }

        public void Add(TimeSpan value) => Now = Now.Add(value);
        public void AddDays(double value) => Now = Now.AddDays(value);
        public void AddHours(double value) => Now = Now.AddHours(value);
        public void AddMilliseconds(double value) => Now = Now.AddMilliseconds(value);
        public void AddMinutes(double value) => Now = Now.AddMinutes(value);
        public void AddMonths(int value) => Now = Now.AddMonths(value);
        public void AddSeconds(double value) => Now = Now.AddSeconds(value);
        public void AddTicks(long value) => Now = Now.AddTicks(value);
        public void AddYears(int value) => Now = Now.AddYears(value);
    }
}
