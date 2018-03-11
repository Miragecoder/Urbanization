using System;
using System.Drawing;
using System.Linq;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public abstract class BaseZoneConsumption : IAreaZoneConsumption
    {
        public abstract string Name { get; }
        public abstract Color Color { get; }
        public abstract char KeyChar { get; }
        public string ColorName => ColorTranslator.ToHtml(Color);
        public abstract Image Tile { get; }

        public abstract int Cost { get; }
        public abstract BuildStyle BuildStyle { get; }

        public abstract IGetCanOverrideWithResult GetCanOverrideWith(IAreaZoneConsumption consumption);
    }

    public abstract class BaseSingleZoneConsumption : BaseZoneConsumption
    {
        
    }
}