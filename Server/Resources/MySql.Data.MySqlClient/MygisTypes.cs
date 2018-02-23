using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public struct MygisCoordinate2D : IEquatable<MygisCoordinate2D> {
	public double X { get; }
	public double Y { get; }
	public MygisCoordinate2D(double x, double y) { X = x; Y = y; }

	public bool Equals(MygisCoordinate2D c) => X == c.X && Y == c.Y;
	public override int GetHashCode() => X.GetHashCode() ^ MygisGeometry.RotateShift(Y.GetHashCode(), sizeof(int) / 2);
	public override bool Equals(object obj) => obj is MygisCoordinate2D && Equals((MygisCoordinate2D)obj);
	public static bool operator ==(MygisCoordinate2D left, MygisCoordinate2D right) => Equals(left, right);
	public static bool operator !=(MygisCoordinate2D left, MygisCoordinate2D right) => !Equals(left, right);
}

public abstract class MygisGeometry {
	protected abstract int GetLenHelper();
	internal int GetLen(bool includeSRID) => 5 + (SRID == 0 || !includeSRID ? 0 : 4) + GetLenHelper();
	public uint SRID { get; set; } = 4326;
	internal static int RotateShift(int val, int shift) => (val << shift) | (val >> (sizeof(int) - shift));
}

public class MygisPoint : MygisGeometry, IEquatable<MygisPoint> {
	MygisCoordinate2D _coord;
	protected override int GetLenHelper() => 16;
	public double X => _coord.X;
	public double Y => _coord.Y;

	public MygisPoint(double x, double y) {
		_coord = new MygisCoordinate2D(x, y);
	}

	public bool Equals(MygisPoint other) => !ReferenceEquals(other, null) && _coord.Equals(other._coord);
	public override bool Equals(object obj) => Equals(obj as MygisPoint);
	public static bool operator ==(MygisPoint x, MygisPoint y) => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);
	public static bool operator !=(MygisPoint x, MygisPoint y) => !(x == y);
	public override int GetHashCode() => X.GetHashCode() ^ RotateShift(Y.GetHashCode(), sizeof(int) / 2);
}

public class MygisLineString : MygisGeometry, IEquatable<MygisLineString>, IEnumerable<MygisCoordinate2D> {
	readonly MygisCoordinate2D[] _points;
	protected override int GetLenHelper() => 4 + _points.Length * 16;
	public IEnumerator<MygisCoordinate2D> GetEnumerator() => ((IEnumerable<MygisCoordinate2D>)_points).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	public MygisCoordinate2D this[int index] => _points[index];
	public int PointCount => _points.Length;

	public MygisLineString(IEnumerable<MygisCoordinate2D> points) {
		_points = points.ToArray();
	}
	public MygisLineString(MygisCoordinate2D[] points) {
		_points = points;
	}

	public bool Equals(MygisLineString other) {
		if (ReferenceEquals(other, null)) return false;
		if (_points.Length != other._points.Length) return false;
		for (var i = 0; i < _points.Length; i++)
			if (!_points[i].Equals(other._points[i])) return false;
		return true;
	}
	public override bool Equals(object obj) => Equals(obj as MygisLineString);
	public static bool operator ==(MygisLineString x, MygisLineString y) => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);
	public static bool operator !=(MygisLineString x, MygisLineString y) => !(x == y);
	public override int GetHashCode() {
		var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
		foreach (var t in _points) ret ^= RotateShift(t.GetHashCode(), ret % sizeof(int));
		return ret;
	}
}

public class MygisPolygon : MygisGeometry, IEquatable<MygisPolygon>, IEnumerable<IEnumerable<MygisCoordinate2D>> {
	readonly MygisCoordinate2D[][] _rings;
	protected override int GetLenHelper() => 4 + _rings.Length * 4 + TotalPointCount * 16;
	public MygisCoordinate2D this[int ringIndex, int pointIndex] => _rings[ringIndex][pointIndex];
	public MygisCoordinate2D[] this[int ringIndex] => _rings[ringIndex];
	public IEnumerator<IEnumerable<MygisCoordinate2D>> GetEnumerator() => ((IEnumerable<IEnumerable<MygisCoordinate2D>>)_rings).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	public int RingCount => _rings.Length;
	public int TotalPointCount => _rings.Sum(r => r.Length);

	public MygisPolygon(MygisCoordinate2D[][] rings) {
		_rings = rings;
	}
	public MygisPolygon(IEnumerable<IEnumerable<MygisCoordinate2D>> rings) {
		_rings = rings.Select(x => x.ToArray()).ToArray();
	}

	public bool Equals(MygisPolygon other) {
		if (ReferenceEquals(other, null)) return false;
		if (_rings.Length != other._rings.Length) return false;
		for (var i = 0; i < _rings.Length; i++) {
			if (_rings[i].Length != other._rings[i].Length) return false;
			for (var j = 0; j < _rings[i].Length; j++)
				if (!_rings[i][j].Equals(other._rings[i][j])) return false;
		}
		return true;
	}
	public override bool Equals(object obj) => Equals(obj as MygisPolygon);
	public static bool operator ==(MygisPolygon x, MygisPolygon y) => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);
	public static bool operator !=(MygisPolygon x, MygisPolygon y) => !(x == y);
	public override int GetHashCode() {
		var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
		for (var i = 0; i < _rings.Length; i++)
			for (var j = 0; j < _rings[i].Length; j++)
				ret ^= RotateShift(_rings[i][j].GetHashCode(), ret % sizeof(int));
		return ret;
	}
}

public class MygisMultiPoint : MygisGeometry, IEquatable<MygisMultiPoint>, IEnumerable<MygisCoordinate2D> {
	readonly MygisCoordinate2D[] _points;
	protected override int GetLenHelper() => 4 + _points.Length * 21;
	public IEnumerator<MygisCoordinate2D> GetEnumerator() => ((IEnumerable<MygisCoordinate2D>)_points).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	public MygisCoordinate2D this[int indexer] => _points[indexer];
	public int PointCount => _points.Length;

	public MygisMultiPoint(MygisCoordinate2D[] points) {
		_points = points;
	}
	public MygisMultiPoint(IEnumerable<MygisPoint> points) {
		_points = points.Select(x => new MygisCoordinate2D(x.X, x.Y)).ToArray();
	}
	public MygisMultiPoint(IEnumerable<MygisCoordinate2D> points) {
		_points = points.ToArray();
	}

	public bool Equals(MygisMultiPoint other) {
		if (ReferenceEquals(other, null)) return false;
		if (_points.Length != other._points.Length) return false;
		for (var i = 0; i < _points.Length; i++)
			if (!_points[i].Equals(other._points[i])) return false;
		return true;
	}
	public override bool Equals(object obj) => Equals(obj as MygisMultiPoint);
	public static bool operator ==(MygisMultiPoint x, MygisMultiPoint y) => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);
	public static bool operator !=(MygisMultiPoint x, MygisMultiPoint y) => !(x == y);
	public override int GetHashCode() {
		var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
		for (var i = 0; i < _points.Length; i++) ret ^= RotateShift(_points[i].GetHashCode(), ret % sizeof(int));
		return ret;
	}
}

public sealed class MygisMultiLineString : MygisGeometry,
	IEquatable<MygisMultiLineString>, IEnumerable<MygisLineString> {
	readonly MygisLineString[] _lineStrings;
	protected override int GetLenHelper() {
		var n = 4;
		for (var i = 0; i < _lineStrings.Length; i++) n += _lineStrings[i].GetLen(false);
		return n;
	}
	public IEnumerator<MygisLineString> GetEnumerator() => ((IEnumerable<MygisLineString>)_lineStrings).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	public MygisLineString this[int index] => _lineStrings[index];
	public int LineCount => _lineStrings.Length;

	internal MygisMultiLineString(MygisCoordinate2D[][] pointArray) {
		_lineStrings = new MygisLineString[pointArray.Length];
		for (var i = 0; i < pointArray.Length; i++)
			_lineStrings[i] = new MygisLineString(pointArray[i]);
	}
	public MygisMultiLineString(MygisLineString[] linestrings) {
		_lineStrings = linestrings;
	}
	public MygisMultiLineString(IEnumerable<MygisLineString> linestrings) {
		_lineStrings = linestrings.ToArray();
	}
	public MygisMultiLineString(IEnumerable<IEnumerable<MygisCoordinate2D>> pointList) {
		_lineStrings = pointList.Select(x => new MygisLineString(x)).ToArray();
	}

	public bool Equals(MygisMultiLineString other) {
		if (ReferenceEquals(other, null)) return false;
		if (_lineStrings.Length != other._lineStrings.Length) return false;
		for (var i = 0; i < _lineStrings.Length; i++)
			if (_lineStrings[i] != other._lineStrings[i]) return false;
		return true;
	}
	public override bool Equals(object obj) => Equals(obj as MygisMultiLineString);
	public static bool operator ==(MygisMultiLineString x, MygisMultiLineString y) => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);
	public static bool operator !=(MygisMultiLineString x, MygisMultiLineString y) => !(x == y);
	public override int GetHashCode() {
		var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
		for (var i = 0; i < _lineStrings.Length; i++) ret ^= RotateShift(_lineStrings[i].GetHashCode(), ret % sizeof(int));
		return ret;
	}
}

public class MygisMultiPolygon : MygisGeometry, IEquatable<MygisMultiPolygon>, IEnumerable<MygisPolygon> {
	readonly MygisPolygon[] _polygons;
	protected override int GetLenHelper() {
		var n = 4;
		for (var i = 0; i < _polygons.Length; i++) n += _polygons[i].GetLen(false);
		return n;
	}
	public IEnumerator<MygisPolygon> GetEnumerator() => ((IEnumerable<MygisPolygon>)_polygons).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	public MygisPolygon this[int index] => _polygons[index];
	public int PolygonCount => _polygons.Length;

	public MygisMultiPolygon(MygisPolygon[] polygons) {
		_polygons = polygons;
	}
	public MygisMultiPolygon(IEnumerable<MygisPolygon> polygons) {
		_polygons = polygons.ToArray();
	}
	public MygisMultiPolygon(IEnumerable<IEnumerable<IEnumerable<MygisCoordinate2D>>> ringList) {
		_polygons = ringList.Select(x => new MygisPolygon(x)).ToArray();
	}

	public bool Equals(MygisMultiPolygon other) {
		if (ReferenceEquals(other, null)) return false;
		if (_polygons.Length != other._polygons.Length) return false;
		for (var i = 0; i < _polygons.Length; i++) if (_polygons[i] != other._polygons[i]) return false;
		return true;
	}
	public override bool Equals(object obj) => obj is MygisMultiPolygon && Equals((MygisMultiPolygon)obj);
	public static bool operator ==(MygisMultiPolygon x, MygisMultiPolygon y) => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);
	public static bool operator !=(MygisMultiPolygon x, MygisMultiPolygon y) => !(x == y);
	public override int GetHashCode() {
		var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
		for (var i = 0; i < _polygons.Length; i++) ret ^= RotateShift(_polygons[i].GetHashCode(), ret % sizeof(int));
		return ret;
	}
}

public class MygisGeometryCollection : MygisGeometry, IEquatable<MygisGeometryCollection>, IEnumerable<MygisGeometry> {
	readonly MygisGeometry[] _geometries;
	public MygisGeometry this[int index] => _geometries[index];
	public IEnumerator<MygisGeometry> GetEnumerator() => ((IEnumerable<MygisGeometry>)_geometries).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	protected override int GetLenHelper() {
		var n = 4;
		for (var i = 0; i < _geometries.Length; i++)
			n += _geometries[i].GetLen(true);
		return n;
	}
	public int GeometryCount => _geometries.Length;

	public MygisGeometryCollection(MygisGeometry[] geometries) {
		_geometries = geometries;
	}
	public MygisGeometryCollection(IEnumerable<MygisGeometry> geometries) {
		_geometries = geometries.ToArray();
	}

	public bool Equals(MygisGeometryCollection other) {
		if (ReferenceEquals(other, null)) return false;
		if (_geometries.Length != other._geometries.Length) return false;
		for (var i = 0; i < _geometries.Length; i++)
			if (!_geometries[i].Equals(other._geometries[i])) return false;
		return true;
	}
	public override bool Equals(object obj) => Equals(obj as MygisGeometryCollection);
	public static bool operator ==(MygisGeometryCollection x, MygisGeometryCollection y) => ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);
	public static bool operator !=(MygisGeometryCollection x, MygisGeometryCollection y) => !(x == y);
	public override int GetHashCode() {
		var ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
		for (var i = 0; i < _geometries.Length; i++) ret ^= RotateShift(_geometries[i].GetHashCode(), ret % sizeof(int));
		return ret;
	}
}