using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasTankScript : MonoBehaviour
{
    ParticleSystem ps;
    SceneManagerScript smS;
    public int tankNum;
    public MeshRenderer cmr;// honestly I don't remember what this stands for, but it's the minimap marker's mesh renderer needed in order to change to green
    public Material offMaterial;
    bool used;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        if(!used)
            ps.Stop();
        smS = GameObject.FindWithTag("scenemanager").GetComponent<SceneManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BreakGasTank()
    {
        ps.Play();
        smS.TankSaveProgress(tankNum);
        cmr.material = offMaterial;
    }

    public void PreAccess()
    {
        used = true; // needed since otherwise start will stop the effect after preaccess is called
        ps = GetComponentInChildren<ParticleSystem>();
        ps.Play();
        cmr.material = offMaterial;
    }

    public int GetGasTankNum()
    {
        return tankNum;
    }

}
