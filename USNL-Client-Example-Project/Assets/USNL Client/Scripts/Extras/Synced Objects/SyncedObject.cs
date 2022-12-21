using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USNL {
    public class SyncedObject : MonoBehaviour {
        [SerializeField] private bool interpolate = true;

        public int SyncedObjectUuid;

        private Vector3 previousUpdatedPosition = new Vector3(-999999, -999999, -999999);
        private Vector3 positionRateOfChange = Vector3.zero; // Per Second
        private float positionUpdateReceivedTime = 0;

        private Vector3 rotationRateOfChange = Vector3.zero; // Per Second

        private Vector3 previousUpdatedScale = new Vector3(-999999, -999999, -999999);
        private Vector3 scaleRateOfChange = Vector3.zero; // Per Second
        private float scaleUpdateReceivedTime = 0;

        private void Update() {
            if (interpolate) {
                transform.position += positionRateOfChange * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.eulerAngles + (rotationRateOfChange * Time.deltaTime)), 99f);
                transform.localScale += scaleRateOfChange * Time.deltaTime;
            }
        }

        #region Local Interpolation

        public void PositionUpdate(Vector3 _updatedPosition) {
            if (SyncedObjectManager.instance.LocalInterpolation) {
                if (previousUpdatedPosition != new Vector3(-999999, -999999, -999999)) {
                    float timeBetweenUpdates = Time.realtimeSinceStartup - positionUpdateReceivedTime;

                    if (timeBetweenUpdates == 0) {
                        positionRateOfChange = Vector3.zero;
                    } else {
                        positionRateOfChange = (_updatedPosition - previousUpdatedPosition) / timeBetweenUpdates;
                    }
                }
                previousUpdatedPosition = _updatedPosition;
                positionUpdateReceivedTime = Time.realtimeSinceStartup;
            }
        }

        public void RotationUpdate(Vector3 _updateRotation) {
            // Add local rotation interpolation here if you want to
            /*if (SyncedObjectManager.instance.LocalInterpolation) {

            }*/
        }

        public void ScaleUpdate(Vector3 _updateScale) {
            if (SyncedObjectManager.instance.LocalInterpolation) {
                if (previousUpdatedScale != new Vector3(-999999, -999999, -999999)) {
                    float timeBetweenUpdates = Time.realtimeSinceStartup - scaleUpdateReceivedTime;

                    if (timeBetweenUpdates == 0) {
                        scaleRateOfChange = Vector3.zero;
                    } else {
                        scaleRateOfChange = (_updateScale - previousUpdatedScale) / timeBetweenUpdates;
                    }
                }

                previousUpdatedScale = _updateScale;
                scaleUpdateReceivedTime = Time.realtimeSinceStartup;
            }
        }

        #endregion

        #region Server Interpolation

        public void PositionInterpolationUpdate(Vector3 _interpolatePosition) {
            if (!SyncedObjectManager.instance.LocalInterpolation) {
                positionRateOfChange = _interpolatePosition;
            }
        }

        public void RotationInterpolationUpdate(Vector3 _interpolateRotation) {
            if (!SyncedObjectManager.instance.LocalInterpolation) {
                rotationRateOfChange = _interpolateRotation;
            }
        }

        public void ScaleInterpolationUpdate(Vector3 _interpolateScale) {
            if (!SyncedObjectManager.instance.LocalInterpolation) {
                scaleRateOfChange = _interpolateScale;
            }
        }

        #endregion
    }
}