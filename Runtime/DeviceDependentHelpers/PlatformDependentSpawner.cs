using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class PlatformDependentSpawner : MonoBehaviour
{
    [SerializeField] private CurrentDevice m_currentDevice;
    [SerializeField] private bool m_autoStart;

    [Space(20)]
    [SerializeField] private float m_spawnDelay;
    [SerializeField] private GameObject m_screenPrefab;
    [SerializeField] private GameObject m_openXRPrefab;
    [SerializeField] private Vector3 m_openXRPrefabOffset = Vector3.up * 2;

    [Space(20)]
    [SerializeField] private bool m_spawnRigPrefab = true;
    [SerializeField] private GameObject m_rigPrefab;

    [Space(20)]
    [SerializeField] private bool m_spawnEventSystems;
    [SerializeField] private GameObject m_screenEventSystemPrefab;
    [SerializeField] private GameObject m_xrEventSystemPrefab;

    [Space(20)]
    [SerializeField] private bool m_destroyOnPlayerSpawn = true;


    [Space(20)]
    [SerializeField] private UnityEvent m_eventOnCreation;
    [SerializeField] private List<string> m_destroyOnSpawnPlayerName = new() {"Overseer", "XR Origin"};

    private GameObject _eventSystemGo;
    private GameObject _rig;
    private GameObject _spawnedObject;

    public bool SpawnRigPrefab
    {
        set => m_spawnRigPrefab = value;
    }

    public bool IsValid { get; private set; }


    public void OnValidate()
    {
        IsValid = true;
    }


    public void OnEnable()
    {
        if (m_autoStart)
        {
            SelectCorrectUIToSpawn();
        }

        // EventsSystem.PlayerInstantiated += CheckForPlayerNames;
    }


    private void CheckForPlayerNames(string playerName)
    {
        if (!m_destroyOnPlayerSpawn)
        {
            return;
        }

        Debug.Log("We should destroy something on " + playerName);

        foreach (var unused in m_destroyOnSpawnPlayerName.Where(playerName.Contains))
        {
            DestroyAllObjects();
        }
    }


    [ContextMenu(nameof(SelectCorrectUIToSpawn))]
    public void SelectCorrectUIToSpawn()
    {
        DestroyAllObjects();

        if (m_currentDevice.Current == Device.HMD)
        {
            SpawnOnOpenXR();
        }
        else
        {
            SpawnScreenBased();
        }
    }


    [ContextMenu(nameof(SpawnOnOpenXR))]
    private void SpawnOnOpenXR()
    {
        Invoke(nameof(SpawnOpenXRObjects), m_spawnDelay);
    }


    [ContextMenu(nameof(SpawnScreenBased))]
    private void SpawnScreenBased()
    {
        Invoke(nameof(SpawnScreenBasedObjects), m_spawnDelay);
    }


    private void SpawnOpenXRObjects()
    {
        _eventSystemGo = InstantiateRequiredObject(m_xrEventSystemPrefab, m_spawnEventSystems);

        _rig = InstantiateRequiredObject(m_rigPrefab, m_spawnRigPrefab);

        if (_spawnedObject != null)
        {
            return;
        }

        _spawnedObject = InstantiateRequiredObject(m_openXRPrefab);
        _spawnedObject.transform.position += m_openXRPrefabOffset;
    }


    private void SpawnScreenBasedObjects()
    {
        _eventSystemGo = InstantiateRequiredObject(m_screenEventSystemPrefab, m_spawnEventSystems);

        if (_spawnedObject != null)
        {
            return;
        }

        _spawnedObject = InstantiateRequiredObject(m_screenPrefab);
    }


    private GameObject InstantiateRequiredObject(GameObject go, bool toSpawn = true)
    {
        if (toSpawn == false)
        {
            return null;
        }

        var spawnedObject = Instantiate(go);

        Debug.Log("Spawned " + go.name);

        m_eventOnCreation?.Invoke();

        return spawnedObject;
    }


    public void DestroyAllObjects()
    {
        DestroyEventSystem();

        DestroyRig();

        DestroySpawnedObject();
    }


    public void DestroyEventSystem()
    {
        if (_eventSystemGo != null)
        {
            Destroy(_eventSystemGo);
        }
    }


    public void DestroyRig()
    {
        if (_rig != null)
        {
            Destroy(_rig);
        }
    }


    public void DestroySpawnedObject()
    {
        if (_spawnedObject != null)
        {
            Destroy(_spawnedObject);
        }
    }


    private void OnDisable()
    {
        // EventsSystem.PlayerInstantiated -= CheckForPlayerNames;
    }
}