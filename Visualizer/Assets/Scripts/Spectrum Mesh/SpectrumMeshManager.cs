using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumMeshManager : MonoBehaviour
{
    [Header("Mesh Settings")]
    [SerializeField] private int xSize = 10;
    [SerializeField] private int ySize = 10;
    [SerializeField] private float sampleMultiplier = 100.0f;
    [SerializeField] private float amplitude = 1.0f;
    [SerializeField] private float scale = 1.0f;

    [Header("Materials")]
    [SerializeField] private Material regularMaterial = null;
    [SerializeField] private Material wireframeMaterial = null;

    private SpectrumManager spectrumManager = null;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;

    private float elapsedTime = 0.0f;
    private bool isWireframe = false;

    private void Start()
    {
        spectrumManager = SpectrumManager.Instance;

        // Create and assign mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Create shape and set UVs
        CreateShape(xSize, ySize);
        SetUVs();

        // Create wireframe material
        wireframeMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        wireframeMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        wireframeMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        wireframeMaterial.SetInt("_ZWrite", 0);
        wireframeMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        wireframeMaterial.SetFloat("_LineWidth", 1.0f);

        // Set the mesh topology to triangles
        mesh.SetIndices(mesh.GetIndices(0), MeshTopology.Triangles, 0);

        // Set the mesh renderer material to the regular material by default
        GetComponent<MeshRenderer>().material = regularMaterial;
    }
    private void Update()
    {
        // Toggle wireframe mode on/off
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isWireframe = !isWireframe;
            mesh.SetIndices(mesh.GetIndices(0), isWireframe ? MeshTopology.Lines : MeshTopology.Triangles, 0);
            GetComponent<MeshRenderer>().material = isWireframe ? wireframeMaterial : regularMaterial;
        }

        // Check for missing materials or spectrum manager
        if (regularMaterial == null || wireframeMaterial == null || spectrumManager == null)
        {
            return;
        }



        // Update mesh vertices based on spectrum data
        for (int i = 0; i < vertices.Length; i++)
        {
            float sample = spectrumManager.Samples[i % spectrumManager.Samples.Length] * sampleMultiplier;
            float scaledSample = amplitude * scale * Mathf.Clamp01(sample);
            vertices[i].y = scaledSample;
        }

        mesh.vertices = vertices;

        // Update mesh animation
        elapsedTime += Time.deltaTime;

        // Smooth mesh vertices
        SmoothMesh();
    }

    public void CreateShape(int xSize, int ySize)
    {
        this.xSize = xSize;
        this.ySize = ySize;

        // Update the size of the vertices array
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];

        int index = 0;

        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float xPos = (float)x / xSize;
                float yPos = (float)y / ySize;
                float zPos = 0.0f;

                vertices[index] = new Vector3(xPos, yPos, zPos);
                index++;
            }
        }

        int numTriangles = xSize * ySize * 6;
        triangles = new int[numTriangles];

        int vert = 0;
        int tris = 0;

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }

            vert++;
        }

        // Set the vertices array on the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }


    private void SetUVs()
    {
        uvs = new Vector2[vertices.Length];

        int index = 0;

        for (int y = 0; y <= ySize; y++)

        {
            for (int x = 0; x <= xSize; x++)
            {
                float u = (float)x / xSize;

                float v = (float)y / ySize;
                uvs[index] = new Vector2(u, v);
                index++;
            }
        }

        mesh.uv = uvs;
    }

    private void SmoothMesh()
    {
        Vector3[] smoothedVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            smoothedVertices[i] = vertices[i];
        }

        // Apply Gaussian smoothing to all vertices except those on the edges
        float[,] kernel = GaussianKernel(5, 1.0f);
        for (int y = 2; y < ySize - 1; y++)
        {
            for (int x = 2; x < xSize - 1; x++)
            {
                int index = y * (xSize + 1) + x;
                Vector3 sum = Vector3.zero;
                float weightSum = 0.0f;
                for (int ky = -2; ky <= 2; ky++)
                {
                    for (int kx = -2; kx <= 2; kx++)
                    {
                        int neighborIndex = (y + ky) * (xSize + 1) + (x + kx);
                        float weight = kernel[ky + 2, kx + 2];
                        sum += vertices[neighborIndex] * weight;
                        weightSum += weight;
                    }
                }
                smoothedVertices[index] = sum / weightSum;
            }
        }

        vertices = smoothedVertices;
    }

    private float[,] GaussianKernel(int size, float sigma)
    {
        float[,] kernel = new float[size, size];
        float sum = 0.0f;
        int radius = size / 2;
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                float weight = Mathf.Exp(-(x * x + y * y) / (2 * sigma * sigma));
                kernel[y + radius, x + radius] = weight;
                sum += weight;
            }
        }
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                kernel[y, x] /= sum;
            }
        }
        return kernel;
    }

}