using UnityEngine;


[RequireComponent(typeof(FindCollidersInScene))]
public class DeleteColliders : MonoBehaviour
{
    [SerializeField] [HideInInspector] private FindCollidersInScene _findColliders;


    private void OnValidate()
    {
        if (_findColliders == null)
        {
            _findColliders = GetComponent<FindCollidersInScene>();
        }
    }


    [ContextMenu(nameof(DeleteAllColliders))]
    private void DeleteAllColliders()
    {
        foreach (var coll in _findColliders.FilteredColliders)
        {
            if (Application.isPlaying)
            {
                Destroy(coll);
            }
            else
            {
                DestroyImmediate(coll);
            }
        }

        _findColliders.FilteredColliders = null;
    }
}