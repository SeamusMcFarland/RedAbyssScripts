using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Light doorLight;
    public Renderer minimapRenderer;
    public Material openMaterial;
    bool opened;

    private void Start()
    {
        if(!opened)
            doorLight.color = Color.red;
    }

    public void Opened()
    {
        opened = true;
        transform.position = new Vector3(300f, 300f, 300f);
        doorLight.color = new Color(1f,0.5f,0,1f);
        minimapRenderer.material = openMaterial;
    }

}
