using SixLabors.ImageSharp;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Mirage.Urbanization.Tilesets
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
                    Image.Load(Assembly
                            .GetExecutingAssembly()
                            .GetManifestResourceStream(resourceName)
                        )
                    )
                );
        }

        public class EmbeddedResourceBitmap
        {
            public EmbeddedResourceBitmap(string resourceName, Image bitmap)
            {
                ResourceName = resourceName;
                Bitmap = bitmap;
            }

            public string ResourceName { get; }
            public Image Bitmap { get; }
            public string FileName => ResourceName.Split('.').Reverse().Skip(1).Take(1).Single();
        }
    }
}