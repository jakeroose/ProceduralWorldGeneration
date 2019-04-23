using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delaunay;
using Delaunay.Geo;

/* TODO:
 - declare v polygons as biomes
 - biomes should be able to spread across multiple polygons
 - smoothing between polygon edges
 */

public class VoronoiSimplexGenerator : MonoBehaviour {

	private class Point {
		public Vector3 coord;
		public uint color;

		public Point(Vector3 vector3, uint v) {
			this.coord = vector3;
			this.color = v;
		}

		public Vector2 Vec2Coord(){
			return new Vector2(coord.x, coord.z);
		}
	};

	[SerializeField]
	private int	m_pointCount = 300;
	[SerializeField]
	private float m_mapDepth = 10;
	[SerializeField]
	private float simplexFactor = 10f;

	private List<Vector2> m_points;
	private readonly float m_mapWidth = 100;
	private readonly float m_mapHeight = 100;
	private List<LineSegment> m_edges = null;
	private List<LineSegment> m_spanningTree;
	private List<LineSegment> m_delaunayTriangulation;

	private OpenSimplexNoise m_simplexNoise;
	private List<Point> m_simplexPoints;

	private Voronoi m_voronoi;
	private List<uint> m_colors;

	private List<LineSegment> tmpVBoundary;

	// generate our simplex plane as a List of Points of length m_mapWidth * m_mapHeight
	private void GenerateSimplexPlane(){
		m_simplexNoise = new OpenSimplexNoise(123456789);
		m_simplexPoints = new List<Point>();

		for ( int i = 0; i < m_mapWidth; i++ ) {
			for ( int j = 0; j < m_mapHeight; j++ ) {
				double noiseVal = m_simplexNoise.eval(i / simplexFactor, j / simplexFactor);
				m_simplexPoints.Add(new Point(new Vector3(i, (float) noiseVal * m_mapDepth, j), 0));
			}
		}
	}

	void Awake() {
		Demo();
	}

	void Update() {
		if ( Input.anyKeyDown ) {
			Demo();
		}
	}

	// Will find nearest Voronoi diagram and assign uniform color to each voronoi polygon
	// TODO: better error checking. probably fix in PolyFillHelper
	// probably a way to make more efficient point checking - i.e. not checking every point for every polygon
	private void AssignSimplexColors(){
		if ( m_points == null || m_simplexPoints == null ) {
			Debug.Log("ERROR - AssignSimplexColors - points and/or simplexPoints not yet defined");
			return;
		}

		for(int i = 0; i < m_points.Count; i++){
			List<LineSegment> vBounds = m_voronoi.VoronoiBoundaryForSite(m_points[i]);
			List<Point> points = m_simplexPoints.FindAll(p => PolyFillHelper.IsInside(p.Vec2Coord(), vBounds));
			// currently an error if polygon is on right edge of map so we skip it
			if( m_points[i].x > (0.9f * m_mapWidth) && ((float)points.Count / (float)m_simplexPoints.Count) > 0.03f){
				Debug.Log("Skipping point (" + m_points[i].x + ", " + m_points[i].y + ") - " + points.Count + " points");
				Debug.Log((float) points.Count / (float) m_simplexPoints.Count);
			} else {
				for ( int j = 0; j < points.Count; j++ ) {
					Point p = points[j];
					p.color = m_colors[i];
				}
			}
		}
		
	}

	// converts rgba uint to UnityEngine.Color
	private Color UintToColor(uint c){
		uint mask = 255;
		uint r = (c >> 24) & mask;
		uint g = (c >> 16) & mask;
		uint b = (c >> 8) & mask;
		return new Color((float) r / 255f, (float) g / 255f, (float) b / 255f);
	}

	private uint RandomUintColor(){
		uint r = (uint)UnityEngine.Random.Range(31, 255);
		uint g = (uint)UnityEngine.Random.Range(31, 255);
		uint b = (uint)UnityEngine.Random.Range(31, 255);
		return ((r << 24) | (g << 16) | (b << 8) | (uint) 255);
	}

	private void Demo() {
		GenerateSimplexPlane();
		m_points = new List<Vector2>();
		m_colors = new List<uint>();

		for ( int i = 0; i < m_pointCount; i++ ) {
			m_colors.Add(RandomUintColor());
			m_points.Add(new Vector2(
					UnityEngine.Random.Range(0, m_mapWidth),
					UnityEngine.Random.Range(0, m_mapHeight))
			);
		}
		//Delaunay.Voronoi v = new Delaunay.Voronoi (m_points, m_colors, new Rect (0, 0, m_mapWidth, m_mapHeight));
		m_voronoi = new Delaunay.Voronoi(m_points, m_colors, new Rect(0, 0, m_mapWidth, m_mapHeight));
		//m_edges = v.VoronoiDiagram();
		m_edges = m_voronoi.VoronoiDiagram();

		m_spanningTree = m_voronoi.SpanningTree(KruskalType.MINIMUM);
		m_delaunayTriangulation = m_voronoi.DelaunayTriangulation();
		tmpVBoundary = m_voronoi.VoronoiBoundaryForSite(m_points[0]);
		//DrawVoronoiPolygon(m_points[0]);
		AssignSimplexColors();
	}

	private Vector3[] VerticiesFromSegments(List<LineSegment> lines){
		Vector3[] verticies = new Vector3[lines.Count + 1];
		Vector2 tmp, point = Vector2.zero;
		int l_index = 1;
		List<int> triangles = new List<int>();
		verticies[0] = new Vector3(point.x, 0, point.y);

		//for (int i = 0; i < lines.Count; i++ ){
		//	tmp = (Vector2)lines[i].p0;
		//	verticies[i] = new Vector3(tmp.x, 0, tmp.y);
		//}

		foreach(LineSegment l in lines){
			tmp = (Vector2) lines[l_index].p0;
			verticies[l_index] = new Vector3(tmp.x, 0, tmp.y);
			triangles.Add(0);
			triangles.Add(l_index);
			triangles.Add((l_index + 1) % lines.Count);

			l_index++;
		}
		return verticies;
	}

	private void DrawVoronoiPolygon(Vector2 point){
		GameObject poly = new GameObject("Poly");
		List<LineSegment> lines = tmpVBoundary;

		MeshRenderer mr = poly.AddComponent<MeshRenderer>();
		mr.material = Resources.Load<Material>("PolyMat");

		MeshFilter mf = poly.AddComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		mesh.Clear(false);
		mf.mesh = mesh;
		mesh.name = "Poly_mesh";

		Vector2 tmp; // point = Vector2.zero;
		int l_index = 0;

		Vector3[] vertices = new Vector3[lines.Count + 1];
		List<int> triangles = new List<int>();
		vertices[0] = new Vector3(point.x, point.y, 0); // center of our polygon

		foreach ( LineSegment l in lines ) {
			Debug.Log("l_index: " + l_index);
			if(l_index % 2 == 0){
				tmp = (Vector2) lines[l_index].p0;
				vertices[l_index] = new Vector3(tmp.x, tmp.y, 0);
				triangles.Add(0);
				triangles.Add(l_index + 1);
				int next = (l_index + 2) % (lines.Count + 1);
				if ( next == 0 ) next = 1;
				triangles.Add(next);
				Debug.Log("Added Triange (" + 0 + "," + (l_index + 1) + "," + (next) + ")");
			}

			l_index++;
		}

		Debug.Log(vertices);

		mesh.vertices = vertices;
		mesh.triangles = triangles.ToArray();

		//List<Vector2> uv = new List<Vector2>();
		//for ( int i = 0; i < vertices.Length; ++i )
		//	uv.Add(new Vector2(0.0f, 0.0f)); // FIX this! to map your texture correctly
		//mesh.uv = uv.ToArray();

		mesh.RecalculateNormals();
	}



	void OnDrawGizmos() {
		//Gizmos.color = Color.red;
		//if ( m_points != null ) {
		//	for ( int i = 0; i < m_points.Count; i++ ) {
		//		Gizmos.DrawSphere(m_points[i], 0.2f);
		//	}
		//}

		Gizmos.color = Color.black;
		if ( m_simplexPoints != null ) {
			for ( int i = 0; i < m_simplexPoints.Count; i++ ) {
				Vector2 p = new Vector2(m_simplexPoints[i].coord.x, m_simplexPoints[i].coord.z);// + new Vector2(0.5f, 0.5f);
				//bool inBounds = PolyFillHelper.IsInside(p, tmpVBoundary);
				//if(inBounds){
				//	Gizmos.color = UintToColor(m_colors[0]);
				//} else {
				//	Gizmos.color = Color.black;
				//}
				Gizmos.color = UintToColor(m_simplexPoints[i].color);
				Gizmos.DrawSphere(m_simplexPoints[i].coord, 0.5f);
			}
		}

		//if ( m_edges != null ) {
		//	Gizmos.color = Color.white;
		//	LineSegment tmp;
		//	for ( int i = 0; i < m_edges.Count; i++ ) {
		//		tmp = m_edges[i];
		//		int tx = (int)((Vector2)tmp.p0).x;
		//		int ty = (int)((Vector2)tmp.p0).y;
		//		double noiseVal = m_simplexNoise.eval(tx / simplexFactor, ty / simplexFactor);
		//		Vector3 left = new Vector3(tx, (float) noiseVal * m_mapDepth, ty);
		//		tx = (int)((Vector2)tmp.p1).x;
		//		ty = (int)((Vector2)tmp.p1).y;
		//		noiseVal = m_simplexNoise.eval(tx / simplexFactor, ty / simplexFactor);
		//		Vector3 right = new Vector3(tx, (float) noiseVal * m_mapDepth, ty);
		//		Gizmos.DrawLine( left, (Vector3) right);
		//	}
		//}

		//Gizmos.color = Color.magenta;
		//if ( m_delaunayTriangulation != null ) {
		//	for ( int i = 0; i < m_delaunayTriangulation.Count; i++ ) {
		//		Vector2 left = (Vector2)m_delaunayTriangulation [i].p0;
		//		Vector2 right = (Vector2)m_delaunayTriangulation [i].p1;
		//		Gizmos.DrawLine((Vector3) left, (Vector3) right);
		//	}
		//}

		//Gizmos.color = Color.cyan;
		//if ( m_delaunayTriangulation != null ) {

		//	for ( int i = 0; i < tmpVBoundary.Count; i++ ) {
		//		Vector2 left = (Vector2)tmpVBoundary [i].p0;
		//		Vector2 right = (Vector2)tmpVBoundary [i].p1;
		//		Gizmos.DrawLine((Vector3) left, (Vector3) right);
		//	}
		//}

		//if (m_spanningTree != null) {
		//	Gizmos.color = Color.green;
		//	for (int i = 0; i< m_spanningTree.Count; i++) {
		//		LineSegment seg = m_spanningTree [i];				
		//		Vector2 left = (Vector2)seg.p0;
		//		Vector2 right = (Vector2)seg.p1;
		//		Gizmos.DrawLine ((Vector3)left, (Vector3)right);
		//	}
		//}

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(new Vector2(0, 0), new Vector2(0, m_mapHeight));
		Gizmos.DrawLine(new Vector2(0, 0), new Vector2(m_mapWidth, 0));
		Gizmos.DrawLine(new Vector2(m_mapWidth, 0), new Vector2(m_mapWidth, m_mapHeight));
		Gizmos.DrawLine(new Vector2(0, m_mapHeight), new Vector2(m_mapWidth, m_mapHeight));
	}
}
