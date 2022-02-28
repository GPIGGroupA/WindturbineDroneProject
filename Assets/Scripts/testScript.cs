using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> test = Utilities.getObjectsInRange(200, transform.position);

        foreach(GameObject current in test) {
            Debug.Log(current.name);
        }
    }
}
