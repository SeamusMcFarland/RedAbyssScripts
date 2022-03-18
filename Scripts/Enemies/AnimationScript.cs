using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{

    public bool noScaling;
    public bool noCycling;
    public int scramble;
    private int aNum;
    public int aState; //1 is idle, 2 is strike, 3 is run //can set to 3 publicly to do an automatic restricted cycle
    public Material[] aMat;
    public float[] relIS; //set relative to dimensions of each individual material image so that
                            //the object changes size preventing stretch and squish
    public float animationGap;
    public float rotationSpeed;
    float rotation;
    private float timePassed;

    public int[] extraStateStartPoints;
    public float[] offsets;
    Vector3 savedPosition;

    Renderer rend;

    public bool yOffsets;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        savedPosition = transform.localPosition;
        aNum = 0;
        if (aState == 0)
            aState = 1;
        timePassed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        rend.material = aMat[aNum + scramble];
        if(!noScaling)
            transform.localScale = new Vector3(relIS[(aNum + scramble) *2], relIS[(aNum + scramble) *2 + 1], 1);
        if (offsets.Length > 0)
            CheckOffset();
        checkIterate();
        timePassed += Time.deltaTime;
        if (rotationSpeed > 0)
        {
            rotation += rotationSpeed;
            transform.localRotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    public void SetState(int num)
    {
        if(num < 4)
            aNum = num - 1;
        else
            aNum = extraStateStartPoints[num - 4];
        aState = num;
    }

    public void SetMaterial(int num) // for overriding states entirely
    {
        aNum = num - 1;
        transform.localScale = new Vector3(relIS[aNum * 2], relIS[aNum * 2 + 1], 1);
        rend.material = aMat[aNum];
    }

    private void checkIterate()
    {
        if (timePassed > animationGap && timePassed > 0.03f) // Checks if it is time to change animation frame ALSO puts a minimum of 0.03 seconds before able to move on to next frame
        {
            if (aState == 3) //checks if is in the running state which is the only animated state
            {
                if (extraStateStartPoints.Length == 0)
                {
                    timePassed = 0;
                    if (aNum + scramble < aMat.Length - 1)
                        aNum++;
                    else if (!noCycling)
                        aNum = 2 - scramble; // sets to beginning animation frame
                }
                else
                {
                    timePassed = 0;
                    if (aNum + scramble < extraStateStartPoints[0] - 1)
                        aNum++;
                    else if (!noCycling)
                        aNum = 2 - scramble; // sets to beginning animation frame
                }
            }
            else if (aState > 3)
            {
                if (aState < extraStateStartPoints.Length + 3)
                {
                    timePassed = 0;
                    if (aNum < extraStateStartPoints[aState - 3] - 1)
                        aNum++;
                    else if (!noCycling)
                        aNum = extraStateStartPoints[aState - 4]; // sets to beginning animation frame
                }
                else
                {
                    timePassed = 0;
                    if (aNum < aMat.Length - 1)
                        aNum++;
                    else if (!noCycling)
                        aNum = extraStateStartPoints[aState - 4]; // sets to beginning animation frame

                }
            }
        }
    }

    private void CheckOffset()
    {
        if(yOffsets)
            transform.localPosition = new Vector3(savedPosition.x, savedPosition.y + offsets[aNum], transform.localPosition.z);
        else
            transform.localPosition = new Vector3(savedPosition.x + offsets[aNum], savedPosition.y, transform.localPosition.z);
    }

    public void SetAnimationGap(float g)
    {
        animationGap = g;
    }
    
}
