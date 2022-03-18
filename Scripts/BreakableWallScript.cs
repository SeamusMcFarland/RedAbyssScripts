using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BreakableWallScript : MonoBehaviour
{
    MeshRenderer[] piecesRenderer;
    Rigidbody[] piecesRB;
    bool broken;
    Collider coll;
    int health;
    NavMeshObstacle nmo;

    // Start is called before the first frame update
    void Start()
    {
        piecesRB = GetComponentsInChildren<Rigidbody>();
        piecesRenderer = GetComponentsInChildren<MeshRenderer>();

        nmo = GetComponent<NavMeshObstacle>();
        health = 3;
        broken = false;
        coll = GetComponent<Collider>();
        piecesRB = GetComponentsInChildren<Rigidbody>();
    }

    public void BreakWall()
    {
        if (!broken)
        {
            health--;
            if (health < 1)
            {
                CompletelyBreakWall();
            }
        }
    }

    public void CompletelyBreakWall()
    {
        health = 0;
        broken = true;
        nmo.enabled = false;
        coll.enabled = false;
        foreach (Rigidbody rb in piecesRB)
        {
            rb.isKinematic = false;
            rb.velocity = new Vector3(Random.Range(-10f, 10f), Random.Range(0, 10f), Random.Range(-10f, 10f));
        }
        foreach (MeshRenderer mr in piecesRenderer)
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public bool GetDestroyed()
    {
        if (health < 1)
            return true;
        else
            return false;
    }
}
