using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightReddenerScript : MonoBehaviour
{

    Light light;
    PlayerScript playerS;

    // Start is called before the first frame update
    void Start()
    {
        playerS = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerScript>();
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckColor(playerS.GetHealthRatio());
    }

    private void CheckColor(float red)
    {
        light.color = new Color(1f, 1f * red, 1f * red);
    }

}
