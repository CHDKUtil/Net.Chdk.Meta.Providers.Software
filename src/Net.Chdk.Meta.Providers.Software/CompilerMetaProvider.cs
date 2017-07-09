using Net.Chdk.Model.Software;
using Net.Chdk.Providers.Product;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Linq;

namespace Net.Chdk.Meta.Providers.Software
{
    sealed class CompilerMetaProvider : ICompilerMetaProvider
    {
        private IProductProvider ProductProvider { get; }

        public CompilerMetaProvider(IProductProvider productProvider)
        {
            ProductProvider = productProvider;
            _compilers = new Lazy<SoftwareCompilerInfo[]>(GetCompilers);
        }

        public SoftwareCompilerInfo GetCompiler(SoftwareInfo software)
        {
            if (software.Camera != null)
                return Compilers[0];
            else
                return Compilers[1];
        }

        #region Compilers

        private readonly Lazy<SoftwareCompilerInfo[]> _compilers;

        private SoftwareCompilerInfo[] Compilers => _compilers.Value;

        private SoftwareCompilerInfo[] GetCompilers()
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new[] { new VersionConverter() }
            };
            var productName = ProductProvider.GetProductNames().Single();
            var path = Path.Combine(Directories.Data, Directories.Product, productName, "compilers.json");
            using (var reader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return JsonSerializer.CreateDefault(settings).Deserialize<SoftwareCompilerInfo[]>(jsonReader);
            }
        }

        #endregion
    }
}
