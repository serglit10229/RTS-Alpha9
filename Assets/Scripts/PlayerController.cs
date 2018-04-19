﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Controls the player. Here we choose our "focus" and where to move. */

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{

    public Interactables focus;  // Our current focus: Item, Enemy etc.
    public bool selected = false;
    public LayerMask movementMask;  // Filter out everything not walkable
    public Transform lastPosition;

    public bool army = false;
    public Vector3 armyMid;

    public Vector3 dest;

    public GameObject thisUnit;
    public GameObject destPoint;

    public bool airplane = false;
    public float airSpeed;
    public float airRotationSpeed;
    public bool airMove = false;
    Camera cam;         // Reference to our camera
    PlayerMotor motor;  // Reference to our motor

    // Get references
    void Start()
    {
        cam = Camera.main;
        motor = thisUnit.GetComponent<PlayerMotor>();
        InvokeRepeating("lastPos", 1,1);
    }

    void lastPos()
    {
        lastPosition = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (selected == true)
        {
            // If we press left mouse
            if (Input.GetMouseButtonDown(1))
            {
                // We create a ray
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // If the ray hits
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, movementMask))
                {
                    dest = hit.point;
                    Instantiate(destPoint, dest, Quaternion.identity);
                    if (army == true)
                    {
                        if(airplane == false)
                        {
                            //armyMid = Vector3.zero;
                            motor.MoveToPoint(transform.position + (dest - armyMid));  // Move to where we hit
                            RemoveFocus();
                        }
                        if(airplane == true)
                        {
                            dest.y = 30f;
                            airMove = true;

                        }
                    }
                    else
                    {
                        if(airplane == false)
                        {
                            armyMid = Vector3.zero;
                            motor.MoveToPoint(hit.point);   // Move to where we hit
                            RemoveFocus();
                        }
                        if(airplane == true)
                        {
                            dest.y = 30f;
                            airMove = true;
                        }
                    }


                }
            }

            // If we press right mouse
            if (Input.GetMouseButtonDown(1))
            {
                // We create a ray
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // If the ray hits
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Interactables interactable = hit.collider.GetComponent<Interactables>();
                    if (interactable != null)
                    {
                        SetFocus(interactable);
                    }
                }
            }
        }
        
        if (airplane == true && airMove == true)
        {
            if(army == true)
            {
                float step = airSpeed * Time.deltaTime;
                Vector3 lookDest = transform.position - (transform.position + (dest - armyMid));
                lookDest.y = 0;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-lookDest), Time.deltaTime * airRotationSpeed);
                if (transform.position != dest)
                    transform.Translate(transform.forward * step, Space.World);
                if (transform.position == transform.position + (dest - armyMid))
                    airMove = false;
            }
            else
            {
                float step = airSpeed * Time.deltaTime;
                Vector3 lookDest = transform.position - dest;
                lookDest.y = 0;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-lookDest), Time.deltaTime * airRotationSpeed);
                if (transform.position != dest)
                    transform.Translate(transform.forward * step, Space.World);
                if(transform.position == dest)
                    airMove = false;
            }
        }
    }

    // Set our focus to a new focus
    void SetFocus(Interactables newFocus)
    {
        // If our focus has changed
        if (newFocus != focus)
        {
            // Defocus the old one
            if (focus != null)
                focus.OnDefocused();

            focus = newFocus;   // Set our new focus
            motor.FollowTarget(newFocus);   // Follow the new focus
        }

        newFocus.OnFocused(transform);
    }

    // Remove our current focus
    void RemoveFocus()
    {
        if (focus != null)
            focus.OnDefocused();

        focus = null;
        motor.StopFollowingTarget();
    }

    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, dest);
    }
}