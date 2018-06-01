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
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PList {
	/// <summary>
	/// <para>
	/// Abstract interface for any object contained in a property list.
	/// </para><para>
	/// The names and functions of the various objects orient themselves
	/// towards Apple's Cocoa API.
	/// </para>
	/// </summary>
	/// @author Daniel Dreibrodt
	/// @author Natalia Portillo
	public abstract class NSObject {
		/// <summary>
		/// The newline character used for generating the XML output.
		/// To maintain compatibility with the Apple format, only a newline character
		/// is used (as opposed to cr+lf which is normally used on Windows).
		/// </summary>
		readonly internal static string NEWLINE = "\n";


		/// <summary>
		/// The identation character used for generating the XML output. This is the
		/// tabulator character.
		/// </summary>
		readonly static string INDENT = "\t";

		/// <summary>
		/// The maximum length of the text lines to be used when generating
		/// ASCII property lists. But this number is only a guideline it is not
		/// guaranteed that it will not be overstepped.
		/// </summary>
		internal readonly static int ASCII_LINE_LENGTH = 80;

		/// <summary>
		/// Generates the XML representation of the object (without XML headers or enclosing plist-tags).
		/// </summary>
		/// <param name="xml">The StringBuilder onto which the XML representation is appended.</param>
		/// <param name="level">The indentation level of the object.</param>
		internal abstract void ToXml(StringBuilder xml, int level);

		/// <summary>
		/// Assigns IDs to all the objects in this NSObject subtree.
		/// </summary>
		/// <param name="outPlist">The writer object that handles the binary serialization.</param>
		internal virtual void AssignIDs(BinaryPropertyListWriter outPlist) {
			outPlist.AssignID(this);
		}

		/// <summary>
		/// Generates the binary representation of the object.
		/// </summary>
		/// <param name="outPlist">The output stream to serialize the object to.</param>
		internal abstract void ToBinary(BinaryPropertyListWriter outPlist);

		/// <summary>
		/// Generates a valid XML property list including headers using this object as root.
		/// </summary>
		/// <returns>The XML representation of the property list including XML header and doctype information.</returns>
		public string ToXmlPropertyList() {
			StringBuilder xml = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			xml.Append(NSObject.NEWLINE);
			xml.Append("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
			xml.Append(NSObject.NEWLINE);
			xml.Append("<plist version=\"1.0\">");
			xml.Append(NSObject.NEWLINE);
			ToXml(xml, 0);
			xml.Append(NSObject.NEWLINE);
			xml.Append("</plist>");
			xml.Append(NSObject.NEWLINE);
			return xml.ToString();
		}

		/// <summary>
		/// Generates the ASCII representation of this object.
		/// The generated ASCII representation does not end with a newline.
		/// Complies with https://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/PropertyLists/OldStylePlists/OldStylePLists.html
		/// </summary>
		/// <param name="ascii">The StringBuilder onto which the ASCII representation is appended.</param>
		/// <param name="level">The indentation level of the object.</param>
		internal abstract void ToASCII(StringBuilder ascii, int level);

		/// <summary>
		/// Generates the ASCII representation of this object in the GnuStep format.
		/// The generated ASCII representation does not end with a newline.
		/// </summary>
		/// <param name="ascii">The StringBuilder onto which the ASCII representation is appended.</param>
		/// <param name="level">The indentation level of the object.</param>
		internal abstract void ToASCIIGnuStep(StringBuilder ascii, int level);

		/// <summary>
		/// Helper method that adds correct identation to the xml output.
		/// Calling this method will add <c>level</c> number of tab characters
		/// to the <c>xml</c> string.
		/// </summary>
		/// <param name="xml">The string builder for the XML document.</param>
		/// <param name="level">The level of identation.</param>
		internal static void Indent(StringBuilder xml, int level) {
			for (int i = 0; i < level; i++)
				xml.Append(INDENT);
		}

		/// <summary>
		/// Wraps the given value inside a NSObject.
		/// </summary>
		/// <param name="value">The value to represent as a NSObject.</param>
		/// <returns>A NSObject representing the given value.</returns>
		public static NSNumber Wrap(long value) {
			return new NSNumber(value);
		}

		/// <summary>
		/// Wraps the given value inside a NSObject.
		/// </summary>
		/// <param name="value">The value to represent as a NSObject.</param>
		/// <returns>A NSObject representing the given value.</returns>
		public static NSNumber Wrap(double value) {
			return new NSNumber(value);
		}

		/// <summary>
		/// Wraps the given value inside a NSObject.
		/// </summary>
		/// <param name="value">The value to represent as a NSObject.</param>
		/// <returns>A NSObject representing the given value.</returns>
		public static NSNumber Wrap(bool value) {
			return new NSNumber(value);
		}

		/// <summary>
		/// Wraps the given value inside a NSObject.
		/// </summary>
		/// <param name="value">The value to represent as a NSObject.</param>
		/// <returns>A NSObject representing the given value.</returns>
		public static NSData Wrap(byte[] value) {
			return new NSData(value);
		}

		/// <summary>
		/// Creates a NSArray with the contents of the given array.
		/// </summary>
		/// <param name="value">The value to represent as a NSObject.</param>
		/// <returns>A NSObject representing the given value.</returns>
		/// <exception cref="SystemException">When one of the objects contained in the array cannot be represented by a NSObject.</exception>
		public static NSArray Wrap(Object[] value) {
			NSArray arr = new NSArray(value.Length);
			for (int i = 0; i < value.Length; i++) {
				arr.Add(Wrap(value[i]));
			}
			return arr;
		}

		/// <summary>
		/// Creates a NSDictionary with the contents of the given map.
		/// </summary>
		/// <param name="value">The value to represent as a NSObject.</param>
		/// <returns>A NSObject representing the given value.</returns>
		/// <exception cref="SystemException">When one of the values contained in the map cannot be represented by a NSObject.</exception>
		public static NSDictionary Wrap(Dictionary<string, Object> value) {
			NSDictionary dict = new NSDictionary();
			foreach (KeyValuePair<string, Object> kvp in value)
				dict.Add(kvp.Key, Wrap(kvp.Value));
			return dict;
		}

		/// <summary>
		/// Creates a NSSet with the contents of this set.
		/// </summary>
		/// <param name="value">The value to represent as a NSObject.</param>
		/// <returns>A NSObject representing the given value.</returns>
		/// <exception cref="SystemException">When one of the values contained in the map cannot be represented by a NSObject.</exception>
		public static NSSet Wrap(List<Object> value) {
			NSSet set = new NSSet();
			foreach (Object o in value)
				set.AddObject(Wrap(o));
			return set;
		}

		/// <summary>
		/// <para>
		/// Creates a NSObject representing the given .NET Object.
		/// </para><para>
		/// Numerics of type <see cref="bool"/>, <see cref="int"/>, <see cref="long"/>, <see cref="short"/>, <see cref="byte"/>, <see cref="float"/> or <see cref="double"/> are wrapped as NSNumber objects.
		/// </para><para>
		/// Strings are wrapped as <see cref="NSString"/> objects and byte arrays as <see cref="NSData"/> objects.
		/// </para><para>
		/// DateTime objects are wrapped as <see cref="NSDate"/> objects.
		/// </para><para>
		/// Serializable classes are serialized and their data is stored in <see cref="NSData"/> objects.
		/// </para><para>
		/// Arrays and Collection objects are converted to <see cref="NSArray"/> where each array member is wrapped into a <see cref="NSObject"/>.
		/// </para><para>
		/// Dictionaries are converted to <see cref="NSDictionary"/>. Each key is converted to a string and each value wrapped into a <see cref="NSObject"/>.
		/// </para>
		/// </summary>
		/// <param name="o">The object to represent.</param>
		///<returns>A NSObject equivalent to the given object.</returns>
		public static NSObject Wrap(Object o) {
			if (o == null)
				throw new NullReferenceException("A null object cannot be wrapped as a NSObject");

			if (o is NSObject)
				return (NSObject)o;

			Type c = o.GetType();
			if (typeof(bool).Equals(c)) {
				return Wrap((bool)o);
			}
			if (typeof(Byte).Equals(c)) {
				return Wrap((int)(Byte)o);
			}
			if (typeof(short).Equals(c)) {
				return Wrap((int)(short)o);
			}
			if (typeof(int).Equals(c)) {
				return Wrap((int)(int)o);
			}
			if (typeof(long).IsAssignableFrom(c)) {
				return Wrap((long)o);
			}
			if (typeof(float).Equals(c)) {
				return Wrap((double)(float)o);
			}
			if (typeof(double).IsAssignableFrom(c)) {
				return Wrap((double)o);
			}
			if (typeof(string).Equals(c)) {
				return new NSString((string)o);
			}
			if (typeof(DateTime).Equals(c)) {
				return new NSDate((DateTime)o);
			}
			if (c.IsArray) {
				Type cc = c.GetElementType();
				if (cc.Equals(typeof(byte))) {
					return Wrap((byte[])o);
				}
				if (cc.Equals(typeof(bool))) {
					bool[] array = (bool[])o;
					NSArray nsa = new NSArray(array.Length);
					for (int i = 0; i < array.Length; i++)
						nsa.Add(Wrap(array[i]));
					return nsa;
				}
				if (cc.Equals(typeof(float))) {
					float[] array = (float[])o;
					NSArray nsa = new NSArray(array.Length);
					for (int i = 0; i < array.Length; i++)
						nsa.Add(Wrap(array[i]));
					return nsa;
				}
				if (cc.Equals(typeof(double))) {
					double[] array = (double[])o;
					NSArray nsa = new NSArray(array.Length);
					for (int i = 0; i < array.Length; i++)
						nsa.Add(Wrap(array[i]));
					return nsa;
				}
				if (cc.Equals(typeof(short))) {
					short[] array = (short[])o;
					NSArray nsa = new NSArray(array.Length);
					for (int i = 0; i < array.Length; i++)
						nsa.Add(Wrap(array[i]));
					return nsa;
				}
				if (cc.Equals(typeof(int))) {
					int[] array = (int[])o;
					NSArray nsa = new NSArray(array.Length);
					for (int i = 0; i < array.Length; i++)
						nsa.Add(Wrap(array[i]));
					return nsa;
				}
				if (cc.Equals(typeof(long))) {
					long[] array = (long[])o;
					NSArray nsa = new NSArray(array.Length);
					for (int i = 0; i < array.Length; i++)
						nsa.Add(Wrap(array[i]));
					return nsa;
				}
				return Wrap((Object[])o);
			}
			if (typeof(Dictionary<string, Object>).IsAssignableFrom(c)) {
				Dictionary<string, Object> netDict = (Dictionary<string, Object>)o;
				NSDictionary dict = new NSDictionary();
				foreach (KeyValuePair<string, Object> kvp in netDict) {
					dict.Add(kvp.Key, Wrap(kvp.Value));
				}
				return dict;
			}
			if (typeof(List<Object>).IsAssignableFrom(c))
				return Wrap(((List<Object>)o).ToArray());

			throw new PropertyListException(string.Format("Cannot wrap an object of type {0}.", o.GetType().Name));
		}

		/// <summary>
		/// Converts this NSObject into an equivalent object
		/// of the .NET Runtime Environment.
		/// <para><see cref="NSArray"/> objects are converted to arrays.</para>
		/// <para><see cref="NSDictionary"/> objects are converted to objects extending the <see cref="Dictionary{TKey, TValue}"/> class.</para>
		/// <para><see cref="NSSet"/> objects are converted to objects extending the <see cref="List{NSObject}"/> class.</para>
		/// <para><see cref="NSNumber"/> objects are converted to primitive number values (<see cref="int"/>, <see cref="long"/>, <see cref="double"/> or <see cref="bool"/>).</para>
		/// <para><see cref="NSString"/> objects are converted to <see cref="string"/> objects.</para>
		/// <para><see cref="NSData"/> objects are converted to <see cref="byte"/> arrays.</para>
		/// <para><see cref="NSDate"/> objects are converted to <see cref="System.DateTime"/> objects.</para>
		/// <para><see cref="UID"/> objects are converted to <see cref="byte"/> arrays.</para>
		/// </summary>
		/// <returns>A native .NET object representing this NSObject's value.</returns>
		public Object ToObject() {
			if (this is NSArray) {
				NSObject[] arrayA = ((NSArray)this).GetArray();
				Object[] arrayB = new Object[arrayA.Length];
				for (int i = 0; i < arrayA.Length; i++) {
					arrayB[i] = arrayA[i].ToObject();
				}
				return arrayB;
			}
			if (this is NSDictionary) {
				Dictionary<string, NSObject> dictA = ((NSDictionary)this).GetDictionary();
				Dictionary<string, Object> dictB = new Dictionary<string, Object>(dictA.Count);
				foreach (KeyValuePair<string, NSObject> kvp in dictA) {
					dictB.Add(kvp.Key, kvp.Value.ToObject());
				}
				return dictB;
			}
			if (this is NSSet) {
				List<NSObject> setA = ((NSSet)this).GetSet();
				List<Object> setB = new List<Object>();
				foreach (NSObject o in setA) {
					setB.Add(o.ToObject());
				}
				return setB;
			}
			if (this is NSNumber) {
				NSNumber num = (NSNumber)this;
				switch (num.GetNSNumberType()) {
					case NSNumber.INTEGER: {
							long longVal = num.ToLong();
							if (longVal > int.MaxValue || longVal < int.MinValue)
								return longVal;
							return num.ToInt();
						}
					case NSNumber.REAL:
						return num.ToDouble();
					case NSNumber.BOOLEAN:
						return num.ToBool();
					default:
						return num.ToDouble();
				}
			}
			if (this is NSString) {
				return ((NSString)this).GetContent();
			}
			if (this is NSData) {
				return ((NSData)this).Bytes;
			}
			if (this is NSDate) {
				return ((NSDate)this).Date;
			}
			if (this is UID) {
				return ((UID)this).Bytes;
			}
			return this;
		}

		internal static bool ArrayEquals(byte[] arrayA, byte[] arrayB) {
			if (arrayA.Length == arrayB.Length) {
				for (int i = 0; i < arrayA.Length; i++)
					if (arrayA[i] != arrayB[i])
						return false;
				return true;
			}
			return false;
		}

		internal static bool ArrayEquals(NSObject[] arrayA, NSObject[] arrayB) {
			if (arrayA.Length == arrayB.Length) {
				for (int i = 0; i < arrayA.Length; i++) {
					if (arrayA[i] != arrayB[i]) {
						return false;
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determines if the specific NSObject is the same as the NSObject overriding this method.
		/// </summary>
		/// <param name="obj">The <see cref="PList.NSObject"/> to compare with the current <see cref="PList.NSObject"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="PList.NSObject"/> is equal to the current
		/// <see cref="PList.NSObject"/>; otherwise, <c>false</c>.</returns>
		public abstract bool Equals(NSObject obj);
	}
}

