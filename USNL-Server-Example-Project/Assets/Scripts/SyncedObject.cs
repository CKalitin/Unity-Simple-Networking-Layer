using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedObject : MonoBehaviour {
    [Tooltip("This is used to spawn this Synced Object on the Client")]
    [SerializeField] private int prefabId;

    private int syncedObjectUUID;

    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 previousScale;

    public int PrefabId { get => prefabId; set => prefabId = value; }
    public int SyncedObjectUUID { get => syncedObjectUUID; set => syncedObjectUUID = value; }

    void Awake() {
        syncedObjectUUID = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0); // Generate UUID

        // Initialize previous values
        previousPosition = transform.position;
        previousRotation = transform.rotation;
        previousScale = transform.lossyScale;
    }

    private void Start() {
        SyncedObjectManager.instance.InstantiateSyncedObject(this);
    }

    void Update() {
        if (transform.position != previousPosition) { SendSyncedObjectUpdate(); previousPosition = transform.position; return; }
        if (transform.rotation != previousRotation) { SendSyncedObjectUpdate(); previousRotation = transform.rotation; return; }
        if (transform.lossyScale != previousScale) { SendSyncedObjectUpdate(); previousScale = transform.lossyScale; return; }
    }

    private void SendSyncedObjectUpdate() {
        PacketSend.SyncedObjectUpdate(syncedObjectUUID, transform.position, transform.rotation, transform.lossyScale);
    }

    private void OnDestroy() {
        SyncedObjectManager.instance.DestroySyncedObject(this);
    }
}
