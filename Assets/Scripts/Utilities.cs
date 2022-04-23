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

    public static float shortestDistanceToLine(Vector3 l1, Vector3 l2, Vector3 p){
        float l1l2 = (l1-l2).magnitude;
        float l1p = (l1-p).magnitude;
        float l2p = (l2-p).magnitude;


        if (l1p > l1l2){return l1p;}
        else if (l2p > l1l2){return l2p;}
        else {
            float s = (l1l2+l1p+l2p)/2;
            float area = Mathf.Sqrt(s*(s-l1l2)*(s-l1p)*(s-l2p));
            return area*2/l1l2;
        }
    }

    public static float jobRank(Job job){
        // !00 for 100 seconds, e.g. Don't even think about it until 100 seconds
        return Mathf.Pow(100 - Mathf.Clamp(Mathf.Abs(job.deadline - Time.time), 0, 100), 2)*job.priority;
    }
}
