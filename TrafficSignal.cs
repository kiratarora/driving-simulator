using UnityEngine;

public class TrafficSignal : MonoBehaviour
{
    public float detectionRadius = 8f;
    public GameObject redLight;
    public GameObject greenLight;

    private bool isRed = false;
    private bool isGreen = true;
    private float timeSinceDetection = 0f;
    private bool carDetected = false;
    private int count = 0;
    private CarController car;
    private bool broke_signal = false;
    // private int num_lives;

    void Start()
    {
        GameObject car_obj = GameObject.FindGameObjectWithTag("Player");
        car = car_obj.GetComponent<CarController>();
        // num_lives = car.num_lives;
    }
    void Update()
    {
        // Check for nearby cars
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        // Check if any of the detected colliders are cars
        carDetected = false;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                carDetected = true;
                break;
            }
        }

        // Change traffic signal properties based on car presence
        if (carDetected)
        {
            //Debug.Log("Car detected!");
            count += 1;
            if (count == 1)
            {
                SwitchLights(); // Start traffic control
                broke_signal = false;
            }
            timeSinceDetection += Time.deltaTime;
            Debug.Log(timeSinceDetection);
            if (timeSinceDetection >= 30f) //
            {
                // Change to green after 1 minute
                Debug.Log("Switched");
                SwitchLights();
                timeSinceDetection = 0.0f; //Reset every 1/2 min. - keep looping lights
            }

            if(isRed && timeSinceDetection >= 2f && car.currentVelocity > 0 && !broke_signal)
            {
                //If car hasn't braked in 3 seconds
                Debug.Log("Breaking signal");
                broke_signal = true;
                car.num_lives -= 1; //Lose life for breaking signal
            }
        }
        else
        {
            greenLight.SetActive(true);
            isGreen = true;
        }
    }
    void SwitchLights()
    {
        if(isRed)
        {
            Debug.Log("Red to Green");
            redLight.SetActive(false);
            greenLight.SetActive(true);
            isGreen = true;
            isRed = false;
            Debug.Log(timeSinceDetection);
            Debug.Log(car.currentVelocity);
            if (timeSinceDetection >= 3f && car.currentVelocity > 0)
            {
                //If car hasn't braked in 3 seconds
                car.num_lives--; //Lose life for breaking signal
                Debug.Log("Breaking signal");
            }
            return;
        }
       if(isGreen)
       {
            Debug.Log("Green to Red");
            redLight.SetActive(true);
            greenLight.SetActive(false);
            isGreen = false;
            isRed = true;
            return;
       }
    }
}
