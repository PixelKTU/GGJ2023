using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineGroth : MonoBehaviour
{
    MeshRenderer MR;
    Material[] materials;
    float time;
    public float secondryScale = 1;
    void Start()
    {
        MR = GetComponent<MeshRenderer>();
        materials = MR.materials;
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Material material in materials)
        {
            material.SetFloat("_TimeInstance", Time.time - time/secondryScale);
            if (material.GetFloat("_TimeInstance") * material.GetFloat("_GrowScale") > 8f)
            {
                Destroy(this);
            }
        }

    }
}
