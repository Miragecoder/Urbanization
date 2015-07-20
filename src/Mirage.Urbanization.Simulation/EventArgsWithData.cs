using System;

namespace Mirage.Urbanization.Simulation
{
    public class EventArgsWithData<TEventData> : EventArgs
        where TEventData : class
    {
        public TEventData EventData { get; }

        public EventArgsWithData(TEventData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            EventData = data;
        }
    }
}