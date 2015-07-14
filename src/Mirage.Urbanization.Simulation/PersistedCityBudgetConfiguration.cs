namespace Mirage.Urbanization.Simulation
{
    public class PersistedCityBudgetConfiguration : ICityBudgetConfiguration
    {
        public decimal ResidentialTaxRate { get; set; }
        public decimal CommercialTaxRate { get; set; }
        public decimal IndustrialTaxRate { get; set; }
        public decimal PoliceServiceRate { get; set; }
        public decimal FireDepartmentServiceRate { get; set; }
        public decimal RoadInfrastructureServiceRate { get; set; }
        public decimal RailroadInfrastructureServiceRate { get; set; }

        public static PersistedCityBudgetConfiguration GetDefaultBudget()
        {
            return new PersistedCityBudgetConfiguration
            {
                ResidentialTaxRate = 0.07M,
                CommercialTaxRate = 0.07M,
                IndustrialTaxRate = 0.07M,
                RoadInfrastructureServiceRate = 1M,
                RailroadInfrastructureServiceRate = 1M,
                PoliceServiceRate = 1M,
                FireDepartmentServiceRate = 1M
            };
        }
    }
}