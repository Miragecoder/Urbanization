using System;

namespace Mirage.Urbanization.Simulation
{
    public class EventArgsWithData<TEventData> : EventArgs
        where TEventData : class
    {
        public TEventData EventData { get; }

        public EventArgsWithData(TEventData data)
        {
            EventData = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}