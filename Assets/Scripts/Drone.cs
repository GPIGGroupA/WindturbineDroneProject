using UnityEngine;

public class Drone : MonoBehaviour {

    // Drone Control Infomation
    public float battery_percentage= 100F;
    private enum State {CollectJobs, TakeOff, Move, DoJob, Land}
    private State current_state = State.TakeOff;


    // Unity Infomation
    private float max_speed = 200;
    private float steer_strength = 100;
    Vector3 position;
    Vector3 velocity;


    // Unity things
    void UnityMove(Vector3 target) 
    {
        float singleStep = steer_strength * Time.deltaTime;
        position = transform.position;

        Vector3 targetDirection = target - transform.position;
        Vector3 desiredDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        Vector3 desiredVelocity = desiredDirection * max_speed;
        Vector3 desiredSteeringForce = (desiredVelocity - velocity) * steer_strength;
        Vector3 acceleration = Vector3.ClampMagnitude(desiredSteeringForce, steer_strength);

        velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.deltaTime, max_speed);
        position += velocity * Time.deltaTime;

        desiredDirection.y= 0;
        transform.rotation = Quaternion.LookRotation(desiredDirection);
        transform.position = new Vector3(position.x, Mathf.Clamp(position.y, 0, 2000), position.z);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (current_state==State.Land && collider.gameObject.tag == "HubTurbine"){
            if (collider.GetComponent<HubTurbine>().HoldDrone(this, 2)){
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

        switch(current_state){
            case State.TakeOff:
                TakeOff();
                break;

            case State.Move:
                break;

            case State.Land:
                Land();
                break;
        }

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


    // State things
    void Land()
    {
        Vector3 target= position;
        target.y= 100F;  
        UnityMove(target);
    }

    void TakeOff()
    {
        Vector3 target= position;
        target.y= 1000F;  
        UnityMove(target);
    }
}
