using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State {Comission, TakeOff, Move, Land, Decomission, Idle}


public class Drone : MonoBehaviour {

    // Drone Infomation
    public float battery_percentage= 100F;
    private State current_state = State.Comission;
    Vector3 target;
    bool has_target= false;
    private List<Job> jobs_queue= new List<Job>();


    // Unity Infomation
    private float max_speed = 200;
    private float steer_strength = 100;
    Vector3 position;
    Vector3 velocity;


    // Unity things
    void UnityMove(Vector3 direction) // Takes a direction the drone wants to move, but this is the reality limitiations, e.g. momentum, rotation speed
    {
        float singleStep = steer_strength * Time.deltaTime;
        position = transform.position;

        Vector3 nextFrameDirection = Vector3.RotateTowards(transform.forward, direction, singleStep, 0.0f);
        Vector3 nextFrameVelocity = nextFrameDirection * max_speed;
        Vector3 nextFrameSteeringForce = (nextFrameVelocity - velocity) * steer_strength;
        Vector3 acceleration = Vector3.ClampMagnitude(nextFrameSteeringForce, steer_strength);

        velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.deltaTime, max_speed);
        position += velocity * Time.deltaTime;

        nextFrameDirection.y= 0F;
        if (nextFrameDirection != Vector3.zero){
            transform.rotation = Quaternion.LookRotation(nextFrameDirection);
        }
        transform.position = new Vector3(position.x, Mathf.Clamp(position.y, 0, 2000), position.z);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (current_state==State.Land && collider.gameObject.tag == "HubTurbine"){
            if (collider.GetComponent<HubTurbine>().pad.holdChargingDrone(this, 2)){
                Object.Destroy(this.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
    }


    // Control things
    public void Start()
    {
        position = transform.position;
    }

    public void Update() 
    {
        StateCheck();
        StateAct();

        battery_percentage-= 0.1F;
        velocity = Vector3.Scale(velocity, new Vector3(0.99F, 0.99F, 0.99F));
    }

    void StateCheck()
    {
        if (current_state== State.TakeOff && position.y >= 600) {
            current_state= State.Move;
        }
        if (battery_percentage<10F) {
            current_state= State.Land;
        }
    }

    void StateAct()
    {
        switch(current_state){
            case State.TakeOff:
                TakeOff();
                break;

            case State.Move:
                Move();
                break;

            case State.Land:
                Land();
                break;

            case State.Comission:
                Comission();
                break;

            case State.Decomission:
                Decomission();
                break;
        }
    }


    // State things
    void Land()
    {
        target= position;
        target.y= 100F;  
        Move();
    }

    void TakeOff()
    {
        target= position;
        target.y= 1000F;  
        Move();
    }

    void Comission()
    {

    }

    void Move()
    {
        Vector3 targetDirection = target - transform.position;
        UnityMove(targetDirection);
    }

    void Decomission()
    {

    }


    // Utilitys
    public int whichJobToDo(Vector3 startpoi){
        return -1;
    }


    // Heuristics
    public float estimatedBatteryCostForJob(Job job, Vector3 startpoi){
        return 0.0F;
    }
}
