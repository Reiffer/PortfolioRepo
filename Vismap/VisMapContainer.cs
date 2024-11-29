using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection.Emit;
using UnityEditor;

public class VisMapContainer : MonoBehaviour
{
    public string filePath;
    private List<Vector3> gridPoints;
    private Dictionary<Vector3, BitArray> visibilityMap;

    void Start()
    {
        visibilityMap = DeserializeVisibilityMap(filePath, out gridPoints);
    }

    public static Dictionary<Vector3, BitArray> DeserializeVisibilityMap(string filePath, out List<Vector3> gridPoints)
    {
        Dictionary<Vector3, BitArray> visibilityMap = new Dictionary<Vector3, BitArray>();
        gridPoints = new List<Vector3>();

        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (BinaryReader reader = new BinaryReader(fs))
        {
            // Read number of grid points
            int pointCount = reader.ReadInt32();

            // Read each grid point
            for (int i = 0; i < pointCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                gridPoints.Add(new Vector3(x, y, z));
            }

            // Read visibility map data
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                // Read Vector3 key
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                Vector3 key = new Vector3(x, y, z);

                // Read BitArray length in bits
                int bitsLength = reader.ReadInt32();

                // Calculate bytes length
                int bytesLength = (bitsLength + 7) / 8;

                // Read BitArray bytes
                byte[] bytes = reader.ReadBytes(bytesLength);

                // Create BitArray from bytes
                BitArray bits = new BitArray(bytes) { Length = bitsLength };

                // Add to visibility map
                visibilityMap[key] = bits;
            }
        }
        return visibilityMap;
    }

    public Vector3 GetGridPoint(Vector3 rawCoords) 
    {
        if (gridPoints == null || gridPoints.Count == 0)
        {
            throw new System.ArgumentException("Grid points list is null or empty");
        }

        Vector3 nearestPoint = gridPoints[0];
        float minDistance = Vector3.Distance(rawCoords, nearestPoint);

        foreach (Vector3 gridPoint in gridPoints)
        {
            float distance = Vector3.Distance(rawCoords, gridPoint);
            if (distance < minDistance)
            {
                nearestPoint = gridPoint;
                minDistance = distance;
            }
        }
        return nearestPoint;
    }

    public bool CanPointsSeeEachOther(Vector3 pointA, Vector3 pointB)
    {
        pointA = GetGridPoint(pointA);
        pointB = GetGridPoint(pointB);
        // Check if both points exist in the visibility map
        if (visibilityMap.ContainsKey(pointA) && visibilityMap.ContainsKey(pointB))
        {
            // Get the BitArray for pointA
            BitArray visiblePointsA = visibilityMap[pointA];
            BitArray visiblePointsB = visibilityMap[pointB];

            // Check if pointB is marked as visible in the BitArray
            int pointBIndex = gridPoints.IndexOf(pointB);
            int pointAIndex = gridPoints.IndexOf(pointA);
            if (pointBIndex >= 0 && pointBIndex < visiblePointsA.Length)
            {
                if (pointAIndex >= 0 && pointAIndex < visiblePointsB.Length)
                {
                    return (visiblePointsB[pointAIndex] || visiblePointsA[pointBIndex]);
                }
            }
        }

        // If any point is not in the visibility map or pointB is not visible from pointA, return false
        return false;
    }


    private void OnDrawGizmosSelected()
    {
        if (gridPoints != null)
        {
            foreach (Vector3 point in gridPoints) { Gizmos.DrawSphere(point, 0.1f); }
                                            
        }
    }
}
