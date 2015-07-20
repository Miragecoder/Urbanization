using System;

namespace Mirage.Urbanization.Simulation
{
    public class EventArgsWithData<TEventData> : EventArgs
        where TEventData : class
    {
        private readonly TEventData _data;
        public TEventData EventData { get { return _data; } }

        public EventArgsWithData(TEventData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            _data = data;
        }
    }
}