using System;

namespace Mirage.Urbanization.GrowthPathFinding
{
    internal class GrowthAlgorithmHighlightState : IGrowthAlgorithmHighlightState
    {
        private DateTime _lastChange = DateTime.Now;

        private HighlightState _lastHighlightState;

        public HighlightState Current
        {
            get
            {
                return _lastChange > DateTime.Now.AddMilliseconds(-40)
                    ? _lastHighlightState
                    : HighlightState.None;
            }
        }

        public void SetState(HighlightState highlightState)
        {
            _lastHighlightState = highlightState;
            _lastChange = DateTime.Now;
        }
    }
}