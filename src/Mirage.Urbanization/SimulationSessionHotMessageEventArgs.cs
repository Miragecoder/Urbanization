namespace Mirage.Urbanization
{
    public class SimulationSessionHotMessageEventArgs : SimulationSessionMessageEventArgs
    {
        public string Title { get; }

        public SimulationSessionHotMessageEventArgs(string title, string message)
            :base(message)
        {
            Title = title;
        }
    }
}