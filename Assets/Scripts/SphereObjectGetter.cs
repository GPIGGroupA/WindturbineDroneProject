using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereObjectGetter : MonoBehaviour
{
    public int range;
    string[] test =  {
        "sphere",
        "rect",
        "cylinder"
    };

    // Update is called once per frame
    void Update()
    {
        //so its not done every time
        if (Input.GetKeyUp(KeyCode.K)) {
            Collider[] test = Physics.OverlapSphere(transform.position, range);
            Debug.Log("This time got:");
            foreach(Collider current in test){
                Debug.Log(current.gameObject.name);
            }
            Debug.Log("--------");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, range);
    }
}
