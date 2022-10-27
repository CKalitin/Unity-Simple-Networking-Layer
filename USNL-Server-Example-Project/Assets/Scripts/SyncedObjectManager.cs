using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedObjectManager : MonoBehaviour {
    public static SyncedObjectManager instance;

    private List<SyncedObject> syncedObjects = new List<SyncedObject>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Synced Object Manager instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void SendAllSyncedObjectsToClient(int _toClient) {
        for (int i = 0; i < syncedObjects.Count; i++) {
            PacketSend.SyncedObjectInstantiate(_toClient, syncedObjects[i].PrefabId, syncedObjects[i].SyncedObjectUUID, syncedObjects[i].transform.position, syncedObjects[i].transform.rotation, syncedObjects[i].transform.lossyScale);
        }
    }

    private void OnClientConnected(int _clientId) {
        SendAllSyncedObjectsToClient(_clientId);
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
}
