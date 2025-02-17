using System;
using UnityEngine;


public class RendererDefaultColorManager : MonoBehaviour
{
    [SerializeField] private Renderer[] m_includedRenderers = Array.Empty<Renderer>();
    [SerializeField] private bool setDefaultColor = false;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private bool changeAlpha = false;


    private void OnValidate()
    {
        if (m_includedRenderers is {Length: > 0})
        {
            return;
        }

        m_includedRenderers = GetComponentsInChildren<Renderer>();
        Debug.Log("RendererDefaultColorManager: Automatically populated includedRenderers with all child renderers.");
    }


    [ContextMenu(nameof(SetDefaultColor))]
    public void SetDefaultColor()
    {
        if (!setDefaultColor)
        {
            return;
        }

        foreach (var rend in m_includedRenderers)
        {
            foreach (var material in rend.sharedMaterials)
            {
                SetMaterialColor(material);
            }
        }
    }


    private void SetMaterialColor(Material material)
    {
        if (changeAlpha)
        {
            material.color = defaultColor;
        }
        else
        {
            var color = defaultColor;
            color.a = material.color.a;
            material.color = color;
        }
    }
}