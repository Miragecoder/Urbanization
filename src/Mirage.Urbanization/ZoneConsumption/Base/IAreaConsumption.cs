using System;
using System.Collections.Generic;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IAreaConsumption
    {
        string Name { get; }
        char KeyChar { get; }
        int Cost { get; }
        BuildStyle BuildStyle { get; }
    }

    public enum BuildStyle
    {
        SingleClick,
        ClickAndDrag
    }
}