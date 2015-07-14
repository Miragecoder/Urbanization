namespace Mirage.Urbanization.Simulation
{
    public interface ICityBudgetConfiguration
    {
        decimal ResidentialTaxRate { get; set; }
        decimal CommercialTaxRate { get; set; }
        decimal IndustrialTaxRate { get; set; }

        decimal RoadInfrastructureServiceRate { get; set; }
        decimal RailroadInfrastructureServiceRate { get; set; }
        decimal PoliceServiceRate { get; set; }
        decimal FireDepartmentServiceRate { get; set; }
    }
}