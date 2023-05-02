using System;
using System.Globalization;

namespace W3CXSD2001
{
    [Serializable]
    public sealed class GYearMonth : IXsd
    {
        // Fields
        private static readonly string[] s_formats = new string[] { "yyyy-MM", "'+'yyyy-MM", "'-'yyyy-MM", "yyyy-MMzzz", "'+'yyyy-MMzzz", "'-'yyyy-MMzzz" };

        // Methods
        public GYearMonth()
        {
            Value = DateTime.MinValue;
            Sign = 0;
        }

        public GYearMonth(DateTime value)
        {
            Sign = 0;
            Value = value;
        }

        public GYearMonth(DateTime value, int sign)
        {
            Value = value;
            Sign = sign;
        }

        public string XsdType1 => XsdType;

        public static GYearMonth Parse(string value) => new GYearMonth(DateTime.ParseExact(value, s_formats, CultureInfo.InvariantCulture, DateTimeStyles.None), value[0] == '-' ? -1 : 0);

        public override string ToString() => Sign < 0
                ? Value.ToString("'-'yyyy-MM", CultureInfo.InvariantCulture)
                : Value.ToString("yyyy-MM", CultureInfo.InvariantCulture);

        // Properties
        public int Sign { get; set; }

        public DateTime Value { get; set; }

        public static string XsdType => "gYearMonth";
    }


}
