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
            if (i > 0) {
                pipe.AlignWith(pipes[i - 1]);
            }
        }
    }
}
