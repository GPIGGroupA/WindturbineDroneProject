using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{

    public static List<GameObject> getObjectsInRange(GameObject parent, float range, string filter="None") {
        Collider[] sphere = Physics.OverlapSphere(parent.transform.position, range);
        List<GameObject> objectsInRange = new List<GameObject>();

        foreach (Collider current in sphere) {
            if (current.gameObject != parent) {
                //if there is a filter, just get the ones that match the filter
                if (filter != "None") {
                    if (current.tag == filter) {
                        objectsInRange.Add(current.gameObject);
                    }
                } else {
                    //if there is no filter then just get all the standard objects
                    if (current.tag == "Drone" || current.tag == "WindTurbine" || current.tag == "HubTurbine") {
                        objectsInRange.Add(current.gameObject);
                    }
                }
            }
        }

        return objectsInRange;
    }
}
