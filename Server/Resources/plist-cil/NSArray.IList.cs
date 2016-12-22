// plist-cil - An open source library to parse and generate property lists for .NET
// Copyright (C) 2016 Quamotion
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PList {
	partial class NSArray : IList<NSObject> {
		/// <inheritdoc/>
		public NSObject this[int index] {
			get {
				return this.array[index];
			}

			set {
				this.array[index] = value;
			}
		}

		/// <inheritdoc/>
		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public void Add(object item) {
			this.Add(NSObject.Wrap(item));
		}

		/// <inheritdoc/>
		public void Add(NSObject item) {
			this.array.Add(item);
		}

		/// <inheritdoc/>
		public void Clear() {
			this.array.Clear();
		}

		public bool Contains(object item) {
			return this.Contains(NSObject.Wrap(item));
		}

		/// <inheritdoc/>
		public bool Contains(NSObject item) {
			return this.array.Contains(item);
		}

		/// <inheritdoc/>
		public void CopyTo(NSObject[] array, int arrayIndex) {
			this.array.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc/>
		public IEnumerator<NSObject> GetEnumerator() {
			return this.array.GetEnumerator();
		}

		public int IndexOf(object item) {
			return this.array.IndexOf(NSObject.Wrap(item));
		}

		/// <inheritdoc/>
		public int IndexOf(NSObject item) {
			return this.array.IndexOf(item);
		}

		public void Insert(int index, object item) {
			this.Insert(index, NSObject.Wrap(item));
		}

		/// <inheritdoc/>
		public void Insert(int index, NSObject item) {
			this.array.Insert(index, item);
		}

		public bool Remove(object item) {
			return this.Remove(NSObject.Wrap(item));
		}

		/// <inheritdoc/>
		public bool Remove(NSObject item) {
			return this.array.Remove(item);
		}

		/// <inheritdoc/>
		public void RemoveAt(int index) {
			this.array.RemoveAt(index);
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() {
			return this.array.GetEnumerator();
		}
	}
}
