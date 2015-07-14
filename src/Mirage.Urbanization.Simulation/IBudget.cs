namespace Mirage.Urbanization.Simulation
{
    public interface IBudget
    {
        decimal ResidentialTaxRate { get; set; }
        decimal CommercialTaxRate { get; set; }
        decimal IndustrialTaxRate { get; set; }

        decimal InfrastructureServiceRate { get; set; }
        decimal PoliceServiceRate { get; set; }
        decimal FireDepartmentServiceRate { get; set; }
    }
}