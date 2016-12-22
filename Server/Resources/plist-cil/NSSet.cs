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
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace PList {
	/// <summary>
	/// <para>
	/// A set is an interface to an unordered collection of objects.
	/// </para><para>
	/// This implementation uses a <see cref="List{NSObject}"/>as the underlying
	/// data structure.
	/// </para>
	/// </summary>
	/// @author Daniel Dreibrodt
	/// @author Natalia Portillo
	public class NSSet : NSObject, IEnumerable {
		readonly List<NSObject> set;

		bool ordered;

		/// <summary>
		/// Creates an empty unordered set.
		/// </summary>
		public NSSet() {
			set = new List<NSObject>();
		}

		/// <summary>
		/// Creates an empty set.
		/// </summary>
		/// <param name="ordered">Should the set be ordered on operations?</param>
		public NSSet(bool ordered) {
			this.ordered = ordered;
			set = new List<NSObject>();
		}


		/// <summary>
		/// Creates a set and fill it with the given objects.
		/// </summary>
		/// <param name="objects">The objects to populate the set.</param>
		public NSSet(params NSObject[] objects) {
			set = new List<NSObject>(objects);
		}

		/// <summary>
		/// Creates a set and fill it with the given objects.
		/// </summary>
		/// <param name="objects">The objects to populate the set.</param>
		/// <param name="ordered">Should the set be ordered on operations?</param>
		public NSSet(bool ordered, params NSObject[] objects) {
			this.ordered = ordered;
			set = new List<NSObject>(objects);
			if (ordered)
				set.Sort();
		}

		/// <summary>
		/// Adds an object to the set.
		/// </summary>
		/// <param name="obj">The object to add.</param>
		public void AddObject(NSObject obj) {
			lock (set) {
				set.Add(obj);
				if (ordered)
					set.Sort();
			}
		}

		/// <summary>
		/// Removes an object from the set.
		/// </summary>
		/// <param name="obj">The object to remove.</param>
		public void RemoveObject(NSObject obj) {
			lock (set) {
				set.Remove(obj);
				if (ordered)
					set.Sort();
			}
		}

		/// <summary>
		/// Returns all objects contained in the set.
		/// </summary>
		/// <returns>An array of all objects in the set.</returns>
		public NSObject[] AllObjects() {
			lock (set) {
				return set.ToArray();
			}
		}

		/// <summary>
		/// Returns one of the objects in the set, or <c>null</c>
		/// if the set contains no objects.
		/// </summary>
		/// <returns>The first object in the set, or <c>null</c> if the set is empty.</returns>
		public NSObject AnyObject() {
			lock (set) {
				return set.Count == 0 ? null : set[0];
			}
		}

		/// <summary>
		/// Finds out whether a given object is contained in the set.
		/// </summary>
		/// <returns><c>true</c>, when the object was found, <c>false</c> otherwise.</returns>
		/// <param name="obj">The object to look for.</param>
		public bool ContainsObject(NSObject obj) {
			return set.Contains(obj);
		}

		/// <summary>
		/// Determines whether the set contains an object equal to a given object
		/// and returns that object if it is present.
		/// </summary>
		/// <param name="obj">The object to look for.</param>
		/// <returns>The object if it is present, <c>null</c> otherwise.</returns>
		public NSObject Member(NSObject obj) {
			lock (set) {
				foreach (NSObject o in set) {
					if (o.Equals(obj))
						return o;
				}
				return null;
			}
		}

		/// <summary>
		/// Finds out whether at least one object is present in both sets.
		/// </summary>
		/// <returns><c>true</c> if the intersection of both sets is empty, <c>false</c> otherwise.</returns>
		/// <param name="otherSet">The other set.</param>
		public bool IntersectsSet(NSSet otherSet) {
			lock (set) {
				foreach (NSObject o in set) {
					if (otherSet.ContainsObject(o))
						return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Finds out if this set is a subset of the given set.
		/// </summary>
		/// <returns><c>true</c> if all elements in this set are also present in the other set, <c>false</c>otherwise.</returns>
		/// <param name="otherSet">The other set.</param>
		public bool IsSubsetOfSet(NSSet otherSet) {
			lock (set) {
				foreach (NSObject o in set) {
					if (!otherSet.ContainsObject(o))
						return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Returns an enumerator object that lets you iterate over all elements of the set.
		/// This is the equivalent to <c>objectEnumerator</c> in the Cocoa implementation
		/// of NSSet.
		/// </summary>
		/// <returns>The iterator for the set.</returns>
		public IEnumerator GetEnumerator() {
			lock (set) {
				return set.GetEnumerator();
			}
		}

		/// <summary>
		/// Gets the underlying data structure in which this NSSets stores its content.
		/// </summary>
		/// <returns>A Set object.</returns>
		internal List<NSObject> GetSet() {
			return set;
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="PList.NSSet"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
		/// hash table.</returns>
		public override int GetHashCode() {
			int hash = 7;
			hash = 29 * hash + (set != null ? set.GetHashCode() : 0);
			return hash;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PList.NSSet"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="PList.NSSet"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="PList.NSSet"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(Object obj) {
			if (obj == null) {
				return false;
			}
			if (GetType() != obj.GetType()) {
				return false;
			}
			NSSet other = (NSSet)obj;
			return !(set != other.set && (set == null || !set.Equals(other.set)));
		}

		/// <summary>
		/// Gets the number of elements in the set.
		/// </summary>
		/// <value>The number of elements in the set.</value>
		public int Count {
			get {
				lock (set) {
					return set.Count;
				}
			}
		}

		/// <summary>
		/// Returns the XML representantion for this set.
		/// There is no official XML representation specified for sets.
		/// In this implementation it is represented by an array.
		/// </summary>
		/// <param name="xml">The XML StringBuilder</param>
		/// <param name="level">The indentation level</param>
		internal override void ToXml(StringBuilder xml, int level) {
			Indent(xml, level);
			xml.Append("<array>");
			xml.Append(NSObject.NEWLINE);
			if (ordered)
				set.Sort();
			foreach (NSObject o in set) {
				o.ToXml(xml, level + 1);
				xml.Append(NSObject.NEWLINE);
			}
			Indent(xml, level);
			xml.Append("</array>");
		}

		internal override void AssignIDs(BinaryPropertyListWriter outPlist) {
			base.AssignIDs(outPlist);
			foreach (NSObject obj in set) {
				obj.AssignIDs(outPlist);
			}
		}

		internal override void ToBinary(BinaryPropertyListWriter outPlist) {
			if (ordered) {
				set.Sort();
				outPlist.WriteIntHeader(0xB, set.Count);
			} else {
				outPlist.WriteIntHeader(0xC, set.Count);
			}
			foreach (NSObject obj in set) {
				outPlist.WriteID(outPlist.GetID(obj));
			}
		}

		/// <summary>
		/// Returns the ASCII representation of this set.
		/// There is no official ASCII representation for sets.
		/// In this implementation sets are represented as arrays.
		/// </summary>
		/// <param name="ascii">The ASCII file string builder</param>
		/// <param name="level">The indentation level</param>
		internal override void ToASCII(StringBuilder ascii, int level) {
			Indent(ascii, level);
			if (ordered)
				set.Sort();
			NSObject[] array = AllObjects();
			ascii.Append(ASCIIPropertyListParser.ARRAY_BEGIN_TOKEN);
			int indexOfLastNewLine = ascii.ToString().LastIndexOf(NEWLINE, StringComparison.Ordinal);
			for (int i = 0; i < array.Length; i++) {
				Type objClass = array[i].GetType();
				if ((objClass.Equals(typeof(NSDictionary)) || objClass.Equals(typeof(NSArray)) || objClass.Equals(typeof(NSData)))
					&& indexOfLastNewLine != ascii.Length) {
					ascii.Append(NEWLINE);
					indexOfLastNewLine = ascii.Length;
					array[i].ToASCII(ascii, level + 1);
				} else {
					if (i != 0)
						ascii.Append(" ");
					array[i].ToASCII(ascii, 0);
				}

				if (i != array.Length - 1)
					ascii.Append(ASCIIPropertyListParser.ARRAY_ITEM_DELIMITER_TOKEN);

				if (ascii.Length - indexOfLastNewLine > ASCII_LINE_LENGTH) {
					ascii.Append(NEWLINE);
					indexOfLastNewLine = ascii.Length;
				}
			}
			ascii.Append(ASCIIPropertyListParser.ARRAY_END_TOKEN);
		}

		/// <summary>
		/// Returns the ASCII representation of this set according to the GnuStep format.
		/// There is no official ASCII representation for sets.
		/// In this implementation sets are represented as arrays.
		/// </summary>
		/// <param name="ascii">The ASCII file string builder</param>
		/// <param name="level">The indentation level</param>
		internal override void ToASCIIGnuStep(StringBuilder ascii, int level) {
			Indent(ascii, level);
			if (ordered)
				set.Sort();
			NSObject[] array = AllObjects();
			ascii.Append(ASCIIPropertyListParser.ARRAY_BEGIN_TOKEN);
			int indexOfLastNewLine = ascii.ToString().LastIndexOf(NEWLINE, StringComparison.Ordinal);
			for (int i = 0; i < array.Length; i++) {
				Type objClass = array[i].GetType();
				if ((objClass.Equals(typeof(NSDictionary)) || objClass.Equals(typeof(NSArray)) || objClass.Equals(typeof(NSData)))
					&& indexOfLastNewLine != ascii.Length) {
					ascii.Append(NEWLINE);
					indexOfLastNewLine = ascii.Length;
					array[i].ToASCIIGnuStep(ascii, level + 1);
				} else {
					if (i != 0)
						ascii.Append(" ");
					array[i].ToASCIIGnuStep(ascii, 0);
				}

				if (i != array.Length - 1)
					ascii.Append(ASCIIPropertyListParser.ARRAY_ITEM_DELIMITER_TOKEN);

				if (ascii.Length - indexOfLastNewLine > ASCII_LINE_LENGTH) {
					ascii.Append(NEWLINE);
					indexOfLastNewLine = ascii.Length;
				}
			}
			ascii.Append(ASCIIPropertyListParser.ARRAY_END_TOKEN);
		}

		/// <summary>
		/// Determines whether the specified <see cref="PList.NSObject"/> is equal to the current <see cref="PList.NSSet"/>.
		/// </summary>
		/// <param name="obj">The <see cref="PList.NSObject"/> to compare with the current <see cref="PList.NSSet"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="PList.NSObject"/> is equal to the current
		/// <see cref="PList.NSSet"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(NSObject obj) {
			if (!(obj is NSSet))
				return false;

			if (set.Count != ((NSSet)obj).Count)
				return false;

			foreach (NSObject objS in (NSSet)obj)
				if (!set.Contains(objS))
					return false;

			return true;
		}
	}
}

