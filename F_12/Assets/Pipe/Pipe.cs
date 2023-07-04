using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public float pipeRadius;
    public int pipeSegmentCount;
    
    public int curveSegmentCount; //number of segments main curve
    
    public float CurveRadius {
        get {
            return curveRadius;
        }
    }
    public float curveRadius; //number of segments per segment of main curve
    
    public float CurveAngle {
        get {
            return curveAngle;
        }
    }
    private float curveAngle; //angle of main curve
    
    
	
    public float RelativeRotation {
        get {
            return relativeRotation;
        }
    }
    private float relativeRotation; // Remember his position
    
    public float ringDistance; //distance between rings
    
    public float minCurveRadius, maxCurveRadius;
    public int minCurveSegmentCount, maxCurveSegmentCount;
    
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private BoxCollider collider;


    public void AlignWith (Pipe pipe) {
        relativeRotation = Random.Range(0, curveSegmentCount) * 360f / pipeSegmentCount;
        
        transform.SetParent(pipe.transform, false);
        
        transform.localPosition = Vector3.zero; //set position to 0,0,0
        transform.localRotation = Quaternion.Euler(0f, 0f, -pipe.curveAngle);
        transform.Translate(0f, pipe.curveRadius, 0f); //move to end of pipe
        transform.Rotate(relativeRotation, 0f, 0f); //rotate randomly
        transform.Translate(0f, -curveRadius, 0f); //move to start of pipe

        transform.SetParent(pipe.transform.parent);
        transform.localScale = Vector3.one;
        Vector3 center = Vector3.zero;

        foreach (Vector3 vertex in vertices)
        {
            collider.center += new Vector3(vertex.x, vertex.y, 0f);
        }
        
        collider.center /= vertices.Length;
        collider.size = CalculateColliderSize(vertices);
    }
    
    private void Awake () 
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        collider = GetComponent<BoxCollider>();
        mesh.name = "Pipe";
    }

    public void Generate()
    {
        
        curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
        curveSegmentCount = Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);
        mesh.Clear();
        SetVertices();
        SetTriangles();
        mesh.RecalculateNormals();
    }
    
    private void SetVertices()
    {
        vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];
        float uStep = ringDistance / curveRadius; //(2f * Mathf.PI) / curveSegmentCount;
        curveAngle = uStep * curveSegmentCount * (360f / (2f * Mathf.PI));
        CreateFirstQuadRing(uStep);
        int iDelta = pipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta) {
            CreateQuadRing(u * uStep, i);
        }
        mesh.vertices = vertices;
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
        Vector3 colliderPosition = new Vector3(colliderX, colliderY, 0f);

        /*Vector3 center = Vector3.zero;

        foreach (Vector3 vertex in vertices)
        {
            center += vertex;
        }

        center /= vertices.Length;*/
        
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
    
    private void CreateFirstQuadRing (float u) //u = angle along the curve
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;

        Vector3 vertexA = GetPointOnTorus(0f, 0f);
        Vector3 vertexB = GetPointOnTorus(u, 0f);
        for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4) {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
        }
    }
    
    private void CreateQuadRing (float u, int i) {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;
        int ringOffset = pipeSegmentCount * 4;
		
        Vector3 vertex = GetPointOnTorus(u, 0f);
        for (int v = 1; v <= pipeSegmentCount; v++, i += 4) {
            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
        }
    }

    private void SetTriangles()
    {
        triangles = new int[pipeSegmentCount * curveSegmentCount * 6];
        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4) {
            triangles[t] = i;
            triangles[t + 1] = triangles[t + 4] = i + 1;
            triangles[t + 2] = triangles[t + 3] = i + 2;
            triangles[t + 5] = i + 3;
        }
        mesh.triangles = triangles;
    }
    
    private Vector3 GetPointOnTorus (float u, float v) 
    {
        Vector3 p;
        float r = (curveRadius + pipeRadius * Mathf.Cos(v)); //radius of the pipe
        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = pipeRadius * Mathf.Sin(v);
        return p;
    }
    
    private void OnDrawGizmos () 
    {
        float uStep = (2f * Mathf.PI) / curveSegmentCount;
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;

        for (int u = 0; u < curveSegmentCount; u++) {
            for (int v = 0; v < pipeSegmentCount; v++) {
                Vector3 point = GetPointOnTorus(u * uStep, v * vStep);
                Gizmos.color = new Color(
                    1f,
                    (float)v / pipeSegmentCount,
                    (float)u / curveSegmentCount
                );
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}
