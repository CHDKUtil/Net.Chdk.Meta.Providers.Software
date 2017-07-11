using Net.Chdk.Model.Software;
using System.Collections.Generic;

namespace Net.Chdk.Meta.Providers.Software
{
    sealed class EncodingMetaProvider : IEncodingMetaProvider
    {
        #region Fields

        private IBootMetaProvider BootProvider { get; }
        private Dictionary<uint, SoftwareEncodingInfo> Encodings { get; }

        #endregion

        #region Constructor

        public EncodingMetaProvider(IBootMetaProvider bootProvider)
        {
            BootProvider = bootProvider;
            Encodings = new Dictionary<uint, SoftwareEncodingInfo>();
        }

        #endregion

        #region IEncodingMetaProvider Members

        public SoftwareEncodingInfo GetEncoding(SoftwareInfo software)
        {
            var key = software.Encoding.Data ?? 0;
            SoftwareEncodingInfo encoding;
            if (!Encodings.TryGetValue(key, out encoding))
            {
                encoding = GetEncoding(software.Encoding.Data);
                Encodings.Add(key, encoding);
            }
            return encoding;
        }

        #endregion

        #region Encodings

        private static SoftwareEncodingInfo GetEncoding(uint? data)
        {
            return data.HasValue
                ? GetEncoding(data.Value)
                : GetEmptyEncoding();
        }

        private static SoftwareEncodingInfo GetEmptyEncoding()
        {
            return new SoftwareEncodingInfo
            {
                Name = string.Empty
            };
        }

        private static SoftwareEncodingInfo GetEncoding(uint offsets)
        {
            return new SoftwareEncodingInfo
            {
                Name = "dancingbits",
                Data = offsets,
            };
        }

        #endregion
    }
}
