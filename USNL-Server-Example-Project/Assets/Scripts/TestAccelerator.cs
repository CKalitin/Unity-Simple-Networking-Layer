using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAccelerator : MonoBehaviour {
    [SerializeField] private Vector2 force;
    [SerializeField] private float forceMultiplier = 1f;
    
    List<Rigidbody2D> syncedObjectsInCollider = new List<Rigidbody2D>();

    private void FixedUpdate() {
        for (int i = 0; i< syncedObjectsInCollider.Count; i++) {
            syncedObjectsInCollider[i].AddForce(force * forceMultiplier);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<SyncedObject>()) {
            syncedObjectsInCollider.Add(collision.GetComponent<Rigidbody2D>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (syncedObjectsInCollider.Contains(collision.GetComponent<Rigidbody2D>())) {
            syncedObjectsInCollider.Remove(collision.GetComponent<Rigidbody2D>());
        }
    }
}
