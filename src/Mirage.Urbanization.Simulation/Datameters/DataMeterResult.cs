namespace Mirage.Urbanization.Simulation.Datameters
{
    public struct DataMeterResult
    {
        private readonly DataMeter _dataMeter;

        public DataMeterResult(
            DataMeter dataMeter, 
            int amount, 
            decimal percentageScore, 
            DataMeterValueCategory valueCategory
        )
        {
            _dataMeter = dataMeter;
            PercentageScore = percentageScore;
            ValueCategory = valueCategory;
            Amount = amount;
        }

        public string Name => _dataMeter.Name;

        public int WebId => _dataMeter.WebId;

        public decimal PercentageScore { get; }

        public string PercentageScoreString => PercentageScore.ToString("P");
        public DataMeterValueCategory ValueCategory { get; }        

        public int Amount { get; }
    }
}