using Net.Chdk.Meta.Providers;
using Net.Chdk.Model.Software;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Net.Chdk.Meta.Providers.Software
{
    sealed class EncodingMetaProvider : IEncodingMetaProvider
    {
        #region Fields

        private IBootMetaProvider BootProvider { get; }

        #endregion

        #region Constructor

        public EncodingMetaProvider(IBootMetaProvider bootProvider)
        {
            BootProvider = bootProvider;

            _encodings = new Lazy<SoftwareEncodingInfo[]>(GetEncodings);
            _encodingsDictionary = new Lazy<Dictionary<uint, SoftwareEncodingInfo>>(GetEncodingsDictionary);
        }

        #endregion

        #region IEncodingMetaProvider Members

        public SoftwareEncodingInfo GetEncoding(SoftwareEncodingInfo encoding)
        {
            return EncodingsDictionary[encoding.Data ?? 0];
        }

        #endregion

        #region Encodings

        private readonly Lazy<SoftwareEncodingInfo[]> _encodings;

        private SoftwareEncodingInfo[] Encodings => _encodings.Value;

        private SoftwareEncodingInfo[] GetEncodings()
        {
            var length = BootProvider.Offsets != null
                ? BootProvider.Offsets.Length
                : 1;
            return Enumerable.Range(0, length)
                .Select(GetEncoding)
                .ToArray();
        }

        private SoftwareEncodingInfo GetEncoding(int i)
        {
            return i > 0
                ? GetEncoding(BootProvider.Offsets[i - 1])
                : GetEmptyEncoding();
        }

        private static SoftwareEncodingInfo GetEmptyEncoding()
        {
            return new SoftwareEncodingInfo
            {
                Name = string.Empty
            };
        }

        private static SoftwareEncodingInfo GetEncoding(int[] offsets)
        {
            return new SoftwareEncodingInfo
            {
                Name = "dancingbits",
                Data = GetOffsets(offsets)
            };
        }

        private static uint? GetOffsets(int[] offsets)
        {
            var uOffsets = 0u;
            for (int index = 0; index < offsets.Length; index++)
                uOffsets += (uint)offsets[index] << (index << 2);
            return uOffsets;
        }

        #endregion

        #region EncodingsDictionary

        private readonly Lazy<Dictionary<uint, SoftwareEncodingInfo>> _encodingsDictionary;

        private Dictionary<uint, SoftwareEncodingInfo> EncodingsDictionary => _encodingsDictionary.Value;

        private Dictionary<uint, SoftwareEncodingInfo> GetEncodingsDictionary()
        {
            return Encodings
                .ToDictionary(e => e.Data ?? 0, e => e);
        }

        #endregion
    }
}
