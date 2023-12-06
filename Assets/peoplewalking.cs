using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class peoplewalking : MonoBehaviour
{
    public float dist;
    public float velocity;
    private char direction;
    public float waitTime;
    private Vector3 startVector;
    private Vector3 endVector;
    private Animator animation_controller;

    // Start is called before the first frame update
    void Start()
    {
        if (dist == 0f){dist = 15f;}
        Vector3 rotationDeg = transform.eulerAngles;
        startVector = transform.position;
        Debug.Log(transform.rotation);
        if(transform.rotation.y == 0f){
            endVector = new Vector3(transform.position.x,transform.position.y,transform.position.z + dist);
        }
        // else if(transform.rotation.y == 1f || transform.rotation.y == -1f){
        else if(Mathf.Approximately(rotationDeg.y, 180f)){
            endVector = new Vector3(transform.position.x,transform.position.y,transform.position.z - dist);
        }
        else if(Mathf.Approximately(rotationDeg.y, 90f)){
            endVector = new Vector3(transform.position.x + dist,transform.position.y,transform.position.z);
        }
        else if(Mathf.Approximately(rotationDeg.y, 270f)){
            endVector = new Vector3(transform.position.x - dist,transform.position.y,transform.position.z);
        }
        animation_controller = GetComponent<Animator>();
        StartCoroutine(MoveAndWait());
    }

    IEnumerator MoveAndWait()
    {
        while (true)
        {
            Debug.Log(animation_controller.GetBool("isStopped"));

            // Move to the end vector
            yield return StartCoroutine(MoveToPosition(endVector));

            // Rotate the character by 180 degrees in the specified direction
            RotateCharacter();

            // Set idle animation
            animation_controller.SetBool("isStopped", true);

            // Wait for 2 seconds
            yield return new WaitForSeconds(waitTime);


            // Move back to the start vector
            yield return StartCoroutine(MoveToPosition(startVector));

            // Rotate the character by 180 degrees in the specified direction
            RotateCharacter();

            // Set idle animation
            animation_controller.SetBool("isStopped", true);
            
            // Wait for 2 seconds
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

    // Function to rotate the character by 180 degrees in the specified direction
    void RotateCharacter(){transform.Rotate(0f, 180f, 0f);}
}