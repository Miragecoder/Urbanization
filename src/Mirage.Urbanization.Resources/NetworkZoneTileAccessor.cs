using System;
using System.Linq;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    class NetworkZoneTileAccessor
    {
        private readonly IBaseNetworkZoneTileAccessor[] _accessors = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(
                domainAssembly => domainAssembly.GetTypes(), 
                (domainAssembly, assemblyType) => new { domainAssembly, assemblyType }
            )
            .Where(@t => typeof(IBaseNetworkZoneTileAccessor).IsAssignableFrom(@t.assemblyType))
            .Where(@t => !@t.assemblyType.IsAbstract)
            .Where(@t => @t.assemblyType.IsClass)
            .Select(@t => @t.assemblyType)
            .Select(Activator.CreateInstance).Cast<IBaseNetworkZoneTileAccessor>()
            .ToArray();

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapshot)
        {
            return _accessors
                .Select(x => x.GetFor(snapshot).MatchingObject)
                .SingleOrDefault(x => x != null)
                .ToQueryResult();
        }
    }
}