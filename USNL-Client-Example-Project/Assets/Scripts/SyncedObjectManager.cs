using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedObjectManager : MonoBehaviour {
    [SerializeField] private SyncedObjectPrefabs syncedObjectsPrefabs;

    private Dictionary<int, Transform> syncedObjects = new Dictionary<int, Transform>();

    private void OnEnable() {
        USNLCallbackEvents.OnSyncedObjectInstantiatePacket += OnSyncedObjectInstantiatePacket;
        USNLCallbackEvents.OnSyncedObjectDestroyPacket += OnSyncedObjectDestroyPacket;
        USNLCallbackEvents.OnSyncedObjectUpdatePacket += OnSyncedObjectUpdatePacket;
    }

    private void OnDisable() {
        USNLCallbackEvents.OnSyncedObjectInstantiatePacket -= OnSyncedObjectInstantiatePacket;
        USNLCallbackEvents.OnSyncedObjectDestroyPacket -= OnSyncedObjectDestroyPacket;
        USNLCallbackEvents.OnSyncedObjectUpdatePacket -= OnSyncedObjectUpdatePacket;
    }

    private void OnSyncedObjectInstantiatePacket(object _packetObject) {
        SyncedObjectInstantiatePacket _packet = (SyncedObjectInstantiatePacket)_packetObject;

        GameObject newSyncedObject = Instantiate(syncedObjectsPrefabs.SyncedObjectsPrefabs[_packet.SyncedObjectPrefebId], _packet.Position, _packet.Rotation);
        newSyncedObject.transform.localScale = _packet.Scale;

        // If object does not have a Syncede Object Component
        if (newSyncedObject.GetComponent<SyncedObject>() == null) { newSyncedObject.gameObject.AddComponent<SyncedObject>(); }
        newSyncedObject.gameObject.GetComponent<SyncedObject>().SyncedObjectUuid = _packet.SyncedObjectUUID;

        syncedObjects.Add(_packet.SyncedObjectUUID, newSyncedObject.transform);
    }

    private void OnSyncedObjectDestroyPacket(object _packetObject) {
        SyncedObjectDestroyPacket _packet = (SyncedObjectDestroyPacket)_packetObject;

        Destroy(syncedObjects[_packet.SyncedObjectUUID].gameObject);
        syncedObjects.Remove(_packet.SyncedObjectUUID);
    }

    private void OnSyncedObjectUpdatePacket(object _packetObject) {
        SyncedObjectUpdatePacket _packet = (SyncedObjectUpdatePacket)_packetObject;

        if (syncedObjects.ContainsKey(_packet.SyncedObjectUUID)) {
            syncedObjects[_packet.SyncedObjectUUID].position = _packet.Position;
            syncedObjects[_packet.SyncedObjectUUID].rotation = _packet.Rotation;
            syncedObjects[_packet.SyncedObjectUUID].localScale = _packet.Scale;
        }
    }
}
