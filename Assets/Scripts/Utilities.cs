using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static List<GameObject> getObjectsInRange(int range, Vector3 position) {
        Collider[] sphere = Physics.OverlapSphere(position, range);
        List<GameObject> objectsInRange = new List<GameObject>();

        foreach (Collider current in sphere) {
            if (current.tag == "Drone" || current.tag == "WindTurbine" || current.tag == "HubTurbine") {
                objectsInRange.Add(current.gameObject);
            }
        }

        return objectsInRange;
    }
}
