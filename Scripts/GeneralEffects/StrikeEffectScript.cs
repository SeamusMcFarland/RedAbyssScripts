using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeEffectScript : MonoBehaviour
{
    public float[] XYdimensions;
    public Material[] strikeEffectM;
    int current;
    MeshRenderer mr;

    GameObject player;
    float rotation;
    float xDiff;
    float zDiff;
    Vector3 facingVector;
    float rotationVariation;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("player");
        mr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaceStrikeEffect(Vector3 pos, int modColor)
    {
        if (modColor == 1) // red
            current = 0;
        else if (modColor == 2) // white
            current = 6;
        mr.material = strikeEffectM[current];
        transform.localScale = new Vector3(XYdimensions[current * 2], XYdimensions[current * 2 + 1], 1f);
        CheckRotation();
        transform.position = new Vector3(pos.x + Random.Range(-.6f, .6f), 0.11f, pos.z + Random.Range(-.6f, .6f));
        mr.enabled = true;
        StartCoroutine("AnimateStrike");
    }

    IEnumerator AnimateStrike()
    {
        yield return new WaitForSeconds(0.03f);
        if (current != 5 && current != 11)
        {
            current++;
            mr.material = strikeEffectM[current];
            transform.localScale = new Vector3(XYdimensions[current * 2], XYdimensions[current * 2 + 1], 1f);
            StartCoroutine("AnimateStrike");
        }
        else
        {
            mr.enabled = false;
        }
    }

    private void CheckRotation()
    {
        facingVector = player.transform.position;
        xDiff = facingVector.x - transform.position.x;
        zDiff = facingVector.z - transform.position.z;
        rotation = 0 - (Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff));
        transform.rotation = Quaternion.Euler(90, rotation, 0);
    }

}
