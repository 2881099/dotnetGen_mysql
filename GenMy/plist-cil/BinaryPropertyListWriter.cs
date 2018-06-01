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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PList {
	/// <summary>
	/// <para>
	/// A BinaryPropertyListWriter is a helper class for writing out
	/// binary property list files.
	/// </para><para>
	/// It contains an output stream and various structures for keeping track
	/// of which NSObjects have already been serialized, and where they were
	/// put in the file.
	/// </para>
	/// </summary>
	/// @author Keith Randall
	/// @author Natalia Portillo
	public class BinaryPropertyListWriter {
		/// <summary>
		/// Binary property list version 0.0
		/// </summary>
		public const int VERSION_00 = 0;
		/// <summary>
		/// Binary property list version 1.0
		/// </summary>
		public const int VERSION_10 = 10;
		/// <summary>
		/// Binary property list version 1.5
		/// </summary>
		public const int VERSION_15 = 15;
		/// <summary>
		/// Binary property list version 2.0
		/// </summary>
		public const int VERSION_20 = 20;

		/// <summary>
		/// Finds out the minimum binary property list format version that
		/// can be used to save the given NSObject tree.
		/// </summary>
		/// <returns>Version code</returns>
		/// <param name="root">Object root.</param>
		static int GetMinimumRequiredVersion(NSObject root) {
			int minVersion = VERSION_00;
			if (root == null) {
				minVersion = VERSION_10;
			}
			if (root is NSDictionary) {
				NSDictionary dict = (NSDictionary)root;
				foreach (NSObject o in dict.GetDictionary().Values) {
					int v = GetMinimumRequiredVersion(o);
					if (v > minVersion)
						minVersion = v;
				}
			} else if (root is NSArray) {
				NSArray array = (NSArray)root;
				foreach (NSObject o in array.GetArray()) {
					int v = GetMinimumRequiredVersion(o);
					if (v > minVersion)
						minVersion = v;
				}
			} else if (root is NSSet) {
				//Sets are only allowed in property lists v1+
				minVersion = VERSION_10;
				NSSet set = (NSSet)root;
				foreach (NSObject o in set.AllObjects()) {
					int v = GetMinimumRequiredVersion(o);
					if (v > minVersion)
						minVersion = v;
				}
			}
			return minVersion;
		}

		/// <summary>
		/// Writes a binary plist file with the given object as the root.
		/// </summary>
		/// <param name="file">the file to write to</param>
		/// <param name="root">the source of the data to write to the file</param>
		/// <exception cref="IOException"></exception>
		public static void Write(FileInfo file, NSObject root) {
			using (FileStream fous = file.OpenWrite()) {
				Write(fous, root);
			}
		}

		/// <summary>
		/// Writes a binary plist serialization of the given object as the root.
		/// </summary>
		/// <param name="outStream">the stream to write to</param>
		/// <param name="root">the source of the data to write to the stream</param>
		/// <exception cref="IOException"></exception>
		public static void Write(Stream outStream, NSObject root) {
			int minVersion = GetMinimumRequiredVersion(root);
			if (minVersion > VERSION_00) {
				string versionString = ((minVersion == VERSION_10) ? "v1.0" : ((minVersion == VERSION_15) ? "v1.5" : ((minVersion == VERSION_20) ? "v2.0" : "v0.0")));
				throw new IOException("The given property list structure cannot be saved. " +
				"The required version of the binary format (" + versionString + ") is not yet supported.");
			}
			BinaryPropertyListWriter w = new BinaryPropertyListWriter(outStream, minVersion);
			w.Write(root);
		}

		/// <summary>
		/// Writes a binary plist serialization of the given object as the root
		/// into a byte array.
		/// </summary>
		/// <returns>The byte array containing the serialized property list</returns>
		/// <param name="root">The root object of the property list</param>
		/// <exception cref="IOException"></exception>
		public static byte[] WriteToArray(NSObject root) {
			MemoryStream bout = new MemoryStream();
			Write(bout, root);
			return bout.ToArray();
		}

		int version = VERSION_00;

		// raw output stream to result file
		Stream outStream;

		// # of bytes written so far
		long count;

		// map from object to its ID
		Collection<NSObject> idMap = new Collection<NSObject>(); //(new IdentityEqualityComparer<NSObject>());
		int idSizeInBytes;

		/// <summary>
		/// Creates a new binary property list writer
		/// </summary>
		/// <param name="outStr">The output stream into which the binary property list will be written</param>
		/// <exception cref="IOException">If an error occured while writing to the stream</exception>
		BinaryPropertyListWriter(Stream outStr) {
			outStream = outStr;
		}

		BinaryPropertyListWriter(Stream outStr, int version) {
			this.version = version;
			outStream = outStr;
		}

		void Write(NSObject root) {
			// magic bytes
			Write(new[] { (byte)'b', (byte)'p', (byte)'l', (byte)'i', (byte)'s', (byte)'t' });

			//version
			switch (version) {
				case VERSION_00: {
						Write(new[] { (byte)'0', (byte)'0' });
						break;
					}
				case VERSION_10: {
						Write(new[] { (byte)'1', (byte)'0' });
						break;
					}
				case VERSION_15: {
						Write(new[] { (byte)'1', (byte)'5' });
						break;
					}
				case VERSION_20: {
						Write(new[] { (byte)'2', (byte)'0' });
						break;
					}
			}

			// assign IDs to all the objects.
			root.AssignIDs(this);

			idSizeInBytes = ComputeIdSizeInBytes(idMap.Count);

			// offsets of each object, indexed by ID
			long[] offsets = new long[idMap.Count];

			// write each object, save offset
			for (int i = 0; i < idMap.Count; i++) {
				NSObject obj = idMap[i];
				int id = i;
				offsets[id] = count;
				if (obj == null) {
					Write(0x00);
				} else {
					obj.ToBinary(this);
				}
			}

			// write offset table
			long offsetTableOffset = count;
			int offsetSizeInBytes = ComputeOffsetSizeInBytes(count);
			foreach (long offset in offsets) {
				WriteBytes(offset, offsetSizeInBytes);
			}

			if (version != VERSION_15) {
				// write trailer
				// 6 null bytes
				Write(new byte[6]);
				// size of an offset
				Write(offsetSizeInBytes);
				// size of a ref
				Write(idSizeInBytes);
				// number of objects
				WriteLong(idMap.Count);
				// top object
				int rootID = idMap.IndexOf(root);
				WriteLong(rootID);
				// offset table offset
				WriteLong(offsetTableOffset);
			}

			outStream.Flush();
		}

		internal void AssignID(NSObject obj) {
			if (obj is UID || obj is NSArray) {
				idMap.Add(obj);
			} else if (!idMap.Contains(obj)) {
				idMap.Add(obj);
			}
		}

		internal int GetID(NSObject obj) {
			if (obj is UID) {
				var uid = obj as UID;
				var first = idMap.OfType<UID>().First(v => NSObject.ArrayEquals(v.Bytes, uid.Bytes));
				return idMap.IndexOf(first);
			} else if (obj is NSArray) {
				int index = 0;

				for (int i = 0; i < idMap.Count; i++) {
					if (idMap[i] == obj) {
						index = i;
						break;
					}
				}

				return index;
			} else {
				return idMap.IndexOf(obj);
			}
		}

		static int ComputeIdSizeInBytes(int numberOfIds) {
			if (numberOfIds < 256)
				return 1;
			return numberOfIds < 65536 ? 2 : 4;
		}

		static int ComputeOffsetSizeInBytes(long maxOffset) {
			if (maxOffset < 256)
				return 1;
			if (maxOffset < 65536)
				return 2;
			return maxOffset < 4294967296L ? 4 : 8;
		}

		internal void WriteIntHeader(int kind, int value) {
			if (value < 0)
				throw new ArgumentException("value must be greater than or equal to 0", "value");

			if (value < 15) {
				Write((kind << 4) + value);
			} else if (value < 256) {
				Write((kind << 4) + 15);
				Write(0x10);
				WriteBytes(value, 1);
			} else if (value < 65536) {
				Write((kind << 4) + 15);
				Write(0x11);
				WriteBytes(value, 2);
			} else {
				Write((kind << 4) + 15);
				Write(0x12);
				WriteBytes(value, 4);
			}
		}

		internal void Write(int b) {
			byte[] bBytes = new byte[1];
			bBytes[0] = (byte)b;
			outStream.Write(bBytes, 0, 1);
			count++;
		}

		internal void Write(byte[] bytes) {
			outStream.Write(bytes, 0, bytes.Length);
			count += bytes.Length;
		}

		internal void WriteBytes(long value, int bytes) {
			// write low-order bytes big-endian style
			for (int i = bytes - 1; i >= 0; i--) {
				Write((int)(value >> (8 * i)));
			}
		}

		internal void WriteID(int id) {
			WriteBytes(id, idSizeInBytes);
		}

		internal void WriteLong(long value) {
			WriteBytes(value, 8);
		}

		internal void WriteDouble(double value) {
			WriteLong(BitConverter.DoubleToInt64Bits(value));
		}
	}
}

