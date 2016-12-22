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
	/// Represents a date
	/// </summary>
	/// @author Daniel Dreibrodt
	/// @author Natalia Portillo
	public class NSDate : NSObject {
		readonly DateTime date;

		static readonly DateTime EPOCH = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		// The datetime ends with 'Z', which indicates UTC time. To make sure .NET
		// understands the 'Z' character as a timezone, specify the 'K' format string.
		static readonly string sdfDefault = "yyyy-MM-dd'T'HH:mm:ssK";
		static readonly string sdfGnuStep = "yyyy-MM-dd HH:mm:ss zzz";

		static readonly System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;

		/// <summary>
		/// Parses the XML date string and creates a .NET DateTime object from it.
		/// </summary>
		/// <returns>The parsed Date</returns>
		/// <param name="textRepresentation">The date string as found in the XML property list</param>
		/// <exception cref="FormatException">Given string cannot be parsed</exception>
		static DateTime ParseDateString(string textRepresentation) {
			try {
				return DateTime.ParseExact(textRepresentation, sdfDefault, provider);
			} catch (FormatException) {
				return DateTime.ParseExact(textRepresentation, sdfGnuStep, provider);
			}
		}

		/// <summary>
		/// Generates a String representation of a .NET DateTime object. The string
		/// is formatted according to the specification for XML property list dates.
		/// </summary>
		/// <param name="date">The date which should be represented.</param>
		/// <returns>The string representation of the date.</returns>
		public static string MakeDateString(DateTime date) {
			return date.ToUniversalTime().ToString(sdfDefault);
		}

		/// <summary>
		/// Generates a String representation of a .NET DateTime object. The string
		/// is formatted according to the specification for GnuStep ASCII property
		/// list dates.
		/// </summary>
		/// <param name="date">The date which should be represented.</param>
		/// <returns>The string representation of the date.</returns>
		static string MakeDateStringGnuStep(DateTime date) {
			return date.ToString(sdfGnuStep);
		}

		/// <summary>
		/// Creates a date from its binary representation.
		/// </summary>
		/// <param name="bytes">bytes The date bytes</param>
		public NSDate(byte[] bytes) {
			//dates are 8 byte big-endian double, seconds since the epoch
			date = EPOCH.AddSeconds(BinaryPropertyListParser.ParseDouble(bytes));
		}

		/// <summary>
		/// Parses a date from its textual representation.
		/// That representation has the following pattern: <code>yyyy-MM-dd'T'HH:mm:ss'Z'</code>
		/// </summary>
		/// <param name="textRepresentation">The textual representation of the date (ISO 8601 format)</param>
		/// <exception cref="FormatException">When the date could not be parsed, i.e. it does not match the expected pattern.</exception>
		public NSDate(String textRepresentation) {
			date = ParseDateString(textRepresentation);
		}

		/// <summary>
		/// Creates a NSDate from a .NET DateTime
		/// </summary>
		/// <param name="d">The date</param>
		public NSDate(DateTime d) {
			date = d;
		}

		/// <summary>
		/// Gets the date.
		/// </summary>
		/// <returns>The date.</returns>
		public DateTime Date {
			get {
				return date;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PList.NSDate"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="PList.NSDate"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="PList.NSDate"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(Object obj) {
			return obj.GetType().Equals(GetType()) && date.Equals(((NSDate)obj).Date);
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="PList.NSDate"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
		/// hash table.</returns>
		public override int GetHashCode() {
			return date.GetHashCode();
		}

		internal override void ToXml(StringBuilder xml, int level) {
			Indent(xml, level);
			xml.Append("<date>");
			xml.Append(MakeDateString(date));
			xml.Append("</date>");
		}

		internal override void ToBinary(BinaryPropertyListWriter outPlist) {
			outPlist.Write(0x33);
			outPlist.WriteDouble((date - EPOCH).TotalSeconds);
		}

		/// <summary>
		/// Generates a string representation of the date.
		/// </summary>
		/// <returns>A string representation of the date.</returns>
		public override String ToString() {
			return date.ToString();
		}

		internal override void ToASCII(StringBuilder ascii, int level) {
			Indent(ascii, level);
			ascii.Append("\"");
			ascii.Append(MakeDateString(date));
			ascii.Append("\"");
		}

		internal override void ToASCIIGnuStep(StringBuilder ascii, int level) {
			Indent(ascii, level);
			ascii.Append("<*D");
			ascii.Append(MakeDateStringGnuStep(date));
			ascii.Append(">");
		}

		/// <summary>
		/// Determines whether the specified <see cref="PList.NSObject"/> is equal to the current <see cref="PList.NSDate"/>.
		/// </summary>
		/// <param name="obj">The <see cref="PList.NSObject"/> to compare with the current <see cref="PList.NSDate"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="PList.NSObject"/> is equal to the current
		/// <see cref="PList.NSDate"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(NSObject obj) {
			if (!(obj is NSDate))
				return false;

			int equality = DateTime.Compare(date, ((NSDate)obj).Date);

			return equality == 0;
		}
	}
}

