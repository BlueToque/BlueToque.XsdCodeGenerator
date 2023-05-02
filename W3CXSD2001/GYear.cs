using System;
using System.Globalization;

namespace W3CXSD2001
{
    [Serializable]
    public sealed class GYear : IXsd
    {
        // Fields
        private static readonly string[] s_formats = new string[] { "yyyy", "'+'yyyy", "'-'yyyy", "yyyyzzz", "'+'yyyyzzz", "'-'yyyyzzz" };

        // Methods
        public GYear()
        {
            Value = DateTime.MinValue;
            Sign = 0;
        }

        public GYear(DateTime value)
        {
            Sign = 0;
            Value = value;
        }

        public GYear(DateTime value, int sign)
        {
            Value = value;
            Sign = sign;
        }

        public string XsdType1 => XsdType;

        public static GYear Parse(string value) => new GYear(DateTime.ParseExact(value, s_formats, CultureInfo.InvariantCulture, DateTimeStyles.None), value[0] == '-' ? -1 : 0);

        public override string ToString() => Sign < 0
                ? Value.ToString("'-'yyyy", CultureInfo.InvariantCulture)
                : Value.ToString("yyyy", CultureInfo.InvariantCulture);

        // Properties
        public int Sign { get; set; }

        public DateTime Value { get; set; }

        public static string XsdType => "gYear";
    }


}
