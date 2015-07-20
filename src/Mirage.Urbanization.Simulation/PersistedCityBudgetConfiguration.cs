using System;

namespace Mirage.Urbanization.Simulation
{
    public class PersistedCityBudgetConfiguration : ICityBudgetConfiguration
    {
        public decimal ResidentialTaxRate { get; set; } = 0.7M;
        public decimal CommercialTaxRate { get; set; } = 0.7M;
        public decimal IndustrialTaxRate { get; set; } = 0.7M;
        public decimal PoliceServiceRate { get; set; } = 1M;
        public decimal FireDepartmentServiceRate { get; set; } = 1M;
        public decimal RoadInfrastructureServiceRate { get; set; } = 1M;
        public decimal RailroadInfrastructureServiceRate { get; set; } = 1M;
    }
}