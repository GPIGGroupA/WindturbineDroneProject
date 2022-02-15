using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbine : MonoBehaviour
{
    public string id;
    public int problem_level= 1;
    const float CHANCEOFLEVEL2 = 0.00001F;
    const float CHANCEOFLEVEL3 = 0.000001F;
    const float CHANCEOFLEVEL4 = 0.0000001F;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float chance= Random.Range(0F, 1F);
        if (chance < CHANCEOFLEVEL4){problem_level=4;}
        else if (chance < CHANCEOFLEVEL3){problem_level=3;}
        else if (chance < CHANCEOFLEVEL2){problem_level=2;}

    }
}
