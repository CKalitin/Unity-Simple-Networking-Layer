using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AverageFPS : MonoBehaviour {
    public float averageFPS = 0;
    public int totalFrames = 0;

    private void Start() {
        averageFPS = 1 / Time.deltaTime;
    }

    void Update() {
        totalFrames++;
        float fpsDiff = (1 / Time.deltaTime) - averageFPS;

        averageFPS += (fpsDiff / totalFrames);
    }
}
