using System;
using System.Globalization;
using System.Text;

namespace W3CXSD2001
{
    public sealed class Duration
    {
        // Methods
        private static void CarryOver(int inDays, out int years, out int months, out int days)
        {
            years = inDays / 360;
            int num = years * 360;
            months = Math.Max(0, inDays - num) / 30;
            int num2 = months * 30;
            _ = Math.Max(0, inDays - (num + num2));
            days = inDays % 30;
        }

        public static TimeSpan Parse(string value)
        {
            TimeSpan span;
            int num = 1;
            try
            {
                if (value == null)
                    return TimeSpan.Zero;

                if (value[0] == '-')
                    num = -1;
                char[] chArray = value.ToCharArray();
                int[] numArray1 = new int[7];
                string s = "0";
                string str2 = "0";
                string str3 = "0";
                string str4 = "0";
                string str5 = "0";
                string str6 = "0";
                string str7 = "0";
                bool flag = false;
                bool flag2 = false;
                int startIndex = 0;
                for (int i = 0; i < chArray.Length; i++)
                {
                    switch (chArray[i])
                    {
                        case '.':
                            {
                                flag2 = true;
                                str6 = new string(chArray, startIndex, i - startIndex);
                                startIndex = i + 1;
                                continue;
                            }
                        case 'D':
                            {
                                str3 = new string(chArray, startIndex, i - startIndex);
                                startIndex = i + 1;
                                continue;
                            }
                        case 'H':
                            {
                                str4 = new string(chArray, startIndex, i - startIndex);
                                startIndex = i + 1;
                                continue;
                            }
                        case 'P':
                            {
                                startIndex = i + 1;
                                continue;
                            }
                        case 'Q':
                        case 'R':
                        case 'Z':
                            {
                                continue;
                            }
                        case 'S':
                            {
                                if (flag2)
                                {
                                    break;
                                }
                                str6 = new string(chArray, startIndex, i - startIndex);
                                continue;
                            }
                        case 'T':
                            {
                                flag = true;
                                startIndex = i + 1;
                                continue;
                            }
                        case 'M':
                            {
                                if (flag)
                                {
                                    str5 = new string(chArray, startIndex, i - startIndex);
                                }
                                else
                                {
                                    str2 = new string(chArray, startIndex, i - startIndex);
                                }
                                startIndex = i + 1;
                                continue;
                            }
                        case 'Y':
                            {
                                s = new string(chArray, startIndex, i - startIndex);
                                startIndex = i + 1;
                                continue;
                            }
                        default:
                            {
                                continue;
                            }
                    }
                    str7 = new string(chArray, startIndex, i - startIndex);
                }
                long ticks = num * (((((((long.Parse(s, CultureInfo.InvariantCulture) * 360L) + (long.Parse(str2) * 30L)) + long.Parse(str3)) * 0xc92a69c000L) + (long.Parse(str4, CultureInfo.InvariantCulture) * 0x861c46800L)) + (long.Parse(str5, CultureInfo.InvariantCulture) * 0x23c34600L)) + Convert.ToInt64((double)(double.Parse(str6 + "." + str7, CultureInfo.InvariantCulture) * 10000000)));
                span = new TimeSpan(ticks);
            }
            catch (Exception)
            {
                throw new Exception("Error parsing xsd:duration");
            }
            return span;
        }

        public static string ToString(TimeSpan timeSpan)
        {
            StringBuilder builder = new StringBuilder(10) { Length = 0 };
            if (TimeSpan.Compare(timeSpan, TimeSpan.Zero) < 1)
                builder.Append('-');

            CarryOver(Math.Abs(timeSpan.Days), out int years, out int months, out int days);
            builder.Append('P');
            builder.Append(years);
            builder.Append('Y');
            builder.Append(months);
            builder.Append('M');
            builder.Append(days);
            builder.Append("DT");
            _ = timeSpan.Hours;
            builder.Append(Math.Abs(timeSpan.Hours));
            builder.Append('H');
            builder.Append(Math.Abs(timeSpan.Minutes));
            builder.Append('M');
            builder.Append(Math.Abs(timeSpan.Seconds));
            if (Math.Abs(timeSpan.Milliseconds) != 0)
            {
                int l = (int)(Math.Abs((long)(timeSpan.Ticks % 0xc92a69c000L)) % 0x989680L);
                if (l != 0)
                {
                    string str = ParseNumbers.IntToString(l, 10, 7, '0', 0);
                    builder.Append('.');
                    builder.Append(str);
                }
            }
            builder.Append('S');
            return builder.ToString();
        }

        // Properties
        public static string XsdType { get { return "duration"; } }

    }

}
