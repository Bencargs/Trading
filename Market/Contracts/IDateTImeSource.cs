using System;

namespace Contracts
{
    public interface IDateTimeSource
    {
        DateTime Now { get; }
        void Fastforward(TimeSpan timespan);
    }
}
