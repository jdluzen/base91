using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasE91
{
    public class BasE91
    {
        protected uint queue;
        protected uint nbits;
        protected int val;

        protected static readonly char[] enctabc = new char[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '!', '#', '$',
            '%', '&', '(', ')', '*', '+', ',', '.', '/', ':', ';', '<', '=',
            '>', '?', '@', '[', ']', '^', '_', '`', '{', '|', '}', '~', '"'
        };

        protected static readonly byte[] dectab = new byte[]
        {
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 62, 90, 63, 64, 65, 66, 91, 67, 68, 69, 70, 71, 91, 72, 73,
            52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 74, 75, 76, 77, 78, 79,
            80,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14,
            15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 81, 91, 82, 83, 84,
            85, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
            41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 86, 87, 88, 89, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91,
            91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91
        };

        public BasE91()
        {
            queue = nbits = 0;
            val = -1;
        }

        public virtual IEnumerable<byte> Decode(string input)
        {
            byte d;
            List<byte> ob = new List<byte>((int)(input.Length / 1.22));
            foreach (char c in input)
            {
                d = dectab[c];
                if (d == 91)
                    continue;/* ignore non-alphabet chars */
                if (val == -1)
                    val = d;/* start next value */
                else
                {
                    val += d * 91;
                    queue |= (uint)(val << (int)nbits);
                    nbits += ((uint)val & 8191) > 88 ? (uint)13 : (uint)14;
                    do
                    {
                        ob.Add((byte)queue);
                        queue >>= 8;
                        nbits -= 8;
                    } while (nbits > 7);
                    val = -1;/* mark value complete */
                }
            }
            byte b;
            if (FlushDecode(out b))
                ob.Add(b);
            return ob;
        }

        protected virtual bool FlushDecode(out byte b)
        {
            bool ok = false;
            if (val != -1)
            {
                b = (byte)(queue | (uint)val << (int)nbits);
                ok = true;
            }
            else
                b = 0;
            queue = nbits = 0;
            val = -1;
            return ok;
        }

        public virtual StringBuilder Encode(byte[] input)
        {
            return Encode((IEnumerable<byte>)input);
        }

        public virtual StringBuilder Encode(IEnumerable<byte> input)
        {
            return Encode(input, -1);
        }

        public virtual StringBuilder Encode(IEnumerable<byte> input, int len)
        {
            StringBuilder builder = len == -1 ? new StringBuilder() : new StringBuilder((int)(len * 1.24));//"up to 23%"
            int index = 0;
            foreach (byte b in input)
            {
                if (len > -1 && index >= len)
                    break;
                queue |= (uint)b << (int)nbits;
                nbits += 8;
                if (nbits > 13)/* enough bits in queue */
                {
                    uint lval = queue & 8191;
                    if (lval > 88)
                    {
                        queue >>= 13;
                        nbits -= 13;
                    }
                    else/* we can take 14 bits */
                    {
                        lval = queue & 16383;
                        queue >>= 14;
                        nbits -= 14;
                    }
                    builder.Append(enctabc[lval % 91]).Append(enctabc[lval / 91]);
                }
                index++;
            }
            string s;
            if (FlushEncode(out s))
                builder.Append(s);
            return builder;
        }

        protected virtual bool FlushEncode(out string s)
        {
            s = string.Empty;
            bool ok = false;
            if (nbits > 0)
            {
                s = new string(enctabc[queue % 91], 1);
                if (nbits > 7 || queue > 90)
                    s += enctabc[queue / 91];
                ok = true;
            }
            queue = 0;
            nbits = 0;
            val = -1;
            return ok;
        }
    }
}
