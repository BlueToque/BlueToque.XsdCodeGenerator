using System;
using System.Globalization;

namespace W3CXSD2001
{
    [Serializable]
    public sealed class GMonth : IXsd
    {
        // Fields
        private static readonly string[] s_formats = new string[] { "--MM--", "--MM--zzz" };

        // Methods
        public GMonth() => Value = DateTime.MinValue;

        public GMonth(DateTime value) => Value = value;

        public string XsdType1 => XsdType;

        public static GMonth Parse(string value) => new GMonth(DateTime.ParseExact(value, s_formats, CultureInfo.InvariantCulture, DateTimeStyles.None));

        public override string ToString() => Value.ToString("--MM--", CultureInfo.InvariantCulture);

        // Properties
        public DateTime Value { get; set; }

        public static string XsdType => "gMonth";
    }


}
