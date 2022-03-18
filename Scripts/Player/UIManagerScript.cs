using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{

    public Sprite[] bulletSprite;
    public GameObject bulletO;
    Image bulletI;
    RectTransform bulletRT;
    int weaponType;

    public Sprite[] shotgunBulletSprite;

    public Sprite[] molotovSprite;
    public GameObject molotovO;
    Image molotovI;
    RectTransform molotovRT;

    public Sprite[] trapSprite;
    public GameObject trapO;
    Image trapI;
    RectTransform trapRT;

    public RectTransform gameOverScreen;
    const float DRAGDOWN_RATE = 13f;
    public GameObject button; // retry
    public GameObject button2; // quit
    public GameObject button3; // main menu

    public GameObject fade;
    RectTransform fadeRT;
    Image fadeI;
    float fadeAlpha;
    const float FADE_RATE = 0.01f;
    PlayerScript playerS;
    SceneManagerScript smS;

    // Start is called before the first frame update
    void Start()
    {
        smS = GameObject.FindWithTag("scenemanager").GetComponent<SceneManagerScript>();
        fadeAlpha = 1f;
        fadeRT = fade.GetComponent<RectTransform>();
        fadeI = fade.GetComponent<Image>();
        playerS = GameObject.FindWithTag("player").GetComponent<PlayerScript>();

        gameOverScreen.position = new Vector2(gameOverScreen.position.x, 2000f);

        bulletI = bulletO.GetComponent<Image>();
        bulletRT = bulletO.GetComponent<RectTransform>();
        weaponType = 1;

        molotovI = molotovO.GetComponent<Image>();
        molotovRT = molotovO.GetComponent<RectTransform>();

        trapI = trapO.GetComponent<Image>();
        trapRT = trapO.GetComponent<RectTransform>();

        bulletI.sprite = bulletSprite[9];
        bulletRT.localScale = new Vector2(3.45f, 0.15f);
    }

    public void SetBulletCount(int num)
    {
        if (weaponType == 1)
        {
            bulletI.sprite = bulletSprite[num]; // if num = 0 scale doesn't matter since it won't be visable
            if (num == 1)
            {
                bulletRT.localScale = new Vector2(0.33f, 0.15f);
            }
            else if (num == 2)
            {
                bulletRT.localScale = new Vector2(0.72f, 0.15f);
            }
            else if (num == 3)
            {
                bulletRT.localScale = new Vector2(1.11f, 0.15f);
            }
            else if (num == 4)
            {
                bulletRT.localScale = new Vector2(1.5f, 0.15f);
            }
            else if (num == 5)
            {
                bulletRT.localScale = new Vector2(1.89f, 0.15f);
            }
            else if (num == 6)
            {
                bulletRT.localScale = new Vector2(2.28f, 0.15f);
            }
            else if (num == 7)
            {
                bulletRT.localScale = new Vector2(2.67f, 0.15f);
            }
            else if (num == 8)
            {
                bulletRT.localScale = new Vector2(3.06f, 0.15f);
            }
            else if (num == 9)
            {
                bulletRT.localScale = new Vector2(3.45f, 0.15f);
            }
        }
        else if (weaponType == 2)
        {
            bulletI.sprite = shotgunBulletSprite[num]; // if num = 0 scale doesn't matter since it won't be visable
            if (num == 1)
            {
                bulletRT.localScale = new Vector2(0.45f, 0.15f);
            }
            else if (num == 2)
            {
                bulletRT.localScale = new Vector2(0.96f, 0.15f);
            }
        }
        else
            print("ERROR! INVALID WEAPON TYPE in UIMANAGERSCRIPT");
    }

    public void SetMolotovCount(int num)
    {
        
        molotovI.sprite = molotovSprite[num];
        if (num == 1)
        {
            molotovRT.localScale = new Vector2(0.39f, 0.42f);

        }
        else if (num == 2)
        {
            molotovRT.localScale = new Vector2(0.84f, 0.42f);

        }
        else if (num == 3)
        {
            molotovRT.localScale = new Vector2(1.29f, 0.42f);

        }
        else if (num == 4)
        {
            molotovRT.localScale = new Vector2(1.74f, 0.42f);

        }
        else if (num == 5)
        {
            molotovRT.localScale = new Vector2(2.19f, 0.42f);

        }
    }

    public void SetTrapCount(int num)
    {
        print("num: " + num);
        trapI.sprite = trapSprite[num];
        if (num == 1)
        {
            trapRT.localScale = new Vector2(0.51f, 0.12f);

        }
        else if (num == 2)
        {
            trapRT.localScale = new Vector2(1.11f, 0.12f);

        }
        else if (num == 3)
        {
            trapRT.localScale = new Vector2(1.71f, 0.12f);

        }
    }

    public void SetWeaponType(int w)
    {
        weaponType = w;
    }

    public void GameOver()
    {
        StartCoroutine("GameOverDragdown");
    }

    public void IntroFade()
    {
        StartCoroutine("DelayFade"); // setup is otherwise faulty since no variables have been assigned by the time this method is called
        //Debug.LogError("intro fading");
    }
    public void RemoveFade()
    {
        StartCoroutine("DelayRemove");
    }

    IEnumerator DelayRemove()
    {
        yield return new WaitForSeconds(0.001f);
        fadeAlpha = 0;
        RefreshFadeAlpha();
        playerS.SetLock(false);
    }

    public void ApplyFade()
    {
        fadeAlpha = 1;
        RefreshFadeAlpha();
        playerS.SetLock(true);
    }

    public void OutroScreen() // immediate blackeng
    {
        fadeRT.position = new Vector2(Screen.width / 2, Screen.height / 2);
        fadeAlpha = 1;
        RefreshFadeAlpha();
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(0.001f);
        StartCoroutine("FadeEffect", 3f);
        if(-1 < smS.GetScene() || smS.GetScene() == -3)
            StartCoroutine("EnablePlayer");
    }

    IEnumerator FadeEffect(float time)
    {
        yield return new WaitForSeconds(time);
        fadeAlpha -= FADE_RATE;
        RefreshFadeAlpha();
        if (fadeAlpha > 0)
            StartCoroutine("FadeEffect", 0.01f);
    }

    private void RefreshFadeAlpha()
    {
        var tempColor = fadeI.color;
        tempColor.a = fadeAlpha;
        fadeI.color = tempColor;
    }

    IEnumerator EnablePlayer()
    {
        yield return new WaitForSeconds(3f);
        playerS.SetLock(false);
    }

    IEnumerator GameOverDragdown()
    {
        yield return new WaitForSeconds(0.01f);
        if (80f < gameOverScreen.anchoredPosition.y) // WAS 380 for unity editor using position
        {
            gameOverScreen.anchoredPosition = new Vector2(gameOverScreen.anchoredPosition.x, gameOverScreen.anchoredPosition.y - DRAGDOWN_RATE);
            StartCoroutine("GameOverDragdown");
        }
        else
        {
            button.SetActive(true);
            button2.SetActive(true);
            button3.SetActive(true);
        }
    }

}
