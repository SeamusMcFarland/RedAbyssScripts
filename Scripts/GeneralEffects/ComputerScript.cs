using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerScript : MonoBehaviour
{
    public int compNum;
    public DoorScript doorS;
    bool used; // initially not set to false since it may need to be overidden by preaccess
    public MeshRenderer cmr; // honestly I don't remember what this stands for, but it's the minimap marker's mesh renderer needed in order to change to green
    public Material offMaterial;
    Light theLight; // initially not set to enabled in script (it is in the scene) since it may need to be overidden by preaccess

    SceneManagerScript smS;

    void Start()
    {
        theLight = GetComponent<Light>();
        smS = GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>();
    }

    public bool Access() // returns true if not already accessed
    {
        if (used == false)
        {
            theLight.enabled = false;
            smS.SaveProgress(compNum);
            used = true;
            doorS.Opened();
            cmr.material = offMaterial;
            return true;
        }
        else
            return false;
    }

    public void PreAccess()
    {
        theLight = GetComponent<Light>(); // in case if preaccess is called before start
        theLight.enabled = false;
        used = true;
        doorS.Opened();
        cmr.material = offMaterial;
    }

    public int GetCompNum()
    {
        return compNum;    
    }

}
