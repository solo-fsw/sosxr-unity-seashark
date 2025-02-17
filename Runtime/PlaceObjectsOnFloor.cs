using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SimpleHelpers
{
    /// <summary>
    ///     This Unity C# script, PlaceObjectsOnFloor, automates the placement of random objects onto a defined floor within
    ///     the Unity Editor. It allows the user to specify the floor, a list of objects, and other parameters like the number
    ///     of objects to place, their vertical offset, scale range, and the parent transform for organizing the instantiated
    ///     objects
    /// </summary>
    [ExecuteAlways]
    public class PlaceObjectsOnFloor : MonoBehaviour
    {
        [SerializeField] private GameObject m_floor;
        [SerializeField] private List<GameObject> m_objectsToPlaceOnFloor;
        [SerializeField] private int m_numberOfObjectsToPlace = 500;
        [SerializeField] private float m_yOffset = -0.07f;
        [SerializeField] private Vector2 m_scaleRange = new(0.5f, 1.5f);
        [SerializeField] private Transform m_parentObject;


        [ContextMenu(nameof(PlaceObjects))]
        private void PlaceObjects()
        {
            for (var i = 0; i < m_numberOfObjectsToPlace; i++)
            {
                var randomObject = m_objectsToPlaceOnFloor[Random.Range(0, m_objectsToPlaceOnFloor.Count)];
                var randomPosition = new Vector3(Random.Range(-m_floor.transform.localScale.x / 2, m_floor.transform.localScale.x / 2), m_yOffset, Random.Range(-m_floor.transform.localScale.z / 2, m_floor.transform.localScale.z / 2));
                var randomScale = Random.Range(m_scaleRange.x, m_scaleRange.y);

                var newObject = Instantiate(randomObject, m_floor.transform.position + randomPosition, Quaternion.identity, m_parentObject);
                newObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            }
        }
    }
}