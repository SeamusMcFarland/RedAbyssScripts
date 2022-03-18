using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoopScript : MonoBehaviour
{

    public Sprite[] allSprites;
    public float loopTime;

    Image thisImage;
    int iterator;
    float timePassed;
    

    private void Awake()
    {
        thisImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (loopTime <= timePassed)
        {
            timePassed = 0;
            iterator++;
            if (iterator >= allSprites.Length)
                iterator = 0;
            thisImage.sprite = allSprites[iterator];
        }
    }
}
