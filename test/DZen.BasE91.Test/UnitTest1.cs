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
                string enc = BasE91.Encode(raw).ToString();
                byte dec = BasE91.Decode(enc).First();
                Assert.Equal(raw[0], dec);
            }
        }

        [Fact]
        public void Roundtrip_ushort()
        {
            for (ushort i = 1; i < ushort.MaxValue; i++)
            {
                byte[] raw = BitConverter.GetBytes(i);
                string enc = BasE91.Encode(raw).ToString();
                List<byte> dec = BasE91.Decode(enc);
                Assert.Equal(raw[0], dec[0]);
                Assert.Equal(raw[1], dec[1]);

                Span<byte> decoded = new Span<byte>(new byte[raw.Length]);
                decoded = BasE91.Decode(enc, decoded);
                Assert.Equal(raw, decoded.ToArray());
            }
        }

		[Fact]
		public void Trims_Decoded()
		{
			byte[] data = new byte[100 * 1024];
			new Random().NextBytes(data);
			var encoded = BasE91.Encode(data).ToString();
			List<byte> newData = BasE91.Decode(encoded);
			
            Assert.Equal(data, newData);

			Span<byte> decoded = new Span<byte>(new byte[data.Length + 100]);
			decoded = BasE91.Decode(encoded, decoded);
			Assert.Equal(data.Length, decoded.Length);
			Assert.Equal(data, decoded.ToArray());
		}

		[Fact]
        public void Large_Array()
        {
            byte[] data = new byte[20 * 1024 * 1024];
            new Random().NextBytes(data);
            var encoded = BasE91.Encode(data).ToString();
            List<byte> newData = BasE91.Decode(encoded);
            for (int i = 0; i < data.Length; i++)
            {
                Assert.Equal(data[i], newData[i]);
            }

            Span<byte> decoded = new Span<byte>(new byte[data.Length]);
            decoded = BasE91.Decode(encoded, decoded);
            Assert.Equal(data, decoded.ToArray());
        }

		[Fact]
		public void DifferentSizes()
		{
            for (int i = 0; i < 10 * 1024; i++)
            {
                byte[] data = new byte[i];
                new Random().NextBytes(data);
                var encoded = BasE91.Encode(data).ToString();
                List<byte> newData = BasE91.Decode(encoded);
                
                Assert.Equal(data, newData);

                Span<byte> decoded = new Span<byte>(new byte[data.Length]);
                decoded = BasE91.Decode(encoded, decoded);
                Assert.Equal(data, decoded.ToArray());
            }
		}

		[Fact]
        public void Invalid_Data()
        {
            Assert.Throws<ArgumentNullException>(() => BasE91.Encode(null));
            Assert.Throws<ArgumentNullException>(() => BasE91.Decode(null));
        }
    }
}
