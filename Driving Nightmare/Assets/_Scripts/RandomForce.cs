using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomForce : MonoBehaviour
{
    private float _lastImpact;
    private void FixedUpdate() {
        if (Time.time - _lastImpact > 1f) {
            GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 0.5f, ForceMode.Impulse);
            _lastImpact = Time.time;
        }
    }
}
