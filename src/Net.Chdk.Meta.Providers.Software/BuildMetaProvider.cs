using Net.Chdk;
using Net.Chdk.Meta.Providers.Software;
using Net.Chdk.Model.Software;
using Net.Chdk.Providers.Product;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Net.Chdk.Meta.Providers.Software
{
    sealed class BuildMetaProvider : IBuildMetaProvider
    {
        private IProductProvider ProductProvider { get; }

        public BuildMetaProvider(IProductProvider productProvider)
        {
            ProductProvider = productProvider;
            _builds = new Lazy<Dictionary<Version, SoftwareBuildInfo>[]>(GetBuilds);
        }

        public SoftwareBuildInfo GetBuild(SoftwareInfo software)
        {
            if (software.Camera != null)
                return Builds[0][software.Product.Version];
            else
                return Builds[1][software.Product.Version];
        }

        #region Builds

        private readonly Lazy<Dictionary<Version, SoftwareBuildInfo>[]> _builds;

        private Dictionary<Version, SoftwareBuildInfo>[] Builds => _builds.Value;

        private Dictionary<Version, SoftwareBuildInfo>[] GetBuilds()
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new[] { new VersionConverter() }
            };
            var productName = ProductProvider.GetProductNames().Single();
            var path = Path.Combine(Directories.Data, Directories.Product, productName, "builds.json");
            using (var reader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return JsonSerializer.CreateDefault(settings).Deserialize<Dictionary<Version, SoftwareBuildInfo>[]>(jsonReader);
            }
        }

        #endregion
    }
}
