using UnityEngine;

public class Drone : MonoBehaviour {

    // Drone Control Infomation
    public float battery_percentage= 100F;
    private enum State {Parked, CollectJobs, TakeOff, Move, DoJob, Land}
    private State current_state = State.Parked;


    // Unity Infomation
    private float max_speed = 200;
    private float steer_strength = 100;
    Vector3 position;
    Vector3 velocity;


    void Start()
    {
        position = transform.position;
        // targets = GameObject.FindGameObjectsWithTag("WindTurbine");
        
        // foreach (GameObject t in targets)
        // {
        //     if (t.GetComponent<WindTurbine>()) 
        //     {
        //         target = t.transform;
        //     }
        // }
    }

    void Update() 
    {
        // SetState();
    }

    void SetState()
    {
        // switch (state) 
        // {
        //     case "Moving":
        //         Move();
        //         break;

        //     case "Idle":
        //         Idle();
        //         break;
        // }
    }

    void Move() 
    {
        // position = transform.position;

        // float singleStep = steerStrength * Time.deltaTime;

        // Vector3 targetDirection = target.position - transform.position;
        // Vector3 desiredDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Vector3 desiredVelocity = desiredDirection * maxSpeed;
        // Vector3 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        // Vector3 acceleration = Vector3.ClampMagnitude(desiredSteeringForce, steerStrength);

        // velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        // position += velocity * Time.deltaTime;
        
        // transform.rotation = Quaternion.LookRotation(desiredDirection);
        // transform.position = new Vector3(position.x, Mathf.Clamp(position.y, minHeight, maxHeight), position.z);
    }

    void Idle()
    {
        //Move up and down the wind turbine
        /*
        timer += Time.deltaTime;
        float oscillator = Mathf.Cos(timer * 2 / Mathf.PI) * 50;
        float newPosY = transform.position.y + Time.deltaTime * oscillator;

        Debug.Log("oscillator: " + oscillator);

        transform.position  = new Vector3(transform.position.x, newPosY, transform.position.z);
        */

        // transform.RotateAround(target.transform.position, Vector3.up, idleSpeed * Time.deltaTime);
        
    }

    void OnTriggerEnter(Collider collider)
    {
        // if (collider.gameObject.tag == "WindTurbine")
        // {
        //     this.state = "Idle";
        // }
        // else if (collider.gameObject.tag == "Drone")
        // {

        // }
    }

    void OnTriggerExit(Collider collider)
    {
        // if (collider.gameObject.tag == "WindTurbine")
        // {
        //     this.state = "Moving";
        // }
        
    }
}