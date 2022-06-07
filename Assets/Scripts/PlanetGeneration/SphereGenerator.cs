using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGenerator : MonoBehaviour
{

    [Range(2, 256)]
    public int resolution = 10;
    public float radius;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    SphereFace[] terrainFaces;

    void Initialize()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new SphereFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                meshFilters[i].sharedMesh = mesh;
                meshObj.AddComponent<MeshCollider>().sharedMesh = mesh;
            }
            terrainFaces[i] = new SphereFace(meshFilters[i].sharedMesh, resolution, radius, directions[i]);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
    }

    void GenerateMesh()
    {
        foreach (SphereFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }
}