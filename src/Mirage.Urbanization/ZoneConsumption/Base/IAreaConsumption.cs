﻿using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IAreaConsumption
    {
        string Name { get; }
        char KeyChar { get; }
        int Cost { get; }
        BuildStyle BuildStyle { get; }
        Image Tile { get; }
    }

    public enum BuildStyle
    {
        SingleClick,
        ClickAndDrag
    }
}