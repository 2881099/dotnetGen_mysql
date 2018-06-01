// plist-cil - An open source library to parse and generate property lists for .NET
// Copyright (C) 2015 Natalia Portillo
//
// This code is based on:
// plist - An open source library to parse and generate property lists
// Copyright (C) 2014 Daniel Dreibrodt
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;
using System.IO;
using System.Text;

namespace PList {
	/// <summary>
	/// NSData objects are wrappers for byte buffers
	/// </summary>
	/// @author Daniel Dreibrodt
	/// @author Natalia Portillo
	public class NSData : NSObject {
		readonly byte[] bytes;

		/// <summary>
		/// Creates the NSData object from the binary representation of it.
		/// </summary>
		/// <param name="bytes">The raw data contained in the NSData object.</param>
		public NSData(byte[] bytes) {
			this.bytes = bytes;
		}

		/// <summary>
		/// Creates a NSData object from its textual representation, which is a Base64 encoded amount of bytes.
		/// </summary>
		/// <param name="base64">The Base64 encoded contents of the NSData object.</param>
		/// <exception cref="FormatException">When the given string is not a proper Base64 formatted string.</exception>
		public NSData(string base64) {
			bytes = Convert.FromBase64String(base64);
		}

		/// <summary>
		/// Creates a NSData object from a file. Using the files contents as the contents of this NSData object.
		/// </summary>
		/// <param name="file">The file containing the data.</param>
		/// <exception cref="FileNotFoundException">If the file could not be found.</exception>
		/// <exception cref="IOException">If the file could not be read.</exception>
		public NSData(FileInfo file) {
			bytes = new byte[(int)file.Length];
			using (FileStream raf = file.OpenRead()) {
				raf.Read(bytes, 0, (int)file.Length);
			}
		}

		/// <summary>
		/// The bytes contained in this NSData object.
		/// </summary>
		/// <value>The data as bytes</value>
		public byte[] Bytes {
			get {
				return bytes;
			}
		}

		/// <summary>
		/// Gets the amount of data stored in this object.
		/// </summary>
		/// <value>The number of bytes contained in this object.</value>
		public int Length {
			get {
				return bytes.Length;
			}
		}

		/// <summary>
		/// Loads the bytes from this NSData object into a byte buffer.
		/// </summary>
		/// <param name="buf">The byte buffer which will contain the data</param>
		/// <param name="length">The amount of data to copy</param>
		public void GetBytes(MemoryStream buf, int length) {
			buf.Write(bytes, 0, Math.Min(bytes.Length, length));
		}

		/// <summary>
		/// Loads the bytes from this NSData object into a byte buffer.
		/// </summary>
		/// <param name="buf">The byte buffer which will contain the data</param>
		/// <param name="rangeStart">The start index.</param>
		/// <param name="rangeStop">The stop index.</param>
		public void GetBytes(MemoryStream buf, int rangeStart, int rangeStop) {
			buf.Write(bytes, rangeStart, Math.Min(bytes.Length, rangeStop));
		}

		/// <summary>
		/// Gets the Base64 encoded data contained in this NSData object.
		/// </summary>
		/// <returns>The Base64 encoded data as a <c>string</c>.</returns>
		public string GetBase64EncodedData() {
			return Convert.ToBase64String(bytes);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PList.NSData"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="PList.NSData"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="PList.NSData"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(Object obj) {
			return obj.GetType().Equals(GetType()) && ArrayEquals(((NSData)obj).bytes, bytes);
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="PList.NSData"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
		/// hash table.</returns>
		public override int GetHashCode() {
			int hash = 5;
			hash = 67 * hash + bytes.GetHashCode();
			return hash;
		}

		internal override void ToXml(StringBuilder xml, int level) {
			Indent(xml, level);
			xml.Append("<data>");
			xml.Append(NSObject.NEWLINE);
			string base64 = GetBase64EncodedData();
			foreach (string line in base64.Split('\n')) {
				Indent(xml, level);
				xml.Append(line);
				xml.Append(NSObject.NEWLINE);
			}
			Indent(xml, level);
			xml.Append("</data>");
		}

		internal override void ToBinary(BinaryPropertyListWriter outPlist) {
			outPlist.WriteIntHeader(0x4, bytes.Length);
			outPlist.Write(bytes);
		}

		internal override void ToASCII(StringBuilder ascii, int level) {
			Indent(ascii, level);
			ascii.Append(ASCIIPropertyListParser.DATA_BEGIN_TOKEN);
			int indexOfLastNewLine = ascii.ToString().LastIndexOf(NEWLINE, StringComparison.Ordinal);
			for (int i = 0; i < bytes.Length; i++) {
				int b = bytes[i] & 0xFF;
				ascii.Append(String.Format("{0:x2}", b));
				if (ascii.Length - indexOfLastNewLine > ASCII_LINE_LENGTH) {
					ascii.Append(NEWLINE);
					indexOfLastNewLine = ascii.Length;
				} else if ((i + 1) % 2 == 0 && i != bytes.Length - 1) {
					ascii.Append(" ");
				}
			}
			ascii.Append(ASCIIPropertyListParser.DATA_END_TOKEN);
		}

		internal override void ToASCIIGnuStep(StringBuilder ascii, int level) {
			ToASCII(ascii, level);
		}

		/// <summary>
		/// Determines whether the specified <see cref="PList.NSObject"/> is equal to the current <see cref="PList.NSData"/>.
		/// </summary>
		/// <param name="obj">The <see cref="PList.NSObject"/> to compare with the current <see cref="PList.NSData"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="PList.NSObject"/> is equal to the current
		/// <see cref="PList.NSData"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(NSObject obj) {
			if (!(obj is NSData))
				return false;

			return ArrayEquals(bytes, ((NSData)obj).Bytes);
		}
	}
}

