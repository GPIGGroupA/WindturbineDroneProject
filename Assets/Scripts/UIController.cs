
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    public GameController gameController;
    public GameObject panel;
    public TMP_Text gameStats;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        panel = GameObject.Find("DeveloperPanel");
        gameStats = GameObject.Find("GameStats").GetComponent<TMP_Text>();
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
}
