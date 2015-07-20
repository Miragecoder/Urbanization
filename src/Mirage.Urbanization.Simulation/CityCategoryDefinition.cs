using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Simulation
{
    public class CityCategoryDefinition
    {
        public CityCategoryDefinition(string name, int minimumPopulation)
        {
            Name = name;
            MinimumPopulation = minimumPopulation;
        }

        public string Name { get; }

        public int MinimumPopulation { get; }

        public static CityCategoryDefinition GetForPopulation(int population)
        {
            return Definitions
                .Where(x => x.MinimumPopulation <= population)
                .OrderByDescending(x => x.MinimumPopulation)
                .First();
        }

        public static CityCategoryDefinition Village = new CityCategoryDefinition("Village", 0);

        private static readonly IReadOnlyCollection<CityCategoryDefinition> Definitions = new[]
        {
            Village,
            new CityCategoryDefinition("Town", 2000), 
            new CityCategoryDefinition("City", 10000), 
            new CityCategoryDefinition("Capital", 50000), 
            new CityCategoryDefinition("Metropolis", 100000)
        }.ToList();
    }
}