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

    // To remove
    Transform target;


    public void Start()
    {
        position = transform.position;

        // To remove
        GameObject[] targets = GameObject.FindGameObjectsWithTag("HubTurbine");
        
        foreach (GameObject t in targets)
        {
            if (t.GetComponent<HubTurbine>()) 
            {
                target = t.transform;
            }
        }
    }

    public void Update() 
    {
        Move();
    }

    void Move() 
    {
        position = transform.position;

        float singleStep = steer_strength * Time.deltaTime;

        Vector3 targetDirection = target.position - transform.position;
        Vector3 desiredDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        Vector3 desiredVelocity = desiredDirection * max_speed;
        Vector3 desiredSteeringForce = (desiredVelocity - velocity) * steer_strength;
        Vector3 acceleration = Vector3.ClampMagnitude(desiredSteeringForce, steer_strength);

        velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.deltaTime, max_speed);
        position += velocity * Time.deltaTime;
        
        transform.rotation = Quaternion.LookRotation(desiredDirection);
        transform.position = new Vector3(position.x, Mathf.Clamp(position.y, 0, 1000), position.z);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "HubTurbine"){
            if (collider.GetComponent<HubTurbine>().HoldDrone(this, 2)){
                Object.Destroy(this.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
    }
}
