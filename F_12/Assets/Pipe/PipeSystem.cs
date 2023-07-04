using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSystem : MonoBehaviour
{
    public Pipe pipePrefab;

    public int pipeCount;

    private Pipe[] pipes;

    private void Awake () {
        pipes = new Pipe[pipeCount];
        for (int i = 0; i < pipes.Length; i++) {
            Pipe pipe = pipes[i] = Instantiate<Pipe>(pipePrefab);
            pipe.transform.SetParent(transform, false);
            pipe.Generate();
            if (i > 0) {
                pipe.AlignWith(pipes[i - 1]);
            }
        }
        AlignNextPipeWithOrigin();
    }
    
    private Vector3 CalculateColliderPosition(float curveRadius, float curveAngle, float pipeRadius)
    {
        // Calculer la position du collider en utilisant les paramètres de positionnement du tube
        /*float colliderY = curveRadius + pipeRadius;
        Vector3 colliderPosition = new Vector3(0f, colliderY, 0f);

        return colliderPosition;*/
        // Calculer la position du collider en utilisant les paramètres de positionnement du tube
        float colliderY = curveRadius + pipeRadius;
        float colliderX = Mathf.Cos(curveAngle * Mathf.Deg2Rad) * (curveRadius + pipeRadius);
        float colliderZ = Mathf.Sin(curveAngle * Mathf.Deg2Rad) * (curveRadius + pipeRadius);
        Vector3 colliderPosition = new Vector3(colliderX, colliderY, colliderZ);

        return colliderPosition;
    }
    
    private Vector3 CalculateColliderSize(Vector3[] vertices)
    {
        // Calculer la taille du collider en utilisant les vertices du tube généré
        Vector3 minVertex = vertices[0];
        Vector3 maxVertex = vertices[0];

        // Trouver les coordonnées minimales et maximales parmi les vertices
        for (int i = 1; i < vertices.Length; i++)
        {
            minVertex = Vector3.Min(minVertex, vertices[i]);
            maxVertex = Vector3.Max(maxVertex, vertices[i]);
        }

        // Calculer la taille en utilisant les différences entre les coordonnées minimales et maximales
        Vector3 size = maxVertex - minVertex;

        return size;
    }
    
    public Pipe SetupFirstPipe () {
        transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);
        return pipes[1];
    }
    
    public Pipe SetupNextPipe () {
        ShiftPipes();
        AlignNextPipeWithOrigin();
        pipes[pipes.Length - 1].Generate();
        pipes[pipes.Length - 1].AlignWith(pipes[pipes.Length - 2]);
        transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);
        return pipes[1];
    }
    
    private void ShiftPipes () {
        Pipe temp = pipes[0];
        for (int i = 1; i < pipes.Length; i++) {
            pipes[i - 1] = pipes[i];
        }
        pipes[pipes.Length - 1] = temp;
    }
    
    private void AlignNextPipeWithOrigin () {
        Transform transformToAlign = pipes[1].transform;
        for (int i = 0; i < pipes.Length; i++) {
            if (i != 1)
            {
                pipes[i].transform.SetParent(transformToAlign);
            }
        }
		
        transformToAlign.localPosition = Vector3.zero;
        transformToAlign.localRotation = Quaternion.identity;
		
        for (int i = 1; i < pipes.Length; i++) {
            pipes[i].transform.SetParent(transform);
        }
    }
}
