using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZoneController : MonoBehaviour
{
    MeshRenderer meshRenderer;

    public Material[] materials;

    void Awake()
    {
        if (GetComponent<MeshRenderer>() != null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    private void Start()
    {
        if (meshRenderer != null && materials.Length > 0)
        {
            if (PlayerPrefs.GetInt("Stage") <= 3)
            {
                meshRenderer.material = materials[0];
            }
            else
            {
                meshRenderer.material = materials[1];
            }
        }
    }
}
