using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Modules.UnityMathematics.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class to represent edges of the navmesh used internally by the Vismap generator.
/// </summary>
public class Edge
{
    public Vector3 Vertex1 { get; }
    public Vector3 Vertex2 { get; }

    public Edge(Vector3 vertex1, Vector3 vertex2)
    {
        // Ensure consistent ordering of vertices based on x, y, z values
        if (vertex1.x < vertex2.x || (vertex1.x == vertex2.x && vertex1.y < vertex2.y) || (vertex1.x == vertex2.x && vertex1.y == vertex2.y && vertex1.z < vertex2.z))
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }
        else
        {
            Vertex1 = vertex2;
            Vertex2 = vertex1;
        }
    }

    /// <summary>
    /// Compares this edge with another to see if they're the same edge.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Edge other = (Edge)obj;
        return (Vertex1.Equals(other.Vertex1) && Vertex2.Equals(other.Vertex2));
    }
}

public class VisMapGenerator : MonoBehaviour
{
    public LayerMask coverLayer;
    public LayerMask navigationLayer;
    public List<Vector3> boundaryEdges;
    public List<Edge> edges = new List<Edge>();
    public string filePath;
    public float gridSize = 2f;
    public float subGridSize = 2f;
    public float yCeiling = 6f;
    NavMeshTriangulation navMeshData;

    private List<Vector3> gridPoints = new List<Vector3>();
    private Dictionary<Vector3, BitArray> visibilityMap = new Dictionary<Vector3, BitArray>();


    [Button("Extract Boundary Edges")]
    public void ExtractBoundaryEdges()
    {
        //yCeiling is the maximum height we want the vismap to go to. Decided by combat and level design.
        boundaryEdges = ExtractBoundaryEdges(yCeiling);
    }

    [Button("Generate Grid Points")]
    public void GenerateGridPoints()
    {
        GenerateGridPoints(boundaryEdges);
    }

    [Button("Generate VisMap")]
    public void GenerateVisMap()
    {
        GenerateVisibilityMap();
    }

    /// <summary>
    /// Find the external edges of the navmesh grid up to vertical limit yLimit.
    /// </summary>
    /// <param name="yLimit"></param>
    /// <returns></returns>
    public List<Vector3> ExtractBoundaryEdges(float yLimit)
    {
        List<Vector3> boundaryEdges = new List<Vector3>();
        navMeshData = NavMesh.CalculateTriangulation();

        //Store the triangulation as three distinct edges per tri. Value int indicates how many times this edge was found.
        //An edge is part of a boundary if it is only found once.
        Dictionary<Edge, int> edgeDict = new Dictionary<Edge, int>();

        // Iterate over triangles
        for (int i = 0; i < navMeshData.indices.Length; i += 3)
        {
            int[] triangleIndices = new int[3] { navMeshData.indices[i], navMeshData.indices[i + 1], navMeshData.indices[i + 2] };

            // For each edge in the triangle
            for (int j = 0; j < 3; j++)
            {
                int vertIndexA = triangleIndices[j];
                int vertIndexB = triangleIndices[(j + 1) % 3];

                Vector3 vertexA = navMeshData.vertices[vertIndexA];
                Vector3 vertexB = navMeshData.vertices[vertIndexB];

                Edge edge = new Edge(vertexA, vertexB);

                if (edgeDict.ContainsKey(edge))
                {
                    edgeDict[edge]++;
                }
                else
                {
                    edgeDict[edge] = 1;
                }
            }
        }

        // Extract boundary edges
        foreach (Edge edge in edgeDict)
        {
            if (edge.Value == 1) // Edge is part of the boundary if it is only present on one triangle.
            {
                edges.Add(edge.Key);
                boundaryEdges.Add(edge.Key.Vertex1);
                boundaryEdges.Add(edge.Key.Vertex2);
            }
        }

        Debug.Log("Boundary edges identified = " + boundaryEdges.Count);
        return boundaryEdges;
    }


    /// <summary>
    /// Generate the list of points that will comprise the vismap grid.
    /// </summary>
    /// <param name="boundaryEdges"></param>
    public async void GenerateGridPoints(List<Vector3> boundaryEdges)
    {

        gridPoints.Clear();

        //Rather than check mathematically if a point is within the bounds, we'll create a mesh using the boundary edges we extracted earlier, then use Unity's built-in physics.
        Mesh mesh = new Mesh();

        mesh.SetVertices(navMeshData.vertices.ToList());
        mesh.SetIndices(navMeshData.indices.ToList(), MeshTopology.Triangles, 0);

        mesh.RecalculateBounds();

        float minx = mesh.bounds.min.x;
        float miny = mesh.bounds.min.y + 1f;
        float minz = mesh.bounds.min.z;

        float maxx = mesh.bounds.max.x;
        float maxy = mesh.bounds.max.y + yCeiling;
        float maxz = mesh.bounds.max.z;

        //Iterate over all axes, starting with the smallest values.
        for (float x = minx; x < maxx; x += gridSize)
        {
            for (float y = miny; y < maxy; y += gridSize)
            {
                for (float z = minz; z < maxz; z += gridSize)
                {
                    //This process can take a while, and I want to still be able to use Unity and don't want to maintain a build machine.
                    await Task.Yield();
                    Vector3 point = new Vector3(Mathf.Round(x), Mathf.Round(y), Mathf.Round(z));

                    //Here we check if the point is within the bounds of game objects that would prevent characters from existing there.
                    if (!Physics.CheckSphere(point,0.01f,coverLayer) && !Physics.CheckSphere(point, 0.01f, navigationLayer)) 
                    {
                        if (!gridPoints.Contains(point)) 
                        {
                            gridPoints.Add(point);
                            await Task.Yield();
                            foreach (Edge edge in edges)
                            {
                                //Increase the fidelity of the vismap in places where it matters: within close proximity of game objects that provide cover.
                                if (IsWithinDistanceOfLine(point, edge.Vertex1, edge.Vertex2, 1.5f))
                                {

                                    GenerateSubPoints(point,maxx,minx,maxy,miny,maxz,minz);
                                }
                            }

                        }                  
                    }
                }   
            }
        }

        Debug.Log("Pointmap created with gridPoint count of " + gridPoints.Count);
    }

    /// <summary>
    /// Determines if a point is within a distance of a line.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="lineStart"></param>
    /// <param name="lineEnd"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static bool IsWithinDistanceOfLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd, float distance = 2f)
    {
        float distanceSquared = distance * distance;
        Vector3 lineVector = lineEnd - lineStart;
        Vector3 pointVector = point - lineStart;

        float t = Vector3.Dot(pointVector, lineVector) / Vector3.Dot(lineVector, lineVector);

        t = Mathf.Clamp01(t);
        Vector3 closestPoint = lineStart + t * lineVector;

        float distanceToLineSquared = (point - closestPoint).sqrMagnitude;

        return distanceToLineSquared <= distanceSquared;
    }

    /// <summary>
    /// Generate additional fidelity around a passed point. Creates points within the min/max bounds around a point provided.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="maxx"></param>
    /// <param name="minx"></param>
    /// <param name="maxy"></param>
    /// <param name="miny"></param>
    /// <param name="maxz"></param>
    /// <param name="minz"></param>
    void GenerateSubPoints(Vector3 point, float maxx, float minx, float maxy, float miny, float maxz, float minz)
    {
        for (float x = -1; x <= 1; x += subGridSize)
        {
            for (float y = -1; y <= 1; y += subGridSize)
            {
                for (float z = -1; z <= 1; z += subGridSize)
                {
                    Vector3 subPoint = point + new Vector3(x, y, z);
                    if (subPoint.x < maxx && subPoint.x > minx && subPoint.y < maxy && subPoint.y > miny && subPoint.z < maxz && subPoint.z > minz)
                    {
                        if (!Physics.CheckSphere(point, 0.01f, coverLayer) && !Physics.CheckSphere(point, 0.01f, navigationLayer))
                        {
                            if (!gridPoints.Contains(subPoint)) { gridPoints.Add(subPoint); }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Kicks off the vismap bake. Raycasts from each point to each other point, caches all of that data on a per-point basis, then serializes.
    /// </summary>
    async void GenerateVisibilityMap()
    {
        int totalPoints = gridPoints.Count;
        Debug.Log(totalPoints);

        for (int i = 0; i < totalPoints; i++)
        {
            Vector3 pointA = gridPoints[i];
            BitArray visiblePoints = new BitArray(totalPoints);
            for (int j = 0; j < totalPoints; j++)
            {
                Vector3 pointB = gridPoints[j];
                if (!visibilityMap.ContainsKey(pointB))
                {
                    if (i != j && IsVisible(pointA, pointB))
                    {
                        visiblePoints[j] = true;
                        await Task.Yield();
                    }
                }
            }
            visibilityMap.Add(pointA, visiblePoints);
            Debug.Log("Bake at " + ((100/totalPoints) * i) + "% completion");
        }
        Debug.Log("vismap created with kvp count of " + visibilityMap.Count);
        SerializeVisibilityMap();
    }

    /// <summary>
    /// Returns true if there are no collisions between two points, indicating a clear line of sight.
    /// </summary>
    /// <param name="pointA"></param>
    /// <param name="pointB"></param>
    /// <returns></returns>
    bool IsVisible(Vector3 pointA, Vector3 pointB)
    {
        return !Physics.Linecast(pointA, pointB, coverLayer);
    }

    /// <summary>
    /// Writes the bit array to a binary for storage.
    /// </summary>
    void SerializeVisibilityMap()
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            // First, write the number of points
            writer.Write(gridPoints.Count);

            // Write each grid point
            foreach (Vector3 point in gridPoints)
            {
                writer.Write(point.x);
                writer.Write(point.y);
                writer.Write(point.z);
            }

            // Write visibility data
            foreach (KeyValuePair<Vector3, BitArray> kvp in visibilityMap)
            {
                Vector3 point = kvp.Key;
                BitArray bitArray = kvp.Value;

                writer.Write(point.x);
                writer.Write(point.y);
                writer.Write(point.z);

                // Write the length of the BitArray in bits
                writer.Write(bitArray.Length);

                // Calculate the exact number of bytes needed for the BitArray
                int bytesLength = (bitArray.Length + 7) / 8;

                // Convert BitArray to bytes and write
                byte[] bitArrayBytes = new byte[bytesLength];
                bitArray.CopyTo(bitArrayBytes, 0);
                writer.Write(bitArrayBytes);
            }
        }
        Debug.Log("vismap created at " + filePath);
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Edge edge in edges) { Debug.DrawLine(edge.Vertex1, edge.Vertex2, Color.red); }
        foreach (Vector3 point in gridPoints) { Gizmos.DrawSphere(point, 0.1f); }
    }

}