using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedObjectManager : MonoBehaviour {
    #region Core

    [SerializeField] private SyncedObjectPrefabs syncedObjectsPrefabs;

    private Dictionary<int, Transform> syncedObjects = new Dictionary<int, Transform>();

    private void OnEnable() {
        USNLCallbackEvents.OnSyncedObjectInstantiatePacket += OnSyncedObjectInstantiatePacket;
        USNLCallbackEvents.OnSyncedObjectDestroyPacket += OnSyncedObjectDestroyPacket;
        USNLCallbackEvents.OnSyncedObjectVec2PosUpdatePacket += OnSyncedObjectVec2PosUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec3PosUpdatePacket += OnSyncedObjectVec3PosUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectRotZUpdatePacket += OnSyncedObjectRotZUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectRotUpdatePacket += OnSyncedObjectRotUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec2ScaleUpdatePacket += OnSyncedObjectVec2ScaleUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec3ScaleUpdatePacket += OnSyncedObjectVec3ScaleUpdatePacket;
    }

    private void OnDisable() {
        USNLCallbackEvents.OnSyncedObjectInstantiatePacket -= OnSyncedObjectInstantiatePacket;
        USNLCallbackEvents.OnSyncedObjectDestroyPacket -= OnSyncedObjectDestroyPacket;
        USNLCallbackEvents.OnSyncedObjectVec2PosUpdatePacket -= OnSyncedObjectVec2PosUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec3PosUpdatePacket -= OnSyncedObjectVec3PosUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectRotZUpdatePacket -= OnSyncedObjectRotZUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectRotUpdatePacket -= OnSyncedObjectRotUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec2ScaleUpdatePacket -= OnSyncedObjectVec2ScaleUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec3ScaleUpdatePacket -= OnSyncedObjectVec3ScaleUpdatePacket;
    }

    #endregion

    #region Synced Objects

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

    private void OnSyncedObjectVec2PosUpdatePacket(object _packetObject) {
        SyncedObjectVec2PosUpdatePacket _packet = (SyncedObjectVec2PosUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].position = new Vector3(_packet.Positions[i].x, _packet.Positions[i].y, syncedObjects[_packet.SyncedObjectUUIDs[i]].position.z);
            }
        }
    }

    private void OnSyncedObjectVec3PosUpdatePacket(object _packetObject) {
        SyncedObjectVec3PosUpdatePacket _packet = (SyncedObjectVec3PosUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].position = _packet.Positions[i];
            }
        }
    }

    private void OnSyncedObjectRotZUpdatePacket(object _packetObject) {
        SyncedObjectRotZUpdatePacket _packet = (SyncedObjectRotZUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation = new Quaternion(syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation.x, syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation.y, _packet.Rotations[i], syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation.w);
            }
        }
    }

    private void OnSyncedObjectRotUpdatePacket(object _packetObject) {
        SyncedObjectRotUpdatePacket _packet = (SyncedObjectRotUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation = _packet.Rotations[i];
            }
        }
    }

    private void OnSyncedObjectVec2ScaleUpdatePacket(object _packetObject) {
        SyncedObjectVec2ScaleUpdatePacket _packet = (SyncedObjectVec2ScaleUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].localScale = new Vector3(_packet.Scales[i].x, _packet.Scales[i].y, syncedObjects[_packet.SyncedObjectUUIDs[i]].localScale.z);
            }
        }
    }

    private void OnSyncedObjectVec3ScaleUpdatePacket(object _packetObject) {
        SyncedObjectVec2ScaleUpdatePacket _packet = (SyncedObjectVec2ScaleUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].localScale = _packet.Scales[i];
            }
        }
    }

    #endregion
}
