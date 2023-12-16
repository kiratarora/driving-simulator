using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copCar : MonoBehaviour
{
    public Transform playerCar; // Reference to the player car
    public bool lawsBroken = false; // Set this to true if any laws are broken

    public float followSpeed = 40.0f / 2.237f; // Adjust the speed as needed

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lawsBroken)
        {
            FollowPlayerCar();
        }
    }

    void FollowPlayerCar()
    {
        if (playerCar != null)
        {
            // Move towards the player car
            transform.position = Vector3.MoveTowards(transform.position, playerCar.position, Time.deltaTime * followSpeed);

            // Rotate towards the player car direction
            transform.LookAt(playerCar);

            // You can add additional logic for siren, etc. here
        }
        else
        {
            Debug.LogWarning("Player car reference not set in the cop car script.");
        }
    }
}
