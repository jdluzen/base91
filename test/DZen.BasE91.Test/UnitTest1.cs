using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DZen.BasE91.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Roundtrip_Byte()
        {
            for (byte i = 1; i < byte.MaxValue; i++)
            {
                byte[] raw = new byte[] { i };
                string enc = new BasE91().Encode(raw).ToString();
                byte dec = new BasE91().Decode(enc).First();
                Assert.Equal(raw[0], dec);
            }
        }

        [Fact]
        public void Roundtrip_ushort()
        {
            for (ushort i = 1; i < ushort.MaxValue; i++)
            {
                byte[] raw = BitConverter.GetBytes(i);
                string enc = new BasE91().Encode(raw).ToString();
                List<byte> dec = new BasE91().Decode(enc);
                Assert.Equal(raw[0], dec[0]);
                Assert.Equal(raw[1], dec[1]);
            }
        }

        [Fact]
        public void Large_Array()
        {
            byte[] data = new byte[20 * 1024 * 1024];
            new Random().NextBytes(data);
            var builder = new BasE91().Encode(data);
            List<byte> newData = new BasE91().Decode(builder.ToString());
            for (int i = 0; i < data.Length; i++)
            {
                Assert.Equal(data[i], newData[i]);
            }
        }

        [Fact]
        public void Invalid_Data()
        {
            Assert.Throws<ArgumentNullException>(() => new BasE91().Encode(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BasE91().Encode(new byte[1], 2));
            Assert.Throws<ArgumentNullException>(() => new BasE91().Decode(null));
        }
    }
}
