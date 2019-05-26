using Contracts;
using System;

namespace Exchange
{
    public class DateTimeSource : IDateTimeSource
    {
        private TimeSpan _offset = new TimeSpan(0);

        public DateTime Now => DateTime.Now + _offset;

        public void Fastforward(TimeSpan timespan)
        {
            _offset = timespan;
        }
    }
}
