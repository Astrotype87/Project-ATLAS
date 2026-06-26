using UnityEngine;

[ExecuteAlways]  // <--- Makes it run in Edit Mode and Play Mode
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneMeshGenerator : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Plane Settings")]
    public float width = 1f;
    public float height = 1f;
    public int widthSegments = 1;
    public int heightSegments = 1;
    public Axis facingAxis = Axis.Y;
    public bool flip = false;

    private MeshFilter meshFilter;

    void OnValidate() => GeneratePlane();  // Regenerate when you change settings in Inspector
    void Awake() => GeneratePlane();       // Regenerate when entering Play Mode

    public void GeneratePlane()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh.name = "Procedural Plane";

        int xCount = widthSegments + 1;
        int yCount = heightSegments + 1;
        int vertexCount = xCount * yCount;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[widthSegments * heightSegments * 6];

        float xStep = width / widthSegments;
        float yStep = height / heightSegments;

        int v = 0;
        for (int y = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++)
            {
                float xPos = -width / 2f + x * xStep;
                float yPos = -height / 2f + y * yStep;

                switch (facingAxis)
                {
                    case Axis.Y: vertices[v] = new Vector3(xPos, 0, yPos); break; // XZ plane
                    case Axis.X: vertices[v] = new Vector3(0, yPos, xPos); break; // YZ plane
                    case Axis.Z: vertices[v] = new Vector3(xPos, yPos, 0); break; // XY plane
                }

                uvs[v] = new Vector2((float)x / widthSegments, (float)y / heightSegments);
                v++;
            }
        }

        int t = 0;
        for (int y = 0; y < heightSegments; y++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                int i0 = y * xCount + x;
                int i1 = i0 + 1;
                int i2 = i0 + xCount;
                int i3 = i2 + 1;

                if (flip)
                {
                    triangles[t++] = i0; triangles[t++] = i2; triangles[t++] = i1;
                    triangles[t++] = i1; triangles[t++] = i2; triangles[t++] = i3;
                }
                else
                {
                    triangles[t++] = i0; triangles[t++] = i1; triangles[t++] = i2;
                    triangles[t++] = i1; triangles[t++] = i3; triangles[t++] = i2;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
    }
}
