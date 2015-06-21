using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Simulation
{
    public class CityCategoryDefinition
    {
        private readonly string _name;
        private readonly int _minimumPopulation;

        public CityCategoryDefinition(string name, int minimumPopulation)
        {
            _name = name;
            _minimumPopulation = minimumPopulation;
        }

        public string Name { get { return _name; } }
        public int MinimumPopulation { get { return _minimumPopulation; } }

        public static CityCategoryDefinition GetForPopulation(int population)
        {
            return Definitions
                .Where(x => x.MinimumPopulation <= population)
                .OrderByDescending(x => x.MinimumPopulation)
                .First();
        }

        private static readonly IReadOnlyCollection<CityCategoryDefinition> Definitions = new[]
        {
            new CityCategoryDefinition("Village", 0),
            new CityCategoryDefinition("Town", 2000), 
            new CityCategoryDefinition("City", 10000), 
            new CityCategoryDefinition("Capital", 50000), 
            new CityCategoryDefinition("Metropolis", 100000)
        }.ToList();
    }
}