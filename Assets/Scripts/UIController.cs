
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    public GameController gameController;
    public GameObject panel;
    public TMP_Text gameStats;

    private GameObject selectedObject = null;

    public CameraMovement mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        panel = GameObject.Find("DeveloperPanel");
        gameStats = GameObject.Find("GameStats").GetComponent<TMP_Text>();
        mainCamera = Camera.main.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleDeveloperMode()
    {
       bool state = panel.activeSelf ? false : true;

       panel.SetActive(state);
    }

    public void UpdateStats()
    {
        int turbines = gameController.allWindTurbines.Count;
        int hubs = gameController.allHubTurbines.Count;
        int drones = gameController.allDrones.Count;
        int boats = gameController.allBoats.Count;

        gameStats.text = String.Format("Turbines: {0} \nHubs: {1}, \nDrones: {2} \nBoats: {3}", turbines, hubs, drones, boats);
    }

    public void AssignRandomTask()
    {
        GameObject drone = (GameObject)gameController.allDrones[0];
        GameObject turbine = (GameObject)gameController.allWindTurbines[0];

        Vector3 initialPosition = drone.transform.position;

        drone.GetComponent<Drone>().action_stack.Add(new Action( ActionType.TakeOff));
        drone.GetComponent<Drone>().action_stack.Add(new Action( ActionType.GoTo, turbine.transform.position));
        drone.GetComponent<Drone>().action_stack.Add(new Action( ActionType.Maintain, turbine.transform.position));
        drone.GetComponent<Drone>().action_stack.Add(new Action( ActionType.TakeOff));
        
        drone.GetComponent<Drone>().action_stack.Add(new Action( ActionType.GoTo, initialPosition));

    }

    public void FollowTarget(bool follow)
    {
        if (selectedObject == null)
        {
            mainCamera.previousPosition = mainCamera.transform.position;
        }
        else 
        {
            selectedObject.GetComponent<Outline>().enabled = false;
        }

        if (follow)
        {
            
            //Makes Camera Follow a randomly chosen target drone
            int random = UnityEngine.Random.Range(0, gameController.allDrones.Count);
            selectedObject = (GameObject)gameController.allDrones[random];
            
            mainCamera.followTarget = selectedObject;
            mainCamera.followingObject = true;

            //Outlines the current drone
            selectedObject.GetComponent<Outline>().enabled = true;
        }
        else
        {
            mainCamera.followingObject = false;
            mainCamera.transform.position = mainCamera.previousPosition;

            selectedObject.GetComponent<Outline>().enabled = false;
            selectedObject = null;
        }
    }
}
