using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Simulation
{
    public class CitySaveStateController
    {
        private readonly Action<bool> _onSaveFunctionalityAvailableAction;

        public CitySaveStateController(Action<bool> onSaveFunctionalityAvailableAction)
        {
            _onSaveFunctionalityAvailableAction = onSaveFunctionalityAvailableAction;
        }

        private string _lastSaveFileName;

        private readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(PersistedSimulation));

        public void ToggleSaveFunctionalityAvailable()
        {
            _onSaveFunctionalityAvailableAction(true);
        }

        public PersistedSimulation LoadFile(string fileName)
        {
            _onSaveFunctionalityAvailableAction(true);
            _lastSaveFileName = fileName;
            using (var file = File.OpenRead(fileName))
                return _xmlSerializer.Deserialize(file) as PersistedSimulation;
        }

        public bool CurrentFilenameIsKnown => _lastSaveFileName != null;

        public void Save(PersistedSimulation simulation)
        {
            if (!CurrentFilenameIsKnown) throw new InvalidOperationException();
            SaveFile(_lastSaveFileName, simulation);
        }

        public void SaveFile(string fileName, PersistedSimulation simulation)
        {
            using (var file = File.Open(fileName, FileMode.Create, FileAccess.Write))
                _xmlSerializer.Serialize(file, simulation);
            _lastSaveFileName = fileName;
        }
    }
}
