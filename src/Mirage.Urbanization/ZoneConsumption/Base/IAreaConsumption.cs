using System;
using System.Collections.Generic;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IAreaConsumption
    {
        string Name { get; }
        char KeyChar { get; }
    }
}