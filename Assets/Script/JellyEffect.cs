using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyEffect : MonoBehaviour
{
    public float intensity = 1;
    public float stiffness = 0.5f;
    public float mass = 2;
    public float damping = 0.7f;

    //LOGIC
    Mesh workingMesh;
    Mesh originalMesh;
    Vector3 p_velocity;
    class Vertex
    {
        public Vector3 pos;
        public float intensity;
        public float damping;
        public float stiffness;
        public float mass;

        Vector3 velocity;

        public Vertex(Vector3 pos, float intensity, float damping, float stiffness, float mass)
        {
            this.pos = pos;
            this.intensity = intensity;
            this.damping = damping;
            this.stiffness = stiffness;
            this.mass = mass;
        }

        public void Update(Vector3 newPos)
        {
            var force = (newPos - pos) * stiffness;
            var acceleration = force / mass;
            //caculate velocity
            velocity = (velocity + acceleration) * damping;
            //add velocity
            pos += velocity;
            //intensity
            pos = Vector3.Lerp(pos, newPos, 1f - intensity);
        }
    }

    Vertex[] vertices;

    MeshRenderer _renderer;
    MeshRenderer renderer
    {
        get { return _renderer ? _renderer : _renderer = GetComponent<MeshRenderer>(); }

    }
   
    private void Start()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        workingMesh = Instantiate(filter.sharedMesh);
        originalMesh = filter.sharedMesh;
        filter.sharedMesh = workingMesh;
        vertices = new Vertex[originalMesh.vertexCount];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vertex(transform.TransformPoint(originalMesh.vertices[i]), intensity, damping, stiffness, mass);
        }
    }

    private void FixedUpdate()
    {
        //get new vertex pos
        var newVertices = originalMesh.vertices;
        //set new vertex pos
        for (int i = 0; i < newVertices.Length; i++)
        {
            vertices[i].damping = damping;
            vertices[i].stiffness = stiffness;
            vertices[i].mass = mass;
            vertices[i].intensity = ((vertices[i].pos.y - renderer.bounds.min.y) / renderer.bounds.size.y) * intensity;
            vertices[i].Update(transform.TransformPoint(newVertices[i]));
            newVertices[i] = transform.InverseTransformPoint(vertices[i].pos);
        }
        //update mesh
        workingMesh.vertices = newVertices;
        

    }

    
}
