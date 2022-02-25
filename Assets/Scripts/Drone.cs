using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State {Parked, Comission, TakeOff, Move, Land, Decomission, Failure}


public class Drone : MonoBehaviour {

    // Drone Infomation
    public float battery_percentage= 100F;
    public State current_state = State.Parked;
    public Vector3 target;
    public bool has_target= false;
    private List<Job> jobs_queue= new List<Job>();
    public float battery_level_tolerance= 10F;


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
    }

    void UnityRotateAnimation(Vector3 targetDirection){
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
    }

    public void Update() 
    {
        if (current_state==State.Parked){return;}

        StateCheck();
        StateAct();

        if (current_state==State.Comission || current_state==State.Decomission){return;}

        battery_percentage-= 0.001F;
        velocity = Vector3.Scale(velocity, new Vector3(0.99F, 0.99F, 0.99F));
    }

    void StateCheck()
    {
        // Specfic state things
        switch (current_state){
            case State.Comission:
                if (jobs_queue.Count!=0){current_state= State.TakeOff;}
                break;

            case State.TakeOff:
                if (transform.position.y>400f){
                    current_state= State.Move;
                    Job? job= whichJobToDo(transform.position);
                    if (job!=null){}// TODO: Get target from the job
                }
                break;
        }

        // Allowed in all states
        if (estimatedBatteryCostForSafeReturn() + battery_level_tolerance >= battery_percentage){
            // TODO: Select nearest charging turbine and do landing
            current_state= State.Land; // TODO: Remove
        }
        if (transform.position.y<=0f){
            current_state= State.Failure;
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
        UnityMove(Vector3.down, 1F);
        UnityRotateAnimation(Vector3.down);
    }

    void TakeOff()
    {
        UnityMove(Vector3.up, 1F);
        UnityRotateAnimation(Vector3.up);
    }

    void Comission()
    {

    }

    void Move()
    {
        if (has_target){
            Vector3 targetDirection = target - transform.position;
            UnityMove(targetDirection, 1F);
            UnityRotateAnimation(targetDirection);
        }
        else {
            UnityMove(Vector3.zero, 1F);
            UnityRotateAnimation(Vector3.up);
        }
    }

    void Decomission()
    {

    }


    // Utilitys
    public Job? whichJobToDo(Vector3 startpoi){
        if (jobs_queue.Count!=0){
            return jobs_queue[0];
        }
        return null;
    }

    // Heuristics
    public float estimatedBatteryCostForJob(Job job, Vector3 startpoi){
        return 0.0F;
    }

    public float estimatedBatteryCostForSafeReturn(){
        return 10f;
    }

}
