using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public ArrayList allWindTurbines;
    public ArrayList allHubTurbines;
    public ArrayList allDrones;
    public ArrayList allBoats;

    private Light sun;

    //Set in Editor
    public Material darkSkybox;
    public Material lightSkybox;

    public Cubemap darkCubemap;
    public Cubemap lightCubemap;

    private GameObject rainStorm;
    private GameObject wind;

    public GameObject nuke;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        allWindTurbines = new ArrayList();
        allHubTurbines = new ArrayList();
        allDrones = new ArrayList();
        allBoats = new ArrayList();

        sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<Light>();

        rainStorm = GameObject.FindGameObjectWithTag("RainStorm");
        rainStorm.SetActive(false);
        wind = GameObject.FindGameObjectWithTag("Wind");
        wind.SetActive(false);

        UpdateAllEntityReferences();
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSunColour();
        EasyToggleRainStorm();
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

    public void ToggleStorm()
    {
        if (rainStorm.activeSelf)
        {
            rainStorm.SetActive(false);
            wind.SetActive(false);
            RenderSettings.skybox = lightSkybox;
            RenderSettings.customReflection = lightCubemap;
        }
        else 
        {
            rainStorm.SetActive(true);
            wind.SetActive(true);
            RenderSettings.skybox = darkSkybox;
            RenderSettings.customReflection = darkCubemap;
        }
    }

    private void ChangeSunColour() 
    {
        if (rainStorm.activeSelf && sun.intensity > 0)
        {
            sun.intensity -= 0.5f * Time.deltaTime;
        }
        else if (!rainStorm.activeSelf && sun.intensity < 1)
        {
            sun.intensity += Mathf.Clamp01(0.5f * Time.deltaTime);
        }
    }

    public void DestroyWindturbine()
    {
        GameObject randTurbine = (GameObject)allWindTurbines[Random.Range(0, allWindTurbines.Count)];
        Transform t = randTurbine.GetComponent<Transform>();

        GameObject n = Instantiate(nuke, t.position, Quaternion.identity);
        Destroy(randTurbine);

        UpdateAllEntityReferences();
    }

    //Emilien stuff to make filming easier
    public void EasyToggleRainStorm()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (rainStorm.activeSelf)
            {
                rainStorm.SetActive(false);
                wind.SetActive(false);
                RenderSettings.skybox = lightSkybox;
                RenderSettings.customReflection = lightCubemap;
            }
            else 
            {
                rainStorm.SetActive(true);
                wind.SetActive(true);
                RenderSettings.skybox = darkSkybox;
                RenderSettings.customReflection = darkCubemap;
            }
        }
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
