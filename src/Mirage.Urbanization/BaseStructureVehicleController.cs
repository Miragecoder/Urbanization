using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    internal abstract class BaseStructureVehicleController<TVehicle, TStructure> : BaseVehicleController<TVehicle>
        where TVehicle : IMoveableVehicle
        where TStructure : BaseZoneClusterConsumption
    {
        protected BaseStructureVehicleController(Func<ISet<IZoneInfo>> getZoneInfosFunc)
            : base(getZoneInfosFunc)
        {

        }

        protected override sealed void PerformMoveCycle()
        {
            PrepareVehiclesWithStructures(GetStructures());


            foreach (var plane in Vehicles)
            {
                plane.Move();
            }

            RemoveOrphanVehicles();
        }

        protected abstract void PrepareVehiclesWithStructures(TStructure[] structures);

        private TStructure[] GetStructures()
        {
            return GetZoneInfosFunc()
                .Select(x => x.GetAsZoneCluster<TStructure>())
                .Where(x => x.HasMatch)
                .Select(x => x.MatchingObject)
                .Where(x => x.HasPower)
                .Distinct()
                .ToArray();
        }
    }
}