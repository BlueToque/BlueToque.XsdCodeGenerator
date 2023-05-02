using System;
using System.Globalization;

namespace W3CXSD2001
{
    [Serializable]
    public sealed class GDay : IXsd
    {
        // Fields
        private static readonly string[] s_formats = new string[] { "---dd", "---ddzzz" };

        // Methods
        public GDay() => Value = DateTime.MinValue;

        public GDay(DateTime value) => Value = value;

        public string XsdType1 => XsdType;

        public static GDay Parse(string value) => new GDay(DateTime.ParseExact(value, s_formats, CultureInfo.InvariantCulture, DateTimeStyles.None));

        public override string ToString() => Value.ToString("---dd", CultureInfo.InvariantCulture);

        // Properties
        public DateTime Value { get; set; }

        public static string XsdType => "gDay";
    }

}
