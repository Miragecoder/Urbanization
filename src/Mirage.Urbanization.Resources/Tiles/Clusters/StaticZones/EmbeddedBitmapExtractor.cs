using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Mirage.Urbanization.Tilesets.Tiles.Clusters.StaticZones
{
    public class EmbeddedBitmapExtractor
    {
        public IEnumerable<EmbeddedResourceBitmap> GetBitmapsFromNamespace(string @namespace)
        {
            return Assembly
                .GetExecutingAssembly()
                .GetManifestResourceNames()
                .Where(x => x.Contains(@namespace))
                .Select(resourceName => new EmbeddedResourceBitmap(resourceName,
                    new Bitmap(
                        Image.FromStream(
                            Assembly
                                .GetExecutingAssembly()
                                .GetManifestResourceStream(resourceName)
                            )
                        )
                    )
                );
        }

        public class EmbeddedResourceBitmap
        {
            public EmbeddedResourceBitmap(string resourceName, Bitmap bitmap)
            {
                ResourceName = resourceName;
                Bitmap = bitmap;
            }

            public string ResourceName { get; }
            public Bitmap Bitmap { get; }
        }
    }
}