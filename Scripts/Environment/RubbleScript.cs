using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbleScript : MonoBehaviour
{
    float[] signModifier = new float[3];
    Rigidbody rb;
    public Collider rayColl;
    MeshRenderer mr;
    Vector3 originalPosition;
    const float SHADOW_DISTANCE = 0.2f;

    List<GameObject> tempMList = new List<GameObject>();
    const float MAX_DISTANCE = 0.75f;

    RaycastHit hit; //pack detection variables
    bool validHit; //
    GameObject hitObject; //
    List<GameObject> targets = new List<GameObject>(); //
    int removedNum; //

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 0.2f)
        {
            rayColl.enabled = false;
            if(Vector3.Distance(originalPosition, transform.position) > SHADOW_DISTANCE)
                mr.enabled = false; // disables shadow-generating meshrenderer but not the shadowless meshrenderer
        }
    }

    public void Struck(Vector3 vVect)
    {
        mr.enabled = false; // disables shadow-generating meshrenderer but not the shadowless meshrenderer

        rb.velocity = vVect;
        NearbyReaction();
        rayColl.enabled = false; // once it is struck directly, it can no longer be struck again
    }

    public void NearbyStruck()
    {
        mr.enabled = false; // disables shadow-generating meshrenderer but not the shadowless meshrenderer

        FreshSignModifier();
        rb.velocity = new Vector3(rb.velocity.x + GetTheRandom() * signModifier[0], rb.velocity.y + GetTheRandom() * signModifier[1], rb.velocity.z + GetTheRandom() * signModifier[2]);
    }

    private float GetTheRandom()
    {
        return Random.Range(2f, 3f);
    }

    private void FreshSignModifier()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Random.value < 0.5f)
                signModifier[i] = 1f;
            else
                signModifier[i] = -1f;
        }
    }

    List<GameObject> tempList = new List<GameObject>();

    private void NearbyReaction()
    {
        tempList = GetSameNearby();
        foreach (GameObject o in tempList)
        {
            o.GetComponent<RubbleScript>().NearbyStruck();
        }
    }

    private List<GameObject> GetSameNearby() // checks for nearby monsters of same type
    {
        removedNum = 0;
        tempMList = GetTargets();
        int savInt = tempMList.Count;
        for (int i = 0; i < savInt; i++)
        {
            if (Get2DDistance(tempMList[i - removedNum].transform, transform) > MAX_DISTANCE) //checks if too far away
            {
                tempMList.RemoveAt(i - removedNum);
                removedNum++;
            }
            else if (!tempMList[i - removedNum].transform.CompareTag("rubble")) //checks if of different object type
            {
                tempMList.RemoveAt(i - removedNum);
                removedNum++;
            }
        }

        return tempMList;

    }

    public List<GameObject> GetTargets() // gathers all surrounding objects
    {
        targets.Clear();

        for (int i = 0; i < 10; i++) // first quadrent
        {
            IncludeIfUnique((1f * (10 - i) / 10), (1f * i / 10), i);
        }
        for (int i = 0; i < 10; i++) // second quadrent
        {
            IncludeIfUnique(-(1f * (10 - i) / 10), (1f * i / 10), i);
        }
        for (int i = 0; i < 10; i++) // third quadrent
        {
            IncludeIfUnique(-(1f * (10 - i) / 10), -(1f * i / 10), i);
        }
        for (int i = 0; i < 10; i++) // fourth quadrent
        {
            IncludeIfUnique(-(1f * (10 - i) / 10), -(1f * i / 10), i);
        }
        return targets;
    }

    private void IncludeIfUnique(float x, float z, int i) // adds if not already in list
    {
        if(Physics.Raycast(new Vector3(transform.position.x, 0.5f, transform.position.z), new Vector3(x, 0, z), out hit));
        {
            hitObject = hit.transform.gameObject;
            validHit = true;
            foreach (GameObject t in targets)
                if (t == hitObject)
                    validHit = false;
            if (validHit)
                targets.Add(hitObject);
        }
    }

    public float Get2DDistance(Transform t1, Transform t2)
    {
        return Mathf.Sqrt(Mathf.Pow(t1.position.x - t2.position.x, 2f) + Mathf.Pow(t1.position.z - t2.position.z, 2f));
    }
}
