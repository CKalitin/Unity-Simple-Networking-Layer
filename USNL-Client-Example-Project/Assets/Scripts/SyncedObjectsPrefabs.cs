using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SyncedObjectPrefabs", menuName = "USNL/Synced Object Prefabs", order = 0)]
public class SyncedObjectsPrefabs : ScriptableObject {
    [Tooltip("The Index of an object here is the Id of a Synced Object on the Server.")]
    [SerializeField] private GameObject[] syncedObjectPrefabs;

    public GameObject[] SyncedObjectPrefabs { get => syncedObjectPrefabs; set => syncedObjectPrefabs = value; }
}
