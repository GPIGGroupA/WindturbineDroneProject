using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public GameObject[] allWindTurbines;
    public GameObject[] allHubTurbines;
    public GameObject[] allDrones;
    public GameObject[] allBoats;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        UpdateAllEntityReferences();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateAllEntityReferences()
    {
        allWindTurbines = GameObject.FindGameObjectsWithTag("WindTurbine");
        allHubTurbines = GameObject.FindGameObjectsWithTag("HubTurbine");
        allDrones = GameObject.FindGameObjectsWithTag("Drone");
        allBoats = GameObject.FindGameObjectsWithTag("Boat");
    }
}
