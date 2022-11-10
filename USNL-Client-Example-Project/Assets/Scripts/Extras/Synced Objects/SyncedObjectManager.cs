using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedObjectManager : MonoBehaviour {
    #region Core

    public static SyncedObjectManager instance;

    [SerializeField] private SyncedObjectPrefabs syncedObjectsPrefabs;
    
    private Dictionary<int, Transform> syncedObjects = new Dictionary<int, Transform>();

    private bool localInterpolation = false;

    public bool LocalInterpolation { get => localInterpolation; set => localInterpolation = value; }

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Synced Object Manager instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void OnEnable() {
        USNLCallbackEvents.OnDisconnected += DisconnectedFromServer;

        USNLCallbackEvents.OnSyncedObjectInterpolationModePacket += OnSyncedObjectInterpolationModePacket;

        USNLCallbackEvents.OnSyncedObjectInstantiatePacket += OnSyncedObjectInstantiatePacket;
        USNLCallbackEvents.OnSyncedObjectDestroyPacket += OnSyncedObjectDestroyPacket;

        USNLCallbackEvents.OnSyncedObjectVec2PosUpdatePacket += OnSyncedObjectVec2PosUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec3PosUpdatePacket += OnSyncedObjectVec3PosUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectRotZUpdatePacket += OnSyncedObjectRotZUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectRotUpdatePacket += OnSyncedObjectRotUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec2ScaleUpdatePacket += OnSyncedObjectVec2ScaleUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec3ScaleUpdatePacket += OnSyncedObjectVec3ScaleUpdatePacket;

        USNLCallbackEvents.OnSyncedObjectVec2PosInterpolationPacket += OnSyncedObjectVec2PosInterpolationPacket;
        USNLCallbackEvents.OnSyncedObjectVec3PosInterpolationPacket += OnSyncedObjectVec3PosInterpolationPacket;
        USNLCallbackEvents.OnSyncedObjectRotZInterpolationPacket += OnSyncedObjectRotZInterpolationPacket;
        USNLCallbackEvents.OnSyncedObjectRotInterpolationPacket += OnSyncedObjectRotInterpolationPacket;
        USNLCallbackEvents.OnSyncedObjectVec2ScaleInterpolationPacket += OnSyncedObjectVec2ScaleInterpolationPacket;
        USNLCallbackEvents.OnSyncedObjectVec3ScaleInterpolationPacket += OnSyncedObjectVec3ScaleInterpolationPacket;
    }

    private void OnDisable() {
        USNLCallbackEvents.OnDisconnected -= DisconnectedFromServer;

        USNLCallbackEvents.OnSyncedObjectInterpolationModePacket -= OnSyncedObjectInterpolationModePacket;

        USNLCallbackEvents.OnSyncedObjectInstantiatePacket -= OnSyncedObjectInstantiatePacket;
        USNLCallbackEvents.OnSyncedObjectDestroyPacket -= OnSyncedObjectDestroyPacket;

        USNLCallbackEvents.OnSyncedObjectVec2PosUpdatePacket -= OnSyncedObjectVec2PosUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec3PosUpdatePacket -= OnSyncedObjectVec3PosUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectRotZUpdatePacket -= OnSyncedObjectRotZUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectRotUpdatePacket -= OnSyncedObjectRotUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec2ScaleUpdatePacket -= OnSyncedObjectVec2ScaleUpdatePacket;
        USNLCallbackEvents.OnSyncedObjectVec3ScaleUpdatePacket -= OnSyncedObjectVec3ScaleUpdatePacket;

        USNLCallbackEvents.OnSyncedObjectVec2PosInterpolationPacket -= OnSyncedObjectVec2PosInterpolationPacket;
        USNLCallbackEvents.OnSyncedObjectVec3PosInterpolationPacket -= OnSyncedObjectVec3PosInterpolationPacket;
        USNLCallbackEvents.OnSyncedObjectRotZInterpolationPacket -= OnSyncedObjectRotZInterpolationPacket;
        USNLCallbackEvents.OnSyncedObjectRotInterpolationPacket -= OnSyncedObjectRotInterpolationPacket;
        USNLCallbackEvents.OnSyncedObjectVec2ScaleInterpolationPacket -= OnSyncedObjectVec2ScaleInterpolationPacket;
        USNLCallbackEvents.OnSyncedObjectVec3ScaleInterpolationPacket -= OnSyncedObjectVec3ScaleInterpolationPacket;
    }

    #endregion

    #region Synced Object Management

    private void OnSyncedObjectInterpolationModePacket(object _packetObject) {
        SyncedObjectInterpolationModePacket packet = (SyncedObjectInterpolationModePacket)_packetObject;
        localInterpolation = !packet.ServerInterpolation;
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

    private void DisconnectedFromServer(object _object) {
        ClearLocalSyncedObjects();
    }

    private void ClearLocalSyncedObjects() {
        if (syncedObjects.Count <= 0) return;
        for (int i = 0; i < syncedObjects.Count; i++) {
            Destroy(syncedObjects[i].gameObject);
        }
        syncedObjects.Clear();
    }

    #endregion

    #region Synced Object Updates

    private void OnSyncedObjectVec2PosUpdatePacket(object _packetObject) {
        SyncedObjectVec2PosUpdatePacket _packet = (SyncedObjectVec2PosUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].position = new Vector3(_packet.Positions[i].x, _packet.Positions[i].y, syncedObjects[_packet.SyncedObjectUUIDs[i]].position.z);
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().PositionUpdate(_packet.Positions[i]);
            }
        }
    }

    private void OnSyncedObjectVec3PosUpdatePacket(object _packetObject) {
        SyncedObjectVec3PosUpdatePacket _packet = (SyncedObjectVec3PosUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].position = _packet.Positions[i];
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().PositionUpdate(_packet.Positions[i]);
            }
        }
    }

    private void OnSyncedObjectRotZUpdatePacket(object _packetObject) {
        SyncedObjectRotZUpdatePacket _packet = (SyncedObjectRotZUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation = Quaternion.RotateTowards(syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation, Quaternion.Euler(new Vector3(syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation.x, syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation.y, _packet.Rotations[i])), 999999f);
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().RotationUpdate(new Vector3(syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation.x, syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation.y, _packet.Rotations[i]));
            }
        }
    }

    private void OnSyncedObjectRotUpdatePacket(object _packetObject) {
        SyncedObjectRotUpdatePacket _packet = (SyncedObjectRotUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation = Quaternion.RotateTowards(syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation, Quaternion.Euler(_packet.Rotations[i]), 999999f);
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().RotationUpdate(_packet.Rotations[i]);
            }
        }
    }

    private void OnSyncedObjectVec2ScaleUpdatePacket(object _packetObject) {
        SyncedObjectVec2ScaleUpdatePacket _packet = (SyncedObjectVec2ScaleUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].localScale = new Vector3(_packet.Scales[i].x, _packet.Scales[i].y, syncedObjects[_packet.SyncedObjectUUIDs[i]].localScale.z);
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().ScaleUpdate(_packet.Scales[i]);
            }
        }
    }

    private void OnSyncedObjectVec3ScaleUpdatePacket(object _packetObject) {
        SyncedObjectVec3ScaleUpdatePacket _packet = (SyncedObjectVec3ScaleUpdatePacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].localScale = _packet.Scales[i];
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().ScaleUpdate(_packet.Scales[i]);
            }
        }
    }

    #endregion

    #region Synced Object Interpolation Packets

    private void OnSyncedObjectVec2PosInterpolationPacket(object _packetObject) {
        SyncedObjectVec2PosInterpolationPacket _packet = (SyncedObjectVec2PosInterpolationPacket)_packetObject;
        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().PositionInterpolationUpdate(_packet.InterpolatePositions[i]);
            }
        }
    }

    private void OnSyncedObjectVec3PosInterpolationPacket(object _packetObject) {
        SyncedObjectVec3PosInterpolationPacket _packet = (SyncedObjectVec3PosInterpolationPacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().PositionInterpolationUpdate(_packet.InterpolatePositions[i]);
            }
        }
    }

    private void OnSyncedObjectRotZInterpolationPacket(object _packetObject) {
        SyncedObjectRotZInterpolationPacket _packet = (SyncedObjectRotZInterpolationPacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().RotationInterpolationUpdate(new Vector3(syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation.x, syncedObjects[_packet.SyncedObjectUUIDs[i]].rotation.y, _packet.InterpolateRotations[i]));
            }
        }
    }

    private void OnSyncedObjectRotInterpolationPacket(object _packetObject) {
        SyncedObjectRotInterpolationPacket _packet = (SyncedObjectRotInterpolationPacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().RotationInterpolationUpdate(_packet.InterpolateRotations[i]);
            }
        }
    }

    private void OnSyncedObjectVec2ScaleInterpolationPacket(object _packetObject) {
        SyncedObjectVec2ScaleInterpolationPacket _packet = (SyncedObjectVec2ScaleInterpolationPacket)_packetObject;
        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().ScaleInterpolationUpdate(_packet.InterpolateScales[i]);
            }
        }
    }

    private void OnSyncedObjectVec3ScaleInterpolationPacket(object _packetObject) {
        SyncedObjectVec3ScaleInterpolationPacket _packet = (SyncedObjectVec3ScaleInterpolationPacket)_packetObject;

        for (int i = 0; i < _packet.SyncedObjectUUIDs.Length; i++) {
            if (syncedObjects.ContainsKey(_packet.SyncedObjectUUIDs[i])) {
                syncedObjects[_packet.SyncedObjectUUIDs[i]].GetComponent<SyncedObject>().ScaleInterpolationUpdate(_packet.InterpolateScales[i]);
            }
        }
    }

    #endregion
}
