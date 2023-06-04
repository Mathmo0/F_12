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
    
    // Start is called before the first frame update
    void Start()
    {
        currentPipe = pipeSystem.SetupFirstPipe();
        deltaToRotation = 360f / (2f * Mathf.PI * currentPipe.CurveRadius);
    }

    // Update is called once per frame
    void Update()
    {
        float delta = velocity * Time.deltaTime;
        distanceTraveled += delta;
        systemRotation += delta * deltaToRotation;
        
        if (systemRotation >= currentPipe.CurveAngle) {
            delta = (systemRotation - currentPipe.CurveAngle) / deltaToRotation;
            currentPipe = pipeSystem.SetupNextPipe();
            deltaToRotation = 360f / (2f * Mathf.PI * currentPipe.CurveRadius);
            systemRotation = delta * deltaToRotation;
        }
        
        pipeSystem.transform.localRotation =
            Quaternion.Euler(0f, 0f, systemRotation);
    }
}
