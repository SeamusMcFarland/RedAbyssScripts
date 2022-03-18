using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthbarScript : MonoBehaviour
{
    public Sprite[] allSprites;
    Image im;
    Canvas parentC;

    // Start is called before the first frame update
    void Start()
    {
        im = GetComponent<Image>();
        parentC = GetComponentInParent<Canvas>();
        DisableHealthbar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateHealthbar(float healthDecimal, Vector3 position)
    {
        UpdateSprite(healthDecimal);
        UpdatePosition(position);
        im.enabled = true;
    }

    public void UpdateHealthbar(float healthDecimal, Vector3 position) // continually called by the associated monster when active
    {
        UpdateSprite(healthDecimal);
        UpdatePosition(position);
    }

    private void UpdateSprite(float healthDecimal)
    {
        if (healthDecimal < 0.0625f * 2f) // emptiest healthbar
            im.sprite = allSprites[15];
        else if (healthDecimal < 0.0625f * 3f)
            im.sprite = allSprites[14];
        else if (healthDecimal < 0.0625f * 4f)
            im.sprite = allSprites[13];
        else if (healthDecimal < 0.0625f * 5f)
            im.sprite = allSprites[12];
        else if (healthDecimal < 0.0625f * 6f)
            im.sprite = allSprites[11];
        else if (healthDecimal < 0.0625f * 7f)
            im.sprite = allSprites[10];
        else if (healthDecimal < 0.0625f * 8f)
            im.sprite = allSprites[9];
        else if (healthDecimal < 0.0625f * 9f)
            im.sprite = allSprites[8];
        else if (healthDecimal < 0.0625f * 10f)
            im.sprite = allSprites[7];
        else if (healthDecimal < 0.0625f * 11f)
            im.sprite = allSprites[6];
        else if (healthDecimal < 0.0625f * 12f)
            im.sprite = allSprites[5];
        else if (healthDecimal < 0.0625f * 13f)
            im.sprite = allSprites[4];
        else if (healthDecimal < 0.0625f * 14f)
            im.sprite = allSprites[3];
        else if (healthDecimal < 0.0625f * 15f)
            im.sprite = allSprites[2];
        else if (healthDecimal < 0.0625f * 16f) // changed to be at 16 (anything less than full health) to induce more of a feeling of progession by showing more empty healthbars
            im.sprite = allSprites[1];
        else // fullest healthbar (will never occur but I'm not taking this out anyway)
            im.sprite = allSprites[0];
    }

    private void UpdatePosition(Vector3 position)
    {
        transform.position = WorldToUISpace(parentC, position);
    }

    private Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;
        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        //Convert the local point to world point
        return parentCanvas.transform.TransformPoint(movePos);
    }

    public void DisableHealthbar()
    {
        im.enabled = false;
    }

    public bool GetEnabled()
    {
        return im.enabled;
    }

}
