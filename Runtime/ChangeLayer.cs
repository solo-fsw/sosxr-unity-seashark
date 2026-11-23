using UnityEngine;



public class ChangeLayer : MonoBehaviour
{
    [Tooltip("Select exactly one layer.")]
    [SerializeField] private LayerMask m_layerMask;
    private int _originalLayer;

    private void Awake()
    {
        _originalLayer = gameObject.layer;
    }

    [ContextMenu(nameof(Change))]
    public void Change()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        var layer = LayerFromMask(m_layerMask.value);

        if (layer < 0)
        {
            Debug.LogWarning($"{nameof(ChangeLayer)} on {name}: select exactly one layer in the mask. The layer is not changed.", this);

            return;
        }

        gameObject.layer = layer;
    }


    [ContextMenu(nameof(Original))]
    public void Original()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        gameObject.layer = _originalLayer;
    }


    public void Toggle(bool original)
    {
        if (original == true)
        {
            Original();
        }
        else
        {
            Change();
        }
    }


    /// <summary>
    ///     Returns the layer index of a mask that has exactly one layer selected, or -1 when it has none or several.
    /// </summary>
    public static int LayerFromMask(int mask)
    {
        if (mask == 0 || (mask & (mask - 1)) != 0)
        {
            return -1;
        }

        var layer = 0;

        while ((mask & 1) == 0)
        {
            mask >>= 1;
            layer++;
        }

        return layer;
    }
}
