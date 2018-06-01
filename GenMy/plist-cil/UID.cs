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
using System.Text;

namespace PList {
	/// <summary>
	/// An UID. Only found in binary property lists that are keyed archives.
	/// </summary>
	/// @author Daniel Dreibrodt
	/// @author Natalia Portillo
	public class UID : NSObject {
		readonly byte[] bytes;
		readonly string name;

		/// <summary>
		/// Initializes a new instance of the <see cref="PList.UID"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="bytes">Bytes.</param>
		public UID(String name, byte[] bytes) {
			this.name = name;
			this.bytes = bytes;
		}

		/// <summary>
		/// Gets the bytes.
		/// </summary>
		/// <value>The bytes.</value>
		public byte[] Bytes {
			get {
				return bytes;
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name {
			get {
				return name;
			}
		}

		/// <summary>
		/// There is no XML representation specified for UIDs.
		/// In this implementation UIDs are represented as strings in the XML output.
		/// </summary>
		/// <param name="xml">The xml StringBuilder</param>
		/// <param name="level">The indentation level</param>
		internal override void ToXml(StringBuilder xml, int level) {
			Indent(xml, level);
			xml.Append("<string>");
			foreach (byte b in bytes)
				xml.Append(String.Format("{0:x2}", b));
			xml.Append("</string>");
		}

		internal override void ToBinary(BinaryPropertyListWriter outPlist) {
			outPlist.Write(0x80 + bytes.Length - 1);
			outPlist.Write(bytes);
		}

		internal override void ToASCII(StringBuilder ascii, int level) {
			Indent(ascii, level);
			ascii.Append("\"");
			foreach (byte b in bytes)
				ascii.Append(String.Format("{0:x2}", b));
			ascii.Append("\"");
		}

		internal override void ToASCIIGnuStep(StringBuilder ascii, int level) {
			ToASCII(ascii, level);
		}

		/// <summary>
		/// Determines whether the specified <see cref="PList.NSObject"/> is equal to the current <see cref="PList.UID"/>.
		/// </summary>
		/// <param name="obj">The <see cref="PList.NSObject"/> to compare with the current <see cref="PList.UID"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="PList.NSObject"/> is equal to the current
		/// <see cref="PList.UID"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(NSObject obj) {
			if (!(obj is UID))
				return false;

			if (((UID)obj).Name != name)
				return false;

			return ArrayEquals(((UID)obj).Bytes, bytes);
		}
	}
}

