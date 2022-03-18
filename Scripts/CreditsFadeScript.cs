using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsFadeScript : MonoBehaviour
{
    Image fadeI;
    float fadeAlpha;
    float frameNormalizer;
    const float FADE_RATE = 1f;
    SceneManagerScript smS;

    bool reverseFade;

    // Start is called before the first frame update
    void Start()
    {
        smS = GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>();
        fadeI = GetComponent<Image>();
        fadeAlpha = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (reverseFade)
        {
            if (fadeAlpha < 1f)
                fadeAlpha += Time.deltaTime * FADE_RATE;
            if (fadeAlpha > 1f)
                fadeAlpha = 1f;
        }
        else
        {
            if (fadeAlpha > 0)
                fadeAlpha -= Time.deltaTime * FADE_RATE;
            if (fadeAlpha < 0)
                fadeAlpha = 0;
        }
      
        RefreshFadeAlpha();

        if (Input.GetMouseButtonDown(0) && fadeAlpha <= 0)
            reverseFade = true;

        if(reverseFade && fadeAlpha >= 1)
            smS.LoadNewScene("MainMenu");
    }


    private void RefreshFadeAlpha()
    {
        var tempColor = fadeI.color;
        tempColor.a = fadeAlpha;
        fadeI.color = tempColor;
    }

}
