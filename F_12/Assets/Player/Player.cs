using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PipeSystem pipeSystem;

    public float velocity;

    private Pipe currentPipe;
    
    private float distanceTraveled;
    
    private float deltaToRotation;
    private float systemRotation;
    
    private Transform world, rotater; // référence to the world
    private float worldRotation, avatarRotation;

    public float rotationVelocity;

    private bool turning, isTurningRight, isTurningLeft;
    
    // Start is called before the first frame update
    void Start()
    {
        world = pipeSystem.transform.parent;
        rotater = transform.GetChild(0);
        currentPipe = pipeSystem.SetupFirstPipe();
        SetupCurrentPipe();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            turning = true;
            isTurningRight = true;
            isTurningLeft = false;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            turning = true;
            isTurningRight = false;
            isTurningLeft = true;
        }
        else
        {
            turning = false;
            isTurningRight = false;
            isTurningLeft = false;
        }
        
        float delta = velocity * Time.deltaTime;
        distanceTraveled += delta;
        systemRotation += delta * deltaToRotation;
        
        if (systemRotation >= currentPipe.CurveAngle) {
            delta = (systemRotation - currentPipe.CurveAngle) / deltaToRotation;
            currentPipe = pipeSystem.SetupNextPipe();
            SetupCurrentPipe();
            systemRotation = delta * deltaToRotation;
        }
        
        pipeSystem.transform.localRotation =
            Quaternion.Euler(0f, 0f, systemRotation);

        UpdateAvatarRotation();
    }
    
    private void SetupCurrentPipe () {
        deltaToRotation = 360f / (2f * Mathf.PI * currentPipe.CurveRadius);
        worldRotation += currentPipe.RelativeRotation;
        if (worldRotation < 0f) {
            worldRotation += 360f;
        }
        else if (worldRotation >= 360f) {
            worldRotation -= 360f;
        }
        world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
    }
    
    private void UpdateAvatarRotation () {
        avatarRotation +=
            rotationVelocity * Time.deltaTime * Input.GetAxis("Horizontal");
        if (avatarRotation < 0f) {
            avatarRotation += 360f;
        }
        else if (avatarRotation >= 360f) {
            avatarRotation -= 360f;
        }
        rotater.localRotation = Quaternion.Euler(avatarRotation, 0f, 0f);
        //rotater.GetChild(0).GetChild(0).localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void FixedUpdate()
    {
        if (turning)
        {
            if (isTurningRight)
            {
                rotater.GetChild(0).GetChild(0).localRotation = Quaternion.Euler(20f, 80f, 20f);
            }
            else if (isTurningLeft)
            {
                rotater.GetChild(0).GetChild(0).localRotation = Quaternion.Euler(20f, 110f, -20f);
            }
        }
        else
        {
            rotater.GetChild(0).GetChild(0).localRotation = Quaternion.Euler(0f, 90f, 0f);
        }
    }
}
