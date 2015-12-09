using System;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public class AnimatedCellBitmapSetLayers
    {
        public AnimatedCellBitmapSet LayerOne { get; }
        public QueryResult<AnimatedCellBitmapSet> LayerTwo { get; }

        private readonly Lazy<AnimatedCellBitmapSetLayers> _rotatedCloneLazy;

        public AnimatedCellBitmapSetLayers(
            AnimatedCellBitmapSet layerOne,
            AnimatedCellBitmapSet layerTwo
            )
        {
            LayerOne = layerOne;
            LayerTwo = layerTwo.ToQueryResult();

            _rotatedCloneLazy = new Lazy<AnimatedCellBitmapSetLayers>(() =>
                new AnimatedCellBitmapSetLayers(LayerOne.Generate90DegreesRotatedClone(),
                    LayerTwo.WithResultIfHasMatch(x => x.Generate90DegreesRotatedClone())
                    ));
        }

        public AnimatedCellBitmapSetLayers Generate90DegreesRotatedClone()
        {
            return _rotatedCloneLazy.Value;
        }
    }
}