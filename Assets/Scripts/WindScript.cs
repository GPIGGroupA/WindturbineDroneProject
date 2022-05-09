using UnityEngine;

public class WindScript : MonoBehaviour
{
    //height and width of the perlin texture (resolution)
    public int width = 256;
    public int height = 256;

    public float scale = 5;
    public float maxAngle = 100;

    private float offsetX;
    private float offsetY;

    public float speed = 0.02f;
    public Vector2 unitDirection;

    public float maxWind = 30;

    void Start() {
        //randomly generate the starting offset and movement direction
        offsetX = Random.Range(0f, 1000000f);
        offsetY = Random.Range(0f, 1000000f);

        //set a random direction for the wind
        unitDirection.x = Random.Range(0f, 1f);
        unitDirection.y = Random.Range(0f, 1f);

        if (Random.Range(0f, 1f) > 0.5) {
            unitDirection.x = -unitDirection.x;
        }
        if (Random.Range(0f, 1f) > 0.5) {
            unitDirection.y = -unitDirection.y;
        }

        unitDirection.Normalize();

        // for (int i = -10; i < 10; i++) {
        //     Debug.Log("sigmoid(" + i + "): " + Sigmoid(i));
        //     Debug.Log("                  : " + (Sigmoid(i) - 0.5f));
        //     Debug.Log("                  : " + ((Sigmoid(i) - 0.5f) * 2f));
        //     Debug.Log("                  : " + ((Sigmoid(i) - 0.5f) * 2f * 120f));
        //     Vector2 test = Quaternion.Euler(0, 0, ((Sigmoid(i) - 0.5f) * 2f * 120f)) * unitDirection;
        //     Debug.Log("                  : " + Vector2.Angle(test, unitDirection));
        // }
    }

    void Update() {
        unitDirection.Normalize();

        offsetX += speed * unitDirection.x;
        offsetY += speed * unitDirection.y;
        
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    //function only used if we want to generate the noise texture
    Texture2D GenerateTexture() {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, CalculateColor(x, y));
            }
        }

        texture.Apply();

        return texture;
    }

    /*
    Function to return a colour based on the value of perlin noise at a given point in a texture
    Used for generating the perlin noise texture
    */
    Color CalculateColor(int x, int y) {

        float xCoord = (float) x / width * scale + offsetX;
        float yCoord = (float) y / height * scale + offsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }

    public Vector2 getWindDir(float x, float y) {
        /*
        New Plan:
        1. convert drone x/y into float
        2. get perlin noise value
        3. convert perlin noise value into wind magnitude
        4. subtract 0.5 and pass through sigmoid function
        5. use new value to calculate an angle from the standard noise
        */

        float droneX = (float) x / width * scale + offsetX;
        float droneY = (float) y / width * scale + offsetY;

        float perlinValue = Mathf.PerlinNoise(droneX, droneY);

        float perlinLessHalf = perlinValue - 0.5f;

        //calculate the magnitude of wind at this point
        float windMagnitude = perlinValue * maxWind;

        float angleFromStandard = ((Sigmoid(perlinLessHalf * 12) - 0.5f) * 2f) * maxAngle;

        Vector2 rotated = Quaternion.Euler(0, 0, angleFromStandard) * unitDirection;
        
        return rotated * windMagnitude;
    }

    public float Sigmoid(float value) {
        float k = Mathf.Exp(value);
        return k / (1.0f + k);
    }
}