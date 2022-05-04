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

    public Vector3? locationOfTurbineWithID(string id){
        foreach (GameObject t in allWindTurbines){
            WindTurbine turbine= t.GetComponent<WindTurbine>();
            if (turbine.id == id) {return turbine.transform.position;}
        }
        foreach (GameObject t in allHubTurbines){
            HubTurbine turbine= t.GetComponent<HubTurbine>();
            if (turbine.id == id) {return turbine.transform.position;}
        }
        return null;
    }

    public string closestHubTurbine(Vector3 poi){
        float[] dists = new float[allHubTurbines.Count];
        for (int i=0; i<allHubTurbines.Count; i++){
            dists[i] = Util.distanceToPoint(poi, ((GameObject) allHubTurbines[i]).GetComponent<HubTurbine>().transform.position);
        }
        (float m, int am)= Util.min(dists);
        return ((GameObject) allHubTurbines[am]).GetComponent<HubTurbine>().id;
    }
}
