﻿using UnityEngine;
using System.Collections;

public class CamMove : MonoBehaviour
{

    public GameObject player;       //Public variable to store a reference to the player game object

    private Vector3 offset;         //Private variable to store the offset distance between the player and camera

    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        //transform.position = player.transform.position + offset;
        //transform.LookAt(player.transform.position);

        transform.position = player.transform.position - transform.forward * 25;
        //transform.position = player.transform.position + transform.up * 15;
        //transform.rotation = Quaternion.AngleAxis(30, Vector3.up);
        transform.LookAt(transform);
    }
}
