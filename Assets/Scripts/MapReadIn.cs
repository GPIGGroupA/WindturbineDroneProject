using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapReadIn : MonoBehaviour
{
    public GameObject[] turbinePrefab;

    // Start is called before the first frame update
    void Start()
    {

        string[] turbinecoords = System.IO.File.ReadAllLines("Assets/turbinecoords.txt");

        foreach (string turbinecoord in turbinecoords)
        {
            string[] tmp= turbinecoord.Trim().Split(',');
            
            if (tmp[0]=="t"){
                GameObject tmp_turbine= Instantiate(turbinePrefab[1], new Vector3(float.Parse(tmp[2]), 100, float.Parse(tmp[3])), Quaternion.identity) as GameObject;
                WindTurbine turbine= tmp_turbine.GetComponent<WindTurbine>();
                turbine.id= tmp[1].Trim();
            } 
            else if (tmp[0]=="c"){
                GameObject tmp_turbine= Instantiate(turbinePrefab[0], new Vector3(float.Parse(tmp[2]), 100, float.Parse(tmp[3])), Quaternion.identity) as GameObject;
                HubTurbine turbine= tmp_turbine.GetComponent<HubTurbine>();
                turbine.id= tmp[1].Trim();
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
