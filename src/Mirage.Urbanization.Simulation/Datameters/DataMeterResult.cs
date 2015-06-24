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

        public string Name { get { return _name; } }
        public decimal PercentageScore { get { return _percentageScore; } }
        public DataMeterValueCategory ValueCategory { get { return _valueCategory; } }
        public int Amount { get { return _amount; } }
    }
}