using System;
using System.Collections.Generic;

namespace Mirage.Urbanization.Vehicles
{
    internal abstract class BaseVehicle : IVehicle
    {
        protected readonly Func<ISet<IZoneInfo>> GetZoneInfosFunc;

        protected BaseVehicle(Func<ISet<IZoneInfo>> getZoneInfosFunc, IZoneInfo currentPosition)
        {
            GetZoneInfosFunc = getZoneInfosFunc;
            CurrentPosition = currentPosition;
        }

        protected void Move(IZoneInfo next)
        {
            PreviousPreviousPreviousPreviousPosition = PreviousPreviousPreviousPosition;
            PreviousPreviousPreviousPosition = PreviousPreviousPosition;
            PreviousPreviousPosition = PreviousPosition;

            PreviousPosition = CurrentPosition;

            CurrentPosition = next;
        }

        public IZoneInfo PreviousPreviousPreviousPreviousPosition { get; protected set; }
        public IZoneInfo PreviousPreviousPreviousPosition { get; protected set; }
        public IZoneInfo PreviousPreviousPosition { get; protected set; }
        public IZoneInfo PreviousPosition { get; protected set; }
        public IZoneInfo CurrentPosition { get; protected set; }

        private DateTime _lastChange = DateTime.Now;

        public bool CanBeRemoved => CurrentPosition == null || _lastChange < DateTime.Now.AddSeconds(-3);

        protected abstract int SpeedInMilliseconds { get; }

        private decimal _cachedProgress;

        private DateTime _lastProgressCalculation = DateTime.MinValue;

        public decimal Progress
        {
            get
            {
                if ((DateTime.Now - _lastProgressCalculation).TotalMilliseconds > 10)
                {
                    decimal returnValue;
                    var range = (DateTime.Now - _lastChange);

                    if (range.Milliseconds > 0)
                    {
                        returnValue = Convert.ToDecimal(
                            (range.TotalMilliseconds / SpeedInMilliseconds
                            ));
                    }
                    else
                        returnValue = 0M;

                    if (returnValue < 0)
                        throw new InvalidOperationException();
                    _cachedProgress = returnValue;
                    _lastProgressCalculation = DateTime.Now;
                }
                return _cachedProgress;
            }
        } 

        protected void IfMustBeMoved(Action action)
        {
            if (_lastChange > DateTime.Now.AddMilliseconds(-SpeedInMilliseconds))
            {
                return;
            }
            _lastChange = DateTime.Now;
            action();
        }
    }
}