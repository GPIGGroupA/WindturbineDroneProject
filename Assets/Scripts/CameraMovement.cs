using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float camSpeed = 3000f;
    public float rotationSpeed = 1000f;
    public float borderWidth = 10f;

    public bool enableMouseMovement = false;

    //Making camera movements with both mouse position and arrow keys.
    void Update()
    {
        Move();
        Rotate();
        Zoom();
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
}
