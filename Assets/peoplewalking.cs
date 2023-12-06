using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class peoplewalking : MonoBehaviour
{
    public float dist; // Variable to get the distance the character needs to walk
    public float velocity; // Variable that assigns the velocity to the charter with which it walks
    public float waitTime; // Wait time variable so that you can set a custom time for the charatcer to walk
    private Vector3 startVector; // Stores the starting porition of the character
    private Vector3 endVector; // stores the endpoint vector
    private Animator animation_controller; // gets the animation controller to edit the walking state of the character

    // Start is called before the first frame update
    void Start()
    {
        // Checks if dist is innitialzed or not, if not, it sets it to 15f
        if (dist == 0f){dist = 15f;}
        Vector3 rotationDeg = transform.eulerAngles; // Transforming the rotation angle of the character to degerees
        startVector = transform.position; // Sets the start vector, needed to make the character walk back to this part from the end point
        // Setting the endpoint vector based on the roration. 
        if(transform.rotation.y == 0f){
            endVector = new Vector3(transform.position.x,transform.position.y,transform.position.z + dist);
        }
        else if(Mathf.Approximately(rotationDeg.y, 180f)){
            endVector = new Vector3(transform.position.x,transform.position.y,transform.position.z - dist);
        }
        else if(Mathf.Approximately(rotationDeg.y, 90f)){
            endVector = new Vector3(transform.position.x + dist,transform.position.y,transform.position.z);
        }
        else if(Mathf.Approximately(rotationDeg.y, 270f)){
            endVector = new Vector3(transform.position.x - dist,transform.position.y,transform.position.z);
        }
        // Getting animation controller to set the values of the state. 
        animation_controller = GetComponent<Animator>();
        StartCoroutine(MoveAndWait());
    }

    IEnumerator MoveAndWait() // Function that controlls movement and waiting of the character
    {
        while (true)
        {

            // Move to the end vector
            yield return StartCoroutine(MoveToPosition(endVector));

            // Rotate the character by 180 degrees in the specified direction
            transform.Rotate(0f, 180f, 0f);

            // Set idle animation
            animation_controller.SetBool("isStopped", true);

            // Wait for waitTime seconds
            yield return new WaitForSeconds(waitTime);


            // Move back to the start vector
            yield return StartCoroutine(MoveToPosition(startVector));

            // Rotate the character by 180 degrees in the specified direction
            transform.Rotate(0f, 180f, 0f);

            // Set idle animation
            animation_controller.SetBool("isStopped", true);
            
            // Wait for waitTime seconds
            yield return new WaitForSeconds(waitTime);
        }
    }

    // Coroutine to move to a specific position
    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        // Set walking animation
        animation_controller.SetBool("isStopped", false);
        float t = 0f;
        Vector3 startPosition = transform.position;

        while (t < 1f)
        {
            // Interpolate between start and end positions
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // Update 't' based on velocity and time
            t += velocity * Time.deltaTime;

            yield return null;
        }

        // Ensure the final position is exactly the target position
        transform.position = targetPosition;
    }

}