using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedObject : MonoBehaviour {
    #region Variables

    [Tooltip("This is used to spawn this Synced Object on the Client")]
    [SerializeField] private int prefabId;

    private int syncedObjectUUID;

    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 previousScale;

    public int PrefabId { get => prefabId; set => prefabId = value; }
    public int SyncedObjectUUID { get => syncedObjectUUID; set => syncedObjectUUID = value; }

    #endregion

    #region Core

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
        if (SyncedObjectManager.instance.Vector2Mode) {
            Vector2SyncedObjectUpdate();
        } else {
            Vector3SyncedObjectUpdate();
        }
    }

    private void OnDestroy() {
        SyncedObjectManager.instance.DestroySyncedObject(this);
    }

    #endregion

    #region Send Synced Object Updates


    private void Vector2SyncedObjectUpdate() {
        Vector2 posDiff = new Vector3(Mathf.Abs(transform.position.x - previousPosition.x), Mathf.Abs(transform.position.y - previousPosition.y));
        float rotDiff = Mathf.Abs(transform.rotation.z - previousRotation.z);
        Vector2 scaleDiff = new Vector2(Mathf.Abs(transform.lossyScale.x - previousScale.x), Mathf.Abs(transform.lossyScale.y - previousScale.y));

        if (posDiff.x > SyncedObjectManager.instance.MinPosChange | posDiff.y > SyncedObjectManager.instance.MinPosChange) {
            SyncedObjectManager.instance.SyncedObjectVec2PosUpdate.Add(this);
            previousPosition = transform.position;
        } 
        
        if (rotDiff > SyncedObjectManager.instance.MinRotChange) {
            SyncedObjectManager.instance.SyncedObjectRotZUpdate.Add(this);
            previousRotation = transform.rotation;
        }

        if (scaleDiff.x > SyncedObjectManager.instance.MinRotChange | scaleDiff.y > SyncedObjectManager.instance.MinScaleChange) {
            SyncedObjectManager.instance.SyncedObjectVec2ScaleUpdate.Add(this);
            previousScale = transform.lossyScale;
        }
    }

    private void Vector3SyncedObjectUpdate() {
        Vector3 posDiff = new Vector3(Mathf.Abs(transform.position.x - previousPosition.x), Mathf.Abs(transform.position.y - previousPosition.y), Mathf.Abs(transform.position.z - previousPosition.z));
        Vector3 rotDiff = new Vector3(Mathf.Abs(transform.rotation.x - previousRotation.x), Mathf.Abs(transform.rotation.y - previousRotation.y), Mathf.Abs(transform.rotation.z - previousRotation.z));
        Vector3 scaleDiff = new Vector3(Mathf.Abs(transform.lossyScale.x - previousScale.x), Mathf.Abs(transform.lossyScale.y - previousScale.y), Mathf.Abs(transform.lossyScale.z - previousScale.z));

        if (posDiff.z > SyncedObjectManager.instance.MinPosChange) {
            SyncedObjectManager.instance.SyncedObjectVec3PosUpdate.Add(this);
            previousPosition = transform.position;
        } else if (posDiff.x > SyncedObjectManager.instance.MinPosChange | posDiff.y > SyncedObjectManager.instance.MinPosChange) {
            SyncedObjectManager.instance.SyncedObjectVec2PosUpdate.Add(this);
            previousPosition = transform.position;
        }

        if (rotDiff.x > SyncedObjectManager.instance.MinRotChange | rotDiff.y > SyncedObjectManager.instance.MinRotChange) {
            SyncedObjectManager.instance.SyncedObjectRotUpdate.Add(this);
            previousRotation = transform.rotation;
        } else if (rotDiff.z > SyncedObjectManager.instance.MinRotChange) {
            SyncedObjectManager.instance.SyncedObjectRotZUpdate.Add(this);
            previousRotation = transform.rotation;
        }

        if (scaleDiff.z > SyncedObjectManager.instance.MinScaleChange) {
            SyncedObjectManager.instance.SyncedObjectVec3ScaleUpdate.Add(this);
            previousScale = transform.lossyScale;
        } else if (scaleDiff.x > SyncedObjectManager.instance.MinRotChange | scaleDiff.y > SyncedObjectManager.instance.MinScaleChange) {
            SyncedObjectManager.instance.SyncedObjectVec2ScaleUpdate.Add(this);
            previousScale = transform.lossyScale;
        }
    }

    #endregion
}
