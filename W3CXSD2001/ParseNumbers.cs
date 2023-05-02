using System.Runtime.CompilerServices;

namespace W3CXSD2001
{
    internal class ParseNumbers
    {
        // Fields
        internal const int IsTight = 0x1000;
        internal const int PrintAsI1 = 0x40;
        internal const int PrintAsI2 = 0x80;
        internal const int PrintAsI4 = 0x100;
        internal const int TreatAsI1 = 0x400;
        internal const int TreatAsI2 = 0x800;
        internal const int TreatAsUnsigned = 0x200;
        internal static readonly int[] ZeroStart = new int[1];

        // Methods
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string IntToDecimalString(int i);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string IntToString(int l, int radix, int width, char paddingChar, int flags);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string LongToString(int radix, int width, long l, char paddingChar, int flags);

        public static string LongToString(long l, int radix, int width, char paddingChar, int flags) => LongToString(radix, width, l, paddingChar, flags);

        public static long RadixStringToLong(string s, int radix, bool isTight) => RadixStringToLong(s, radix, isTight, new int[1]);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern long RadixStringToLong(string s, int radix, bool isTight, int[] currPos);

        public static int StringToInt(string s, int radix, int flags) => StringToInt(s, radix, flags, new int[1]);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int StringToInt(string s, int radix, int flags, int[] currPos);

        public static long StringToLong(string s, int radix, int flags) => StringToLong(s, radix, flags, new int[1]);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern long StringToLong(string s, int radix, int flags, int[] currPos);
    }

}
