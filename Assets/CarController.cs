//Similar to Claire.cs
/*
Moves: forward (Up arrow), backward (down arrow), turn right (shift + right arrow, reg/1.5), turn left (shift + left arrow, reg/1.5)
Accelerate - shift + up (reg * 3)
Decelerate - shift +down, if done when driving at regular speed, car will come to a gradual stop (reg/2)
Variables:
Speed/Velocity
Animation Controller
Character controller
movement direction
caught
haswon
Lives left?
-----
Conditions:
Stays within bounds of roads
Will remain stopped at the end of a road unless user presses turn
Has to stop if caught by police
Optional stopping for pedestrians crossing
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    private Animator actrl;
    private CharacterController car;
    // public Vector3 mvdir;
    float c_vel;
    public float reg_vel;
    public bool caught;
    public bool haswon;
    bool hasCollided = false;
    void Start()
    {
        car = GetComponent<CharacterController>();
    }
    void Update()
    {
        if(!Input.anyKey)
        {
            c_vel = 0.0f;
        }
        //Check if the new position is above the maximum allowed vertical position
        if (transform.position.y > 0.0f)
        {
            transform.position = new Vector3(transform.position.x,0.0f,transform.position.z);
        }

        if(Input.GetKey(KeyCode.UpArrow))
        {
            //Car moves forward
            //actrl.SetBool("", true);
            if (c_vel < 0) //If character is transitioning from a backwards state - reset c_vel to forward direction (0.0f)
            {
                c_vel = 0.0f;   
            }
            if (c_vel < reg_vel)
            {
                c_vel += .5f;
            }
            else
            {
                c_vel = reg_vel; //Limit the velocity to the walking_velocity when reached
            }
            
            //Accelerate
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if (c_vel < reg_vel * 3.0f)
                {
                    c_vel += .8f;
                }
                else
                {
                    c_vel = reg_vel * 4.0f;
                    Debug.Log("Full speed");
                }
            }
            car.Move(transform.forward * c_vel * Time.deltaTime);
            //Letting go of space will abruptly revert back to regular velocity
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            //Reverse Car
            //actrl.SetBool("", true);
            if (Mathf.Abs(c_vel) < Mathf.Abs(reg_vel/1.5f))
            {
                c_vel = -(Mathf.Abs(c_vel) + .3f);
            }
            else
            {
                c_vel = -(reg_vel/1.5f); //Limit the velocity to the walking_velocity/1.5 when reached
            }

            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if (c_vel > reg_vel/1.5f)
                {
                    c_vel -= .3f;
                }
                else
                {
                    c_vel = reg_vel/1.5f;
                }
            }
            car.Move(transform.forward * c_vel * Time.deltaTime);
        }
        else
        {
            //Car is stopped
        }

        //Turning
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            //actrl.SetBool("", true);
            float rtnIn = Input.GetAxis("Horizontal"); //Rotational Input, -1 - left, 1 - right

            // Rotate the character based on input
            Vector3 rotation = Vector3.up * rtnIn * 60.0f * Time.deltaTime;
            transform.Rotate(rotation);

            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if(c_vel < 0.0f)
                {
                    c_vel = 0.0f;
                }
                if(c_vel <  reg_vel * 2.0f)
                {
                    c_vel += 0.8f;
                }
                else
                {
                    c_vel =reg_vel * 2.0f;
                }
                car.Move(transform.forward * c_vel * Time.deltaTime);
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        string obj = collision.gameObject.name;
        if (obj != "MainCar")
        {
            Debug.Log("Collided with "+obj);
            hasCollided = true;
        }
        // Check for collisions with other objects
        if (obj.StartsWith("elder"))
        {
            // Handle collision with a pedestrian
            Debug.Log("Pedestrian Hit");
        }
        if (obj.StartsWith("Road Concrete")|| obj.StartsWith("Building") || obj.StartsWith("Props"))
        {
            // Handle collision with buildings/props
            Debug.Log("Off road");
        }
    }
}