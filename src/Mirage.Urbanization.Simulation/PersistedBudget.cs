namespace Mirage.Urbanization.Simulation
{
    public class PersistedBudget : IBudget
    {
        public decimal ResidentialTaxRate { get; set; }
        public decimal CommercialTaxRate { get; set; }
        public decimal IndustrialTaxRate { get; set; }
        public decimal InfrastructureServiceRate { get; set; }
        public decimal PoliceServiceRate { get; set; }
        public decimal FireDepartmentServiceRate { get; set; }

        public static PersistedBudget GetDefaultBudget()
        {
            return new PersistedBudget
            {
                ResidentialTaxRate = 0.07M,
                CommercialTaxRate = 0.07M,
                IndustrialTaxRate = 0.07M,
                InfrastructureServiceRate = 1M,
                PoliceServiceRate = 1M,
                FireDepartmentServiceRate = 1M
            };
        }
    }
}