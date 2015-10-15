namespace Mirage.Urbanization.Simulation.Datameters
{
    public struct DataMeterResult
    {
        public DataMeterResult(
            string name, 
            int amount, 
            decimal percentageScore, 
            DataMeterValueCategory valueCategory
        )
        {
            Name = name;
            PercentageScore = percentageScore;
            ValueCategory = valueCategory;
            Amount = amount;
        }

        public string Name { get; }

        public decimal PercentageScore { get; }

        public string PercentageScoreString => PercentageScore.ToString("P");
        public DataMeterValueCategory ValueCategory { get; }        

        public int Amount { get; }
    }
}