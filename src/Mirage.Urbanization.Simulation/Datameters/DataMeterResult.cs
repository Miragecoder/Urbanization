namespace Mirage.Urbanization.Simulation.Datameters
{
    public struct DataMeterResult
    {
        private readonly string _name;
        private readonly decimal _percentageScore;
        private readonly int _amount;
        private readonly DataMeterValueCategory _valueCategory;

        public DataMeterResult(
            string name, 
            int amount, 
            decimal percentageScore, 
            DataMeterValueCategory valueCategory
        )
        {
            _name = name;
            _percentageScore = percentageScore;
            _valueCategory = valueCategory;
            _amount = amount;
        }

        public string Name => _name;
        public decimal PercentageScore => _percentageScore;

        public string PercentageScoreString => _percentageScore.ToString("P");
        public DataMeterValueCategory ValueCategory => _valueCategory;
        public int Amount => _amount;
    }
}