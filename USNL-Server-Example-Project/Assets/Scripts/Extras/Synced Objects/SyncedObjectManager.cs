using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedObjectManager : MonoBehaviour {
    #region Variables

    public static SyncedObjectManager instance;

    private List<SyncedObject> syncedObjects = new List<SyncedObject>();

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
}
