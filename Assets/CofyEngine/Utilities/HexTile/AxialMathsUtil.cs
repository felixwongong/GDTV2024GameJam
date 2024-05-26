using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AxialCoord {
	public bool Equals(AxialCoord other) {
		return q == other.q && r == other.r;
	}

	public override bool Equals(object obj) {
		return obj is AxialCoord other && Equals(other);
	}

	public override int GetHashCode() {
		return HashCode.Combine(q, r);
	}

	public int q;
	public int r;

	public AxialCoord(int q, int r) {
		this.q = q;
		this.r = r;
	}

	public override string ToString()
	{
		return string.Format("{0},{1}", q, r);
	}
																				/*	point Top Name	*/
	public static readonly AxialCoord Top = new AxialCoord(1, 0);			/*	Right				*/
	public static readonly AxialCoord TopRight = new AxialCoord(0, 1);		/*	TopRight		*/
	public static readonly AxialCoord TopLeft = new AxialCoord(1, -1);		/*	BottomRight			*/

	public static readonly AxialCoord Bottom  = new AxialCoord(-1, 0);		/*	Left			*/
	public static readonly AxialCoord BottomRight  = new AxialCoord(-1, 1);	/*	TopLeft		*/
	public static readonly AxialCoord BottomLeft = new AxialCoord(0, -1);	/*	BottomLeft		*/

	public static readonly AxialCoord zero = new(0, 0);
	
	public static AxialCoord operator +(AxialCoord lhs, AxialCoord rhs) => new AxialCoord(lhs.q + rhs.q, lhs.r + rhs.r);

	public static AxialCoord operator -(AxialCoord lhs, AxialCoord rhs) => new AxialCoord(lhs.q - rhs.q, lhs.r - rhs.r);

	public static AxialCoord operator *(AxialCoord lhs, int value) => new AxialCoord(lhs.q * value, lhs.r * value);

	public static bool operator ==(AxialCoord lhs, AxialCoord rhs) => lhs.q == rhs.q && lhs.r == rhs.r;

	public static bool operator !=(AxialCoord lhs, AxialCoord rhs) => !(lhs == rhs);

	public static implicit operator Vector2Int(AxialCoord coord) => new(coord.q, coord.r);
	public static AxialCoord MinValue = new(int.MinValue, int.MinValue);
	public static AxialCoord MaxValue = new(int.MaxValue, int.MaxValue);
	
	public static void OddRToAxial(int x, int y, out AxialCoord outAxial) {
		outAxial.q = x - (y - (y & 1)) / 2;
		outAxial.r = y;
	}

	public static Vector3Int AxialToOddR(AxialCoord axialCoord) {
		int x = axialCoord.q + (axialCoord.r - (axialCoord.r & 1)) / 2;
		int y = axialCoord.r;
		return new Vector3Int(x, y, 0);
	}

	public static AxialCoord[] allDirectionsClockwise()
	{
		return new []{ Top, TopRight, BottomRight, Bottom, BottomLeft, TopLeft };
	}
}

//Counterclockwise order is important for rotation calculation
public enum Direction: byte
{
	TopLeft = 0,
	BottomLeft = 1,
	Bottom = 2, 
	BottomRight = 3,
	TopRight = 4,
	Top = 5,
}

public static class AxialCoordExtension
{
	public static AxialCoord coord(this Direction dir)
	{
		return dir switch
		{
			Direction.Top => AxialCoord.Top,
			Direction.TopRight => AxialCoord.TopRight,
			Direction.BottomRight => AxialCoord.BottomRight,
			Direction.Bottom => AxialCoord.Bottom,
			Direction.BottomLeft => AxialCoord.BottomLeft,
			Direction.TopLeft => AxialCoord.TopLeft,
			_ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
		};
	}

	public static Vector3Int oddr(this AxialCoord coord)
	{
		return AxialCoord.AxialToOddR(coord);
	}

	public static AxialCoord axial(this Vector2Int oddr)
	{
		AxialCoord.OddRToAxial(oddr.x, oddr.y, out var axial);
		return axial;
	}

	public static Vector2Int xy(this Vector3Int vec3)
	{
		return new Vector2Int(vec3.x, vec3.y);
	}
	
	public static AxialCoord rotate60(this AxialCoord coord)
	{
		return new AxialCoord(-coord.r, coord.q + coord.r);
	}
	
	public static AxialCoord rotate60Counter(this AxialCoord coord)
	{
		return new AxialCoord(coord.q + coord.r, -coord.q);
	}

	public static AxialCoord rotate120(this AxialCoord coord)
	{
		return new AxialCoord(-coord.q - coord.r, coord.q);
	}
	
	public static AxialCoord rotate120Counter(this AxialCoord coord)
	{
		return new AxialCoord(coord.r, -coord.q - coord.r);
	}
	
	public static AxialCoord rotate180(this AxialCoord coord)
	{
		return new AxialCoord(-coord.q, -coord.r);
	}

	public static AxialCoord clamp1(this AxialCoord coord)
	{
		coord.q = Mathf.Clamp(coord.q, -1, 1);
		coord.r = Mathf.Clamp(coord.r, -1, 1);
		return coord;
	}

	public static AxialCoord decode(this string s)
	{
		var coord = new AxialCoord();
		var separatorIdx = s.IndexOf(",", StringComparison.InvariantCulture);
		
		coord.q = int.Parse(s.AsSpan(0, separatorIdx));
		coord.r = int.Parse(s.AsSpan(separatorIdx + 1, s.Length - separatorIdx - 1));
		return coord;
	}

	public static List<AxialCoord> GetAxialCoordsInRange(this AxialCoord coord, int distance, bool excludeOrigin = false)
	{
		var list = new List<AxialCoord>();
		if(distance < 1) return list;

		for(int q = -distance; q <= distance; q++)
		{
			for(int r = Mathf.Max(-distance, -q - distance); r <= Mathf.Min(distance, -q + distance); r++)
			{
				if(excludeOrigin && q == 0 && r == 0) continue;
				
				list.Add(coord + new AxialCoord(q, r));
			}
		}
		
		return list;
	}

	public static AxialCoord rotateFromTop(this AxialCoord coord, AxialCoord rotateTo)
	{
		return rotateTo switch
		{
			_ when rotateTo == AxialCoord.Top => coord,
			_ when rotateTo == AxialCoord.TopRight => coord.rotate60(),
			_ when rotateTo == AxialCoord.BottomRight => coord.rotate120(),
			_ when rotateTo == AxialCoord.Bottom => coord.rotate180(),
			_ when rotateTo == AxialCoord.BottomLeft => coord.rotate120Counter(),
			_ when rotateTo == AxialCoord.TopLeft => coord.rotate60Counter(),
			_ => throw new ArgumentOutOfRangeException(nameof(rotateTo), rotateTo, null)
		};
	}

	public static AxialCoord rotateToTop(this AxialCoord coord, AxialCoord rotateFrom)
	{
		return rotateFrom switch
		{
			_ when rotateFrom == AxialCoord.Top => coord,
			_ when rotateFrom == AxialCoord.TopRight => coord.rotate60Counter(),
			_ when rotateFrom == AxialCoord.BottomRight => coord.rotate120Counter(),
			_ when rotateFrom == AxialCoord.Bottom => coord.rotate180(),
			_ when rotateFrom == AxialCoord.BottomLeft => coord.rotate120(),
			_ when rotateFrom == AxialCoord.TopLeft => coord.rotate60(),
			_ => throw new ArgumentOutOfRangeException(nameof(rotateFrom), rotateFrom, null)
		};
	}
    
	public static Quaternion getRotation(this AxialCoord direction)
	{
		direction = direction.clamp1();
		float yaw = direction switch
		{
			_ when direction == AxialCoord.Top => 0,
			_ when direction == AxialCoord.TopLeft => 60,
			_ when direction == AxialCoord.BottomLeft => 120,
			_ when direction == AxialCoord.Bottom => 180,
			_ when direction == AxialCoord.BottomRight => 240,
			_ when direction == AxialCoord.TopRight => 300,
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
		};
		return Quaternion.Euler(0, 0, yaw);
	}
}