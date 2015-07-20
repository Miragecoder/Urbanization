using System;
using Mirage.Urbanization.Simulation.Persistence;
using Mirage.Urbanization.Simulation.Datameters;

namespace Mirage.Urbanization.Simulation
{
    public class SimulationOptions
    {
        private readonly TerraformingOptions _terraformingOptions;
        private readonly PersistedSimulation _persistedSimulation;

        public SimulationOptions(TerraformingOptions terraformingOptions, ProcessOptions processOptions)
        {
            _terraformingOptions = terraformingOptions;
            ProcessOptions = processOptions;

        }
        public SimulationOptions(PersistedSimulation persistedSimulation, ProcessOptions processOptions)
        {
            _persistedSimulation = persistedSimulation;
            ProcessOptions = processOptions;
        }

        public void WithPersistedSimulation(Action<PersistedSimulation> action)
        {
            if (_persistedSimulation != null)
                action(_persistedSimulation);
        }

        public AreaOptions GetAreaOptions()
        {
            if (_terraformingOptions != null && _persistedSimulation == null)
                return new AreaOptions(LandValueCalculator.Instance, _terraformingOptions, ProcessOptions);
            else if (_persistedSimulation != null && _terraformingOptions == null)
                return new AreaOptions(LandValueCalculator.Instance, _persistedSimulation.PersistedArea, ProcessOptions);
            else
                throw new InvalidOperationException();
        }

        public ProcessOptions ProcessOptions { get; }
    }
}
