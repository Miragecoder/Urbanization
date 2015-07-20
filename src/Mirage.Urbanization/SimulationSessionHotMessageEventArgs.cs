namespace Mirage.Urbanization
{
    public class SimulationSessionHotMessageEventArgs : SimulationSessionMessageEventArgs
    {
        private readonly string _title;
        public string Title => _title;

        public SimulationSessionHotMessageEventArgs(string title, string message)
            :base(message)
        {
            _title = title;
        }
    }
}