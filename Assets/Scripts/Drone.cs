using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State {Parked, Comission, TakeOff, Move, Land, Decomission, Idle}


public class Drone : MonoBehaviour {

    // Drone Infomation
    public float battery_percentage= 100F;
    public State current_state = State.Parked;
    Vector3 target;
    bool has_target= false;
    private List<Job> jobs_queue= new List<Job>();


    // Unity Infomation
    private float max_speed = 200;
    private float turnspeed = 0.5F;
    Vector3 position;
    Vector3 velocity;


    // Unity things
    void UnityMove(Vector3 targetDirection, float deltaMagnitude) // Takes a direction the drone wants to move, but this is the reality limitiations, e.g. momentum, rotation speed
    {
        position = transform.position;

        Vector3 nextFrameVelocity = Vector3.ClampMagnitude(velocity + Vector3.Normalize(targetDirection)*deltaMagnitude, max_speed);

        velocity = nextFrameVelocity;
        position += nextFrameVelocity * Time.deltaTime;

        transform.position = position;


        Vector3 targetDroneRotation;
        if (Vector3.Angle(targetDirection, Vector3.down)<=90f){
            Vector3 normal = Vector3.Normalize(targetDirection);
            normal.y = 0;

            targetDroneRotation = Vector3.Reflect(targetDirection*-1.0f, normal);
        } else {
            targetDroneRotation = targetDirection;
        }

        targetDroneRotation = Vector3.Normalize(targetDroneRotation);

        Vector3 nextFrameDirection = Vector3.RotateTowards(transform.up, targetDroneRotation, turnspeed*Time.deltaTime, 0.0f);
        nextFrameDirection.x = Mathf.Clamp(nextFrameDirection.x, Vector3.up.x-0.5F, Vector3.up.x+0.5F);
        nextFrameDirection.y = Mathf.Clamp(nextFrameDirection.y, Vector3.up.y-0.5F, Vector3.up.y+0.5F);
        nextFrameDirection.z = Mathf.Clamp(nextFrameDirection.z, Vector3.up.z-0.5F, Vector3.up.z+0.5F);

        transform.up = nextFrameDirection;
    }

    void OnTriggerEnter(Collider collider)
    {
        // if (current_state==State.Land && collider.gameObject.tag == "HubTurbine"){
        //     if (collider.GetComponent<HubTurbine>().pad.holdChargingDrone(this, 2)){
        //         Object.Destroy(this.gameObject);
        //     }
        // }
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
        if (current_state==State.Parked){return;}

        StateCheck();
        StateAct();

        if (current_state==State.Comission || current_state==State.Decomission){return;}

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
        target.y= 400F;
        target.x= -800F;  
        Move();
    }

    void Comission()
    {

    }

    void Move()
    {
        Vector3 targetDirection = target - transform.position;
        UnityMove(targetDirection, 1F);
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
