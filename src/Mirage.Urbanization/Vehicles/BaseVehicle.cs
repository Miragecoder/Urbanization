using System;
using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Vehicles
{
    internal abstract class BaseVehicle : IVehicle
    {
        protected readonly Func<ISet<IZoneInfo>> GetZoneInfosFunc;

        protected BaseVehicle(Func<ISet<IZoneInfo>> getZoneInfosFunc, IZoneInfo currentPosition)
        {
            GetZoneInfosFunc = getZoneInfosFunc;
            Trail.Enqueue(currentPosition);
        }

        protected readonly Queue<IZoneInfo> Trail = new Queue<IZoneInfo>(); 

        protected void Move(IZoneInfo next)
        {
            Trail.Enqueue(next);
            if (Trail.Count == 6)
                Trail.Dequeue();
            else if (Trail.Count > 6)
                throw new InvalidOperationException();
        }

        public IZoneInfo PreviousPreviousPreviousPreviousPosition => Trail.Reverse().Skip(4).FirstOrDefault();
        public IZoneInfo PreviousPreviousPreviousPosition => Trail.Reverse().Skip(3).FirstOrDefault();
        public IZoneInfo PreviousPreviousPosition => Trail.Reverse().Skip(2).FirstOrDefault();
        public IZoneInfo PreviousPosition => Trail.Reverse().Skip(1).FirstOrDefault();
        public IZoneInfo CurrentPosition => Trail.Reverse().FirstOrDefault();

        private DateTime _lastChange = DateTime.Now;

        public bool CanBeRemoved => CurrentPosition == null || _lastChange < DateTime.Now.AddSeconds(-3);
        public IEnumerable<IZoneInfo> TraversePath() => Trail.Reverse();

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