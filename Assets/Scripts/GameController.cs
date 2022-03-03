using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public ArrayList allWindTurbines;
    public ArrayList allHubTurbines;
    public ArrayList allDrones;
    public ArrayList allBoats;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        allWindTurbines = new ArrayList();
        allHubTurbines = new ArrayList();
        allDrones = new ArrayList();
        allBoats = new ArrayList();

        UpdateAllEntityReferences();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateAllEntityReferences()
    {
        allWindTurbines.Clear();
        allHubTurbines.Clear();
        allDrones.Clear();
        allBoats.Clear();

        allWindTurbines.AddRange(GameObject.FindGameObjectsWithTag("WindTurbine"));
        allHubTurbines.AddRange(GameObject.FindGameObjectsWithTag("HubTurbine"));
        allDrones.AddRange(GameObject.FindGameObjectsWithTag("Drone"));
        allBoats.AddRange(GameObject.FindGameObjectsWithTag("Boat"));
    }
}
