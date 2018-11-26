using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    int scaleFactor = 300;
    Vector3 planePosition = new Vector3(0, 0, 20);
    public void GenerateTerrain() {
        Texture2D hMap = Resources.Load<Texture2D>("Textures/HeightMap");
 
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        
        //Bottom left section of the map, other sections are similar
        for(int i = 0; i < 250; i++)
        {
            for(int j = 0; j < 250; j++)
            {
                //Add each new vertex in the plane
                verts.Add(new Vector3(i, hMap.GetPixel(i,j).grayscale * scaleFactor, j));
                //Skip if a new square on the plane hasn't been formed
                if (i == 0 || j == 0) continue;
                //Adds the index of the three vertices in order to make up each of the two tris
                tris.Add(250 * i + j); //Top right
                tris.Add(250 * i + j - 1); //Bottom right
                tris.Add(250 * (i - 1) + j - 1); //Bottom left - First triangle
                tris.Add(250 * (i - 1) + j - 1); //Bottom left 
                tris.Add(250 * (i - 1) + j); //Top left
                tris.Add(250 * i + j); //Top right - Second triangle
            }
        }
        
        Vector2[] uvs = new Vector2[verts.Count];
        for (var i = 0; i < uvs.Length; i++) //Give UV coords X,Z world coords
            uvs[i] = new Vector2(verts[i].x, verts[i].z);
    
        GameObject plane = new GameObject("ProcPlane"); //Create GO and add necessary components
        plane.AddComponent<MeshFilter>();
        plane.AddComponent<MeshRenderer>();
        plane.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
        Mesh procMesh = new Mesh();
        procMesh.vertices = verts.ToArray(); //Assign verts, uvs, and tris to the mesh
        procMesh.uv = uvs;
        procMesh.triangles = tris.ToArray();
        procMesh.RecalculateNormals(); //Determines which way the triangles are facing
        plane.GetComponent<MeshFilter>().mesh = procMesh; //Assign Mesh object to MeshFilter
        plane.transform.position = planePosition;
    }
}
