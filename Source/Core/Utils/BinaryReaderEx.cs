using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Core.Utils
{
	/// <summary>
	/// Extends the <c>System.IO.BinaryReader</c> class by a <c>ReadToEnd</c>
	/// method that can be used to read a whole file.
	/// </summary>
	/// <remarks>
	/// http://dotnet.mvps.org/dotnet/faqs/?id=readfile&amp;lang=en
	/// </remarks>
	public class BinaryReaderEx : BinaryReader
	{
		/// <summary>
		/// Creates a new instance of the <c>ExtendedBinaryReader</c> class.
		/// </summary>
		/// <param name="Input">A stream.</param>
		public BinaryReaderEx(Stream Input)
			: base(Input)
		{
		}

		/// <summary>
		/// Creates a new instance of the <c>ExtendedBinaryReader</c> class.
		/// </summary>
		/// <param name="Input">The provided stream.</param>
		/// <param name="Encoding">The character encoding.</param>
		public BinaryReaderEx(Stream Input, Encoding Encoding)
			: base(Input, Encoding)
		{
		}

		/// <summary>
		/// Reads the whole data in the base stream and returns it in an
		/// array of bytes.
		/// </summary>
		/// <returns>The streams whole binary data.</returns>
		public byte[] ReadToEnd()
		{
			return this.ReadToEnd(short.MaxValue);
		}

		public byte[] ReadToEnd(int initialLength)
		{
			int readed = 0;
			if (initialLength < 1)
			{
				initialLength = short.MaxValue;
			}
			byte[] buffer = new byte[(initialLength - 1) + 1];
			for (int i = this.BaseStream.Read(buffer, readed, buffer.Length - readed); i > 0; i = this.BaseStream.Read(buffer, readed, buffer.Length - readed))
			{
				readed += i;
				if (readed == buffer.Length)
				{
					int nextByte = this.BaseStream.ReadByte();
					if (nextByte == -1)
					{
						return buffer;
					}
					byte[] newBuff = new byte[buffer.Length * 2];
					Buffer.BlockCopy(buffer, 0, newBuff, 0, buffer.Length);
					newBuff[readed] = (byte)nextByte;
					buffer = newBuff;
					readed++;
				}
			}
			byte[] dst = new byte[(readed - 1) + 1];
			Buffer.BlockCopy(buffer, 0, dst, 0, readed);
			return dst;
		}
	}
}
