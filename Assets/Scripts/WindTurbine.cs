using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbine : MonoBehaviour
{
    public string id;
    public float problem_level= 0;
    public int last_maintenance;
    private float chance_of_major_failures= 0.00001F;
    private float problem_level_growth= 1.002F;
    private float major_failure_starting_level= 0.0001F;
    
    // Start is called before the first frame update
    public void Start()
    {
        last_maintenance= Time.frameCount;
    }

    // Update is called once per frame
    public void Update()
    {
        if (problem_level < 1F) {
            if (Random.Range(0F, 1F) < chance_of_major_failures){
                problem_level+= major_failure_starting_level;
            }
            problem_level= problem_level*problem_level_growth;
            if (problem_level > 1){problem_level= 1;}
        }
    }

    void OnMouseDown(){ // TODO: here
        // this object was clicked - do something
        Debug.Log("hit");
    }
}
