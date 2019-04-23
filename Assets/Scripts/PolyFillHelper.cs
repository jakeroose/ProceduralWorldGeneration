using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delaunay.Geo;

// algorithms taken from:
// https://www.geeksforgeeks.org/how-to-check-if-a-given-point-lies-inside-a-polygon/

public class PolyFillHelper {
	public static int INF = 1000000;

	public static bool IsInside(Vector2 p, List<LineSegment> polygon){
		return IsInside(polygon.ToArray(), polygon.Count, p);
	}

	//public static bool DoIntersect(LineSegment l1, LineSegment l2) {
	//	return DoIntersect((Vector2) l1.p0, (Vector2) l2.p0, (Vector2) l1.p1, (Vector2) l2.p1);
	//}

	public static bool DoIntersect(LineSegment l1, LineSegment l2) {
		return DoIntersect((Vector2) l1.p0, (Vector2) l1.p1, (Vector2) l2.p0, (Vector2) l2.p1);
	}

	public static bool DoIntersect(LineSegment l1, Vector2 p0, Vector2 p1) {
		return DoIntersect((Vector2) l1.p0, (Vector2) l1.p1, p0, p1);
	}

	public static bool IsInside(LineSegment[] polygon, int n, Vector2 p) {
		// There must be at least 3 vertices in polygon[] 
		if ( n < 3 ) return false;

		// Create a point for line segment from p to infinite 
		Vector2 extreme = new Vector2(INF, p.y);

		// Count intersections of the above line with sides of polygon 
		int count = 0, i = 0;
		do {
			int next = (i+1)%n;

			// Check if the line segment from 'p' to 'extreme' intersects 
			// with the line segment from 'polygon[i]' to 'polygon[next]' 
			if ( DoIntersect(polygon[i], p, extreme) ) {
				// If the point 'p' is colinear with line segment 'i-next', 
				// then check if it lies on segment. If it lies, return true, 
				// otherwise false 
				if ( Orientation((Vector2)polygon[i].p0, p, (Vector2) polygon[i].p1) == 0 )
					return OnSegment((Vector2) polygon[i].p0, p, (Vector2) polygon[i].p1);

				count++;
			}
			i = next;
		} while ( i != 0 );

		return (count % 2 == 1);
	}

	public static bool IsInside(Vector2[] polygon, int n, Vector2 p){
		// There must be at least 3 vertices in polygon[] 
		if ( n < 3 ) return false;

		// Create a point for line segment from p to infinite 
		Vector2 extreme = new Vector2(INF, p.y);

		// Count intersections of the above line with sides of polygon 
		int count = 0, i = 0;
		do {
			int next = (i+1)%n;

			// Check if the line segment from 'p' to 'extreme' intersects 
			// with the line segment from 'polygon[i]' to 'polygon[next]' 
			if ( DoIntersect(polygon[i], polygon[next], p, extreme) ) {
				// If the point 'p' is colinear with line segment 'i-next', 
				// then check if it lies on segment. If it lies, return true, 
				// otherwise false 
				if ( Orientation(polygon[i], p, polygon[next]) == 0 )
					return OnSegment(polygon[i], p, polygon[next]);

				count++;
			}
			i = next;
		} while ( i != 0 );

		return (count % 2 == 1);
	}

	public static bool DoIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2){
		// Find the four orientations needed for general and 
		// special cases 
		int o1 = Orientation(p1, q1, p2);
		int o2 = Orientation(p1, q1, q2);
		int o3 = Orientation(p2, q2, p1);
		int o4 = Orientation(p2, q2, q1);

		// General case 
		if ( o1 != o2 && o3 != o4 )
			return true;

		// Special Cases 
		// p1, q1 and p2 are colinear and p2 lies on segment p1q1 
		if ( o1 == 0 && OnSegment(p1, p2, q1) ) return true;

		// p1, q1 and p2 are colinear and q2 lies on segment p1q1 
		if ( o2 == 0 && OnSegment(p1, q2, q1) ) return true;

		// p2, q2 and p1 are colinear and p1 lies on segment p2q2 
		if ( o3 == 0 && OnSegment(p2, p1, q2) ) return true;

		// p2, q2 and q1 are colinear and q1 lies on segment p2q2 
		if ( o4 == 0 && OnSegment(p2, q1, q2) ) return true;

		return false; // Doesn't fall in any of the above cases
	}

	// Given three colinear points p, q, r, the function checks if 
	// point q lies on line segment 'pr'
	private static bool OnSegment(Vector2 p, Vector2 q, Vector2 r){
		if ( q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
						q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y) )
			return true;
		return false;
	}

	// To find orientation of ordered triplet (p, q, r). 
	// The function returns following values 
	// 0 --> p, q and r are colinear 
	// 1 --> Clockwise 
	// 2 --> Counterclockwise 
	private static int Orientation(Vector2 p, Vector2 q, Vector2 r){
		int val = (int)((q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y));
		if ( val == 0 ) return 0; // colinear
		return (val > 0) ? 1 : 2;
	}
}
