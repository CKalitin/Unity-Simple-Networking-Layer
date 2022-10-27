using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedObjectManager : MonoBehaviour {
    [SerializeField] private SyncedObjectsPrefabs syncedObjectsPrefabs;

    private Dictionary<int, Transform> syncedObjects = new Dictionary<int, Transform>();
    
    private void OnSyncedObjectInstantiatePacket(SyncedObjectInstantiatePacket _packet) {
        GameObject newSyncedObject = Instantiate(syncedObjectsPrefabs.SyncedObjectPrefabs[_packet.SyncedObjectPrefebId], _packet.Position, _packet.Rotation);
        newSyncedObject.transform.localScale = _packet.Scale;

        // If object does not have a Syncede Object Component
        if (newSyncedObject.GetComponent<SyncedObject>() == null) { newSyncedObject.gameObject.AddComponent<SyncedObject>(); }
        newSyncedObject.gameObject.GetComponent<SyncedObject>().SyncedObjectUuid = _packet.SyncedObjectUUID;

        syncedObjects.Add(_packet.SyncedObjectUUID, newSyncedObject.transform);
    }

    private void OnSyncedObjectDestroyPacket(SyncedObjectDestroyPacket _packet) {
        Destroy(syncedObjects[_packet.SyncedObjectUUID].gameObject);
        syncedObjects.Remove(_packet.SyncedObjectUUID);
    }

    private void OnSyncedObjectUpdatePacket(SyncedObjectUpdatePacket _packet) {
        if (syncedObjects.ContainsKey(_packet.SyncedObjectUUID)) {
            syncedObjects[_packet.SyncedObjectUUID].position = _packet.Position;
            syncedObjects[_packet.SyncedObjectUUID].rotation = _packet.Rotation;
            syncedObjects[_packet.SyncedObjectUUID].localScale = _packet.Scale;
        }
    }
}
