using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedObjectManager : MonoBehaviour {
    #region Variables

    public static SyncedObjectManager instance;

    [Header("Synced Object Updates")]
    [Tooltip("If Synced Objects should be updated in Vector3s, or Vector2s for a 2D game.")]
    [SerializeField] private bool vector2Mode;
    [Space]
    [Tooltip("Minimum ammount a Synced Object needs to move before it is updated on the Clients.")]
    [SerializeField] private float minPosChange = 0.05f;
    [Tooltip("Minimum ammount a Synced Object needs to rotate before it is updated on the Clients.")]
    [SerializeField] private float minRotChange = 0.05f;
    [Tooltip("Minimum ammount a Synced Object needs to change in scale before it is updated on the Clients.")]
    [SerializeField] private float minScaleChange = 0.05f;

    private List<SyncedObject> syncedObjects = new List<SyncedObject>();

    private List<SyncedObject> syncedObjectVec2PosUpdate = new List<SyncedObject>();
    private List<SyncedObject> syncedObjectVec3PosUpdate = new List<SyncedObject>();
    private List<SyncedObject> syncedObjectRotZUpdate = new List<SyncedObject>();
    private List<SyncedObject> syncedObjectRotUpdate = new List<SyncedObject>();
    private List<SyncedObject> syncedObjectVec2ScaleUpdate = new List<SyncedObject>();
    private List<SyncedObject> syncedObjectVec3ScaleUpdate = new List<SyncedObject>();

    public List<SyncedObject> SyncedObjectVec2PosUpdate { get => syncedObjectVec2PosUpdate; set => syncedObjectVec2PosUpdate = value; }
    public List<SyncedObject> SyncedObjectVec3PosUpdate { get => syncedObjectVec3PosUpdate; set => syncedObjectVec3PosUpdate = value; }
    public List<SyncedObject> SyncedObjectRotZUpdate { get => syncedObjectRotZUpdate; set => syncedObjectRotZUpdate = value; }
    public List<SyncedObject> SyncedObjectRotUpdate { get => syncedObjectRotUpdate; set => syncedObjectRotUpdate = value; }
    public List<SyncedObject> SyncedObjectVec2ScaleUpdate { get => syncedObjectVec2ScaleUpdate; set => syncedObjectVec2ScaleUpdate = value; }
    public List<SyncedObject> SyncedObjectVec3ScaleUpdate { get => syncedObjectVec3ScaleUpdate; set => syncedObjectVec3ScaleUpdate = value; }

    public float MinPosChange { get => minPosChange; set => minPosChange = value; }
    public float MinRotChange { get => minRotChange; set => minRotChange = value; }
    public float MinScaleChange { get => minScaleChange; set => minScaleChange = value; }
    public bool Vector2Mode { get => vector2Mode; set => vector2Mode = value; }

    #endregion

    #region Core

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Synced Object Manager instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void LateUpdate() {
        if (syncedObjectVec2PosUpdate.Count > 0) { SyncedObjectVec2PosUpdates(); }
        if (syncedObjectVec3PosUpdate.Count > 0) { SyncedObjectVec3PosUpdates(); }
        if (syncedObjectRotZUpdate.Count > 0) { SyncedObjectRotZUpdates(); }
        if (syncedObjectRotUpdate.Count > 0) { SyncedObjectRotUpdates(); }
        if (syncedObjectVec2ScaleUpdate.Count > 0) { SyncedObjectVec2ScaleUpdates(); }
        if (syncedObjectVec3ScaleUpdate.Count > 0) { SyncedObjectVec3ScaleUpdates(); }
    }

    private void OnEnable() { USNLCallbackEvents.OnClientConnected += OnClientConnected; }
    private void OnDisable() { USNLCallbackEvents.OnClientConnected -= OnClientConnected; }

    #endregion

    #region Synced Object Management

    private void OnClientConnected(object _clientIdObject) {
        SendAllSyncedObjectsToClient((int)_clientIdObject);
    }

    private void SendAllSyncedObjectsToClient(int _toClient) {
        for (int i = 0; i < syncedObjects.Count; i++) {
            PacketSend.SyncedObjectInstantiate(_toClient, syncedObjects[i].PrefabId, syncedObjects[i].SyncedObjectUUID, syncedObjects[i].transform.position, syncedObjects[i].transform.rotation, syncedObjects[i].transform.lossyScale);
        }
    }

    public void InstantiateSyncedObject(SyncedObject _so) {
        syncedObjects.Add(_so);

        for (int i = 0; i < Server.MaxClients; i++) {
            if (Server.clients[i].IsConnected) {
                PacketSend.SyncedObjectInstantiate(i, _so.PrefabId, _so.SyncedObjectUUID, _so.transform.position, _so.transform.rotation, _so.transform.lossyScale);
            }
        }
    }

    public void DestroySyncedObject(SyncedObject _so) {
        syncedObjects.Remove(_so);

        for (int i = 0; i < Server.MaxClients; i++) {
            if (Server.clients[i].IsConnected) {
                PacketSend.SyncedObjectDestroy(i, _so.SyncedObjectUUID);
            }
        }
    }

    #endregion

    #region Synced Object Updates

    private void SyncedObjectVec2PosUpdates() {
        // 1300 is the max Vector2 updates per packet because of the 4096 byte limit
        int max = 1300; // Max values per packets
        for (int i = 0; i < Mathf.CeilToInt((float)syncedObjectVec2PosUpdate.Count / (float)max); i++) {
            // If there is more than max values to send in this packet, length is max, otherwise length is the amount of values
            int length = syncedObjectVec2PosUpdate.Count;
            if (Mathf.CeilToInt(((float)syncedObjectVec2PosUpdate.Count / (float)max) - i * max) > 1) { length = max; }

            int[] indexes = new int[length];
            Vector2[] values = new Vector2[length];

            for (int x = i * max; x < length + (i * max); x++) {
                indexes[x] = syncedObjectVec2PosUpdate[x].SyncedObjectUUID;
                values[x] = syncedObjectVec2PosUpdate[x].transform.position;
            }

            PacketSend.SyncedObjectVec2PosUpdate(indexes, values);
        }
        syncedObjectVec2PosUpdate.Clear();
    }

    private void SyncedObjectVec3PosUpdates() {
        // 1000 is the max Vector3 updates per packet because of the 4096 byte limit
        int max = 1000; // Max values per packets
        for (int i = 0; i < Mathf.CeilToInt((float)syncedObjectVec3PosUpdate.Count / (float)max); i++) {
            // If there is more than max values to send in this packet, length is max, otherwise length is the amount of values
            int length = syncedObjectVec3PosUpdate.Count;
            if (Mathf.CeilToInt(((float)syncedObjectVec3PosUpdate.Count / (float)max) - i * max) > 1) { length = max; }

            int[] indexes = new int[length];
            Vector3[] values = new Vector3[length];

            for (int x = i * max; x < length + (i * max); x++) {
                indexes[x] = syncedObjectVec3PosUpdate[x].SyncedObjectUUID;
                values[x] = syncedObjectVec3PosUpdate[x].transform.position;
            }

            PacketSend.SyncedObjectVec3PosUpdate(indexes, values);
        }
        syncedObjectVec3PosUpdate.Clear();
    }

    private void SyncedObjectRotZUpdates() {
        // 2000 is the max float updates per packet because of the 4096 byte limit
        int max = 2000; // Max values per packets
        for (int i = 0; i < Mathf.CeilToInt((float)syncedObjectRotZUpdate.Count / (float)max); i++) {
            // If there is more than max values to send in this packet, length is max, otherwise length is the amount of values
            int length = syncedObjectRotZUpdate.Count;
            if (Mathf.CeilToInt(((float)syncedObjectRotZUpdate.Count / (float)max) - i * max) > 1) { length = max; }

            int[] indexes = new int[length];
            float[] values = new float[length];

            for (int x = i * max; x < length + (i * max); x++) {
                indexes[x] = syncedObjectRotZUpdate[x].SyncedObjectUUID;
                values[x] = syncedObjectRotZUpdate[x].transform.rotation.z;
            }

            PacketSend.SyncedObjectRotZUpdate(indexes, values);
        }
        syncedObjectRotZUpdate.Clear();
    }
    
    private void SyncedObjectRotUpdates() {
        // 750 is the max quaternion updates per packet because of the 4096 byte limit
        int max = 750; // Max values per packets
        for (int i = 0; i < Mathf.CeilToInt((float)syncedObjectRotUpdate.Count / (float)max); i++) {
            // If there is more than max values to send in this packet, length is max, otherwise length is the amount of values
            int length = syncedObjectRotUpdate.Count;
            if (Mathf.CeilToInt(((float)syncedObjectRotUpdate.Count / (float)max) - i * max) > 1) { length = max; }

            int[] indexes = new int[length];
            Quaternion[] values = new Quaternion[length];

            for (int x = i * max; x < length + (i * max); x++) {
                indexes[x] = syncedObjectRotUpdate[x].SyncedObjectUUID;
                values[x] = syncedObjectRotUpdate[x].transform.rotation;
            }

            PacketSend.SyncedObjectRotUpdate(indexes, values);
        }
        syncedObjectRotUpdate.Clear();
    }

    private void SyncedObjectVec2ScaleUpdates() {
        // 1300 is the max Vector2 updates per packet because of the 4096 byte limit
        int max = 1300; // Max values per packets
        for (int i = 0; i < Mathf.CeilToInt((float)syncedObjectVec2ScaleUpdate.Count / (float)max); i++) {
            // If there is more than max values to send in this packet, length is max, otherwise length is the amount of values
            int length = syncedObjectVec2ScaleUpdate.Count;
            if (Mathf.CeilToInt(((float)syncedObjectVec2ScaleUpdate.Count / (float)max) - i * max) > 1) { length = max; }

            int[] indexes = new int[length];
            Vector2[] values = new Vector2[length];

            for (int x = i * max; x < length + (i * max); x++) {
                indexes[x] = syncedObjectVec2ScaleUpdate[x].SyncedObjectUUID;
                values[x] = syncedObjectVec2ScaleUpdate[x].transform.lossyScale;
            }

            PacketSend.SyncedObjectVec2ScaleUpdate(indexes, values);
        }
        syncedObjectVec2ScaleUpdate.Clear();
    }

    private void SyncedObjectVec3ScaleUpdates() {
        // 1000 is the max Vector3 updates per packet because of the 4096 byte limit
        int max = 1000; // Max values per packets
        for (int i = 0; i < Mathf.CeilToInt((float)syncedObjectVec3ScaleUpdate.Count / (float)max); i++) {
            // If there is more than max values to send in this packet, length is max, otherwise length is the amount of values
            int length = syncedObjectVec3ScaleUpdate.Count;
            if (Mathf.CeilToInt(((float)syncedObjectVec3ScaleUpdate.Count / (float)max) - i * max) > 1) { length = max; }

            int[] indexes = new int[length];
            Vector3[] values = new Vector3[length];

            for (int x = i * max; x < length + (i * max); x++) {
                indexes[x] = syncedObjectVec3ScaleUpdate[x].SyncedObjectUUID;
                values[x] = syncedObjectVec3ScaleUpdate[x].transform.lossyScale;
            }

            PacketSend.SyncedObjectVec3ScaleUpdate(indexes, values);
        }
        syncedObjectVec3ScaleUpdate.Clear();
    }
    #endregion
}
