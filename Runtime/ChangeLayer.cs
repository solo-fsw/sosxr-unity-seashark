using UnityEngine;



public class ChangeLayer : MonoBehaviour
{
    [SerializeField] private LayerMask m_layerMask;
    private LayerMask _originalLayer;

    private void Awake()
    {
        _originalLayer = transform.gameObject.layer;
    }

    [ContextMenu(nameof(Change))]
    public void Change()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        transform.gameObject.layer = (int)Mathf.Log(m_layerMask.value, 2);
    }


    [ContextMenu(nameof(Original))]
    public void Original()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        transform.gameObject.layer = (int)Mathf.Log(_originalLayer.value, 2);
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
}

