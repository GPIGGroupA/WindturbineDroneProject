using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float camSpeed = 3000f;
    public float rotationSpeed = 1000f;

    public float followSpeed = 8f;
    public float borderWidth = 10f;

    Vector3 droneOffset = new Vector3(36, 50, -36);
    public GameObject followTarget;

    public Vector3 previousPosition;

    public bool enableMouseMovement = false;

    public bool followingObject = false;

    //Making camera movements with both mouse position and arrow keys.
    void Update()
    {
        if (!followingObject) 
        {
            Move();
            Rotate();
            Zoom();
        }
        else
        {
            Follow();
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            selectObject();
        }
    }

    public void selectObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10000))
        {
            GameObject hitObject = (GameObject)hit.transform.gameObject;
            if (hitObject.tag == "Drone")
            {
                this.previousPosition = this.transform.position;

                this.followTarget = hitObject;
                this.followingObject = true;
            }
            else if (hitObject.tag == "Sea")
            {
                this.followTarget = null;
                this.followingObject = false;

                this.transform.position = this.previousPosition;
            }
        } 
        
    }

    void Move()
    {
        Vector3 direction = new Vector3();

        //Up, Down, Right and Left movement (translating the camera coordinates, so rotation doesn't affect movement)
        if (Input.GetKey(KeyCode.UpArrow) || (Input.mousePosition.y >= Screen.height - borderWidth && enableMouseMovement)) 
        {
            direction += (Vector3.forward + Vector3.up) / 2;
        }
        if (Input.GetKey(KeyCode.DownArrow) || (Input.mousePosition.y <= borderWidth && enableMouseMovement)) 
        {
            direction += (Vector3.down + Vector3.back) / 2;
        }
        if (Input.GetKey(KeyCode.RightArrow) || (Input.mousePosition.x >= Screen.width - borderWidth && enableMouseMovement)) 
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || (Input.mousePosition.x <= borderWidth && enableMouseMovement)) 
        {
            direction += Vector3.left;
        }

        transform.Translate(direction * camSpeed * Time.deltaTime);
    }

    void Zoom()
    {
        Vector3 direction = new Vector3();

        //Zooming in and out
        if (Input.GetKey(KeyCode.UpArrow) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            direction = Vector3.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            direction = Vector3.back;
        }

        transform.Translate(direction * camSpeed * Time.deltaTime);
    }

    void Rotate()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.LeftArrow))
        {
            float rotation = (rotationSpeed / 2.5f) * Time.deltaTime;
            transform.Rotate(Vector3.up, -rotation, Space.World);
        }
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.RightArrow))
        {
            float rotation = (rotationSpeed / 2.5f) * Time.deltaTime;
            transform.Rotate(Vector3.up, rotation, Space.World);
        }
    }

    public void Follow()
    {
        transform.position = Vector3.Lerp(transform.position, followTarget.transform.position + droneOffset, Time.deltaTime * followSpeed);
    }
}
