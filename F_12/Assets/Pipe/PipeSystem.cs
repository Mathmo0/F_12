using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSystem : MonoBehaviour
{
    public Pipe pipePrefab;

    public int pipeCount;

    private Pipe[] pipes;

    private void Awake()
    {
        pipes = new Pipe[pipeCount];
        for (int i = 0; i < pipes.Length; i++)
        {
            Pipe pipe = pipes[i] = Instantiate<Pipe>(pipePrefab);
            pipe.transform.SetParent(transform, false);
            pipe.Generate();
            if (i > 0)
            {
                pipe.AlignWith(pipes[i - 1]);
            }
        }

        AlignNextPipeWithOrigin();
    }

    public Pipe SetupFirstPipe()
    {
        transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);
        return pipes[1];
    }

    public Pipe SetupNextPipe()
    {
        ShiftPipes();
        AlignNextPipeWithOrigin();
        pipes[pipes.Length - 1].Generate();
        pipes[pipes.Length - 1].AlignWith(pipes[pipes.Length - 2]);

        transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);
        return pipes[1];
    }

    bool CheckCollision(Pipe pipe)
    {
        Collider pipeCollider = pipe.GetComponent<Collider>();

        foreach (Pipe pipebis in pipes)
        {
            // Vérifiez si le collider du pipe entre en collision avec un autre collider
            if (pipeCollider.bounds.Intersects(pipebis.GetComponent<BoxCollider>().bounds))
            {
                return true; // Collision détectée
            }
        }

        return false; // Aucune collision détectée
    }
    
    void RandomRotate(Pipe pipe)
    {
        // Générer un angle de rotation aléatoire
        float angle = Random.Range(0f, 360f);

        // Appliquer la rotation autour de l'axe Y (par exemple)
        pipe.transform.rotation = Quaternion.Euler(angle, 0f, 0f);
    }

    
    private void ShiftPipes()
    {
        Pipe temp = pipes[0];
        for (int i = 1; i < pipes.Length; i++)
        {
            pipes[i - 1] = pipes[i];
        }

        pipes[pipes.Length - 1] = temp;
    }

    private void AlignNextPipeWithOrigin()
    {
        Transform transformToAlign = pipes[1].transform;
        for (int i = 0; i < pipes.Length; i++)
        {
            if (i != 1)
            {
                pipes[i].transform.SetParent(transformToAlign);
            }
        }

        transformToAlign.localPosition = Vector3.zero;
        transformToAlign.localRotation = Quaternion.identity;

        for (int i = 0; i < pipes.Length; i++)
        {
            if (i != 1)
            {
                pipes[i].transform.SetParent(transform);
            }
        }
    }
}