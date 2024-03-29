﻿/*
* basE91 encoding/decoding routines
*
* Copyright (c) 2000-2006 Joachim Henke
* Copyright (c) 2018, 2023 Joe Dluzen
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*
*  - Redistributions of source code must retain the above copyright notice,
*    this list of conditions and the following disclaimer.
*  - Redistributions in binary form must reproduce the above copyright notice,
*    this list of conditions and the following disclaimer in the documentation
*    and/or other materials provided with the distribution.
*  - Neither the name of Joachim Henke nor the names of his contributors may
*    be used to endorse or promote products derived from this software without
*    specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
* ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
* LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
* CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
* SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
* CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
* ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
* POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DZen.BasE91
{
    public static class BasE91
    {
        private static ReadOnlySpan<char> enctab => new char[] {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '!', '#', '$',
            '%', '&', '(', ')', '*', '+', ',', '.', '/', ':', ';', '<', '=',
            '>', '?', '@', '[', ']', '^', '_', '`', '{', '|', '}', '~', '"'
        };

        private static ReadOnlySpan<byte> dectab => new byte[]
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

        public static StringBuilder Encode(ReadOnlySpan<byte> ib)
        {
            if (ib == null)
                throw new ArgumentNullException(nameof(ib));
            int ebq = 0, en = 0;
            StringBuilder sb = new StringBuilder((int)(ib.Length * 1.23f));
            for (int i = 0; i < ib.Length; ++i)
            {
                ebq |= (ib[i] & 255) << en;
                en += 8;
                if (en > 13)
                {
                    int ev = ebq & 8191;

                    if (ev > 88)
                    {
                        ebq >>= 13;
                        en -= 13;
                    }
                    else
                    {
                        ev = ebq & 16383;
                        ebq >>= 14;
                        en -= 14;
                    }
                    sb.Append(enctab[ev % 91]);
                    sb.Append(enctab[ev / 91]);
                }
            }
            if (en > 0)
            {
                sb.Append(enctab[ebq % 91]);
                if (en > 7 || ebq > 90)
                    sb.Append(enctab[ebq / 91]);
            }
            return sb;
        }

        public static List<byte> Decode(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            int dbq = 0, dn = 0, dv = -1;
            List<byte> data = new List<byte>((int)(s.Length * (1f - 0.21)));
            for (int i = 0; i < s.Length; ++i)
            {
                if (dectab[s[i]] == 91)
                    continue;
                if (dv == -1)
                    dv = dectab[s[i]];
                else
                {
                    EncodeChar(s[i], ref dv, ref dbq, ref dn);
                    do
                    {
                        data.Add((byte)dbq);
                        dbq >>= 8;
                        dn -= 8;
                    } while (dn > 7);
                    dv = -1;
                }
            }
            if (dv != -1)
                data.Add((byte)(dbq | dv << dn));
            return data;
        }

        public static Span<byte> Decode(string s, Span<byte> bytes)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            int dbq = 0, dn = 0, dv = -1, decodeIndex = 0;
            for (int i = 0; i < s.Length; ++i)
            {
                if (dectab[s[i]] == 91)
                    continue;
                if (dv == -1)
                    dv = dectab[s[i]];
                else
                {
                    EncodeChar(s[i], ref dv, ref dbq, ref dn);
                    do
                    {
                        bytes[decodeIndex++] = (byte)dbq;
                        dbq >>= 8;
                        dn -= 8;
                    } while (dn > 7);
                    dv = -1;
                }
            }
            if (dv != -1)
                bytes[decodeIndex++] = (byte)(dbq | dv << dn);
            return bytes.Slice(0, decodeIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EncodeChar(char c, ref int dv, ref int dbq, ref int dn)
        {
            dv += dectab[c] * 91;
            dbq |= dv << dn;
            dn += (dv & 8191) > 88 ? 13 : 14;
        }
    }
}
