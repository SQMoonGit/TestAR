﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SnakeController : MonoBehaviour
{
    private TrackedPlane trackedPlane;
    private GameObject snakeInstance;

    public GameObject snakeHeadPrefab;
    public GameObject pointer;
    public Camera firstPersonCamera;
    //Speed to move
    public float speed = 20f;

    void Update()
    { 
        if (snakeInstance == null || snakeInstance.activeSelf == false)
        {
            pointer.SetActive(false);
            return;
        }
        else
            pointer.SetActive(true);

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds;

        if (Frame.Raycast(Screen.width/2, Screen.height/2, raycastFilter, out hit))
        {
            Vector3 pt = hit.Pose.position;
            //Set the Y to the Y of the snakeInstance
            pt.y = snakeInstance.transform.position.y;
            //Set the y position relative to the plane and attach the pointer to the plane
            Vector3 pos = pointer.transform.position;
            pos.y = pt.y;
            pointer.transform.position = pos;

            //now lerp to the position
            pointer.transform.position = Vector3.Lerp(pointer.transform.position, pt, Time.smoothDeltaTime * speed);

            //move towards pointer, slow down if very close
            float dist = Vector3.Distance(pointer.transform.position, snakeInstance.transform.position) - 0.05f;
            if (dist < 0)
                dist = 0;

            Rigidbody rb = snakeInstance.GetComponent<Rigidbody>();
            rb.transform.LookAt(pointer.transform.position);
            rb.velocity = snakeInstance.transform.localScale.x * snakeInstance.transform.forward * dist / .01f;

        }
    }

    public void SetPlane(TrackedPlane plane)
    {
        trackedPlane = plane;
        //Spawn a new snake
        SpawnSnake();
    }

    void SpawnSnake()
    {
        if (snakeInstance != null)
            DestroyImmediate(snakeInstance);
        Vector3 pos = trackedPlane.CenterPose.position;
        pos.y += 0.1f;

        //Not anchored, it is rigidbody that is influenced by the physics engine
        snakeInstance = Instantiate (snakeHeadPrefab, pos, Quaternion.identity, transform);

        //Pass head to slithering component to make movement work
        GetComponent<Slithering>().Head = snakeInstance.transform;
        snakeInstance.AddComponent<FoodConsumer>();
    }

    public int GetLength()
    {
        return GetComponent<Slithering>().GetLength();
    }
}
