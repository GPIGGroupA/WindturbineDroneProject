using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windTest : MonoBehaviour
{
    public WindScript windScript;

    public List<Vector2> recentWindDirections = new List<Vector2>();
    private int index = 0;
    private bool firstSet = true;

    // Update is called once per frame
    void Update()
    {
        if (firstSet) {
            //if its still the first loop add another index
            recentWindDirections.Add(windScript.getWindDir(transform.position.x, transform.position.z));
        } else {
            //otherwise if it already has the 10 index's set up
            recentWindDirections[index] = windScript.getWindDir(transform.position.x, transform.position.z);
        }

        if (firstSet && index == 9) {
            firstSet = false;
        }

        //loop the index from 0-9
        index = index < 9 ? index + 1 : 0;
    }

    Vector2 averageWind()
    {
        Vector2 average = new Vector2();

        foreach(Vector2 windDir in recentWindDirections) {
            average += windDir;
        }

        average /= recentWindDirections.Count;

        return average;
    }

    // Uncomment to show the forward, general wind and drone actual wind lines
    // void OnDrawGizmos() {
    //     Vector2 avgWind = this.averageWind();
        
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(transform.position, new Vector3(avgWind.x, 0, avgWind.y));
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawRay(transform.position, new Vector3(windScript.unitDirection.x, 0, windScript.unitDirection.y) * 10);
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawRay(transform.position, transform.forward * 10);
    // }
}
