using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneration : MonoBehaviour
{

    const string AMOUNT = "_Amount";
    const string RADIUS = "_Radius";
    const float MAX = 0.5f;

    List<Vine> branchNodes;
    
    Mesh mesh;
    Material material;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;


    float branchRadius = 0.02f;
    int meshFaces = 8;

    float growthSpeed = 2;
    float currentAmount = -1;

    public void init(List<Vine> branchNodes, float branchRadius, Material material)
    {
        this.branchNodes = branchNodes;
        this.branchRadius = branchRadius;
        this.material = new Material(material);
        mesh = createMesh(branchNodes);
    }



    void Start()
    {

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        if (material == null)
        {
            material = new Material(Shader.Find("Specular"));
        }


        meshRenderer.material = material;
        if (mesh != null)
        {
            meshFilter.mesh = mesh;
        }

        material.SetFloat(RADIUS, branchRadius);
        material.SetFloat(AMOUNT, currentAmount);
    }

    float remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
    {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }

    Mesh createMesh(List<Vine> nodes)
    {
        Mesh branchMesh = new Mesh();

        Vector3[] vertices = new Vector3[(nodes.Count) * meshFaces * 4];
        Vector3[] normals = new Vector3[nodes.Count * meshFaces * 4];
        Vector2[] uv = new Vector2[nodes.Count * meshFaces * 4];
        int[] triangles = new int[(nodes.Count - 1) * meshFaces * 6];

        for (int i = 0; i < nodes.Count; i++)
        {
            float vStep = (2f * Mathf.PI) / meshFaces;

            var fw = Vector3.zero;
            if (i > 0)
            {
                fw = branchNodes[i - 1].getPosition() - branchNodes[i].getPosition();
            }

            if (i < branchNodes.Count - 1)
            {
                fw += branchNodes[i].getPosition() - branchNodes[i + 1].getPosition();
            }

            if (fw == Vector3.zero)
            {
                fw = Vector3.forward;
            }

            fw.Normalize();

            var up = branchNodes[i].getNormal();
            up.Normalize();

            for (int v = 0; v < meshFaces; v++)
            {
                var orientation = Quaternion.LookRotation(fw, up);
                Vector3 xAxis = Vector3.up;
                Vector3 yAxis = Vector3.right;
                Vector3 pos = branchNodes[i].getPosition();
                pos += orientation * xAxis * (branchRadius * Mathf.Sin(v * vStep));
                pos += orientation * yAxis * (branchRadius * Mathf.Cos(v * vStep));

                vertices[i * meshFaces + v] = pos;

                var diff = pos - branchNodes[i].getPosition();
                normals[i * meshFaces + v] = diff / diff.magnitude;

                float uvID = remap(i, 0, nodes.Count - 1, 0, 1);
                uv[i * meshFaces + v] = new Vector2((float)v / meshFaces, uvID);
            }

            if (i + 1 < nodes.Count)
            {
                for (int v = 0; v < meshFaces; v++)
                {
                    triangles[i * meshFaces * 6 + v * 6] = ((v + 1) % meshFaces) + i * meshFaces;
                    triangles[i * meshFaces * 6 + v * 6 + 1] = triangles[i * meshFaces * 6 + v * 6 + 4] = v + i * meshFaces;
                    triangles[i * meshFaces * 6 + v * 6 + 2] = triangles[i * meshFaces * 6 + v * 6 + 3] = ((v + 1) % meshFaces + meshFaces) + i * meshFaces;
                    triangles[i * meshFaces * 6 + v * 6 + 5] = (meshFaces + v % meshFaces) + i * meshFaces;
                }
            }
        }

        branchMesh.vertices = vertices;
        branchMesh.triangles = triangles;
        branchMesh.normals = normals;
        branchMesh.uv = uv;
        return branchMesh;
    }
}
