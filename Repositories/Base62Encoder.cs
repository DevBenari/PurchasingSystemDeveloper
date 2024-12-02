using System.Numerics;
using System.Text;

namespace PurchasingSystemDeveloper.Repositories
{
    public static class Base62Encoder
    {
        private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const int Base = 62;

        public static string Encode(byte[] input)
        {
            var value = new BigInteger(input.Reverse().ToArray()); // Reverse untuk memastikan endianess sesuai
            var result = new StringBuilder();

            while (value > 0)
            {
                var remainder = (int)(value % Base);
                result.Insert(0, Alphabet[remainder]);
                value /= Base;
            }

            return result.ToString();
        }

        public static byte[] Decode(string input)
        {
            var value = new BigInteger(0);

            foreach (var c in input)
            {
                var index = Alphabet.IndexOf(c);
                if (index < 0)
                    throw new FormatException("Invalid character in Base62 string.");

                value = value * Base + index;
            }

            return value.ToByteArray().Reverse().ToArray();
        }
    }
}
