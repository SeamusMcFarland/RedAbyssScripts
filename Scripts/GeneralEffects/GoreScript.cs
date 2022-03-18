using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoreScript : MonoBehaviour
{

    public Material[] goreM;
    public float[] relIS;
    Rigidbody rb;
    bool used;
    float artRotVel;
    float newZR;
    bool largeException;
    bool brainsException;
    bool sinking;

    const float BASE_FRICTION = 0.01f;
    const float LARGE_MODIFIER = 0.1f; // prevents larger pieces from sliding and spinning as much.
    const float BLOOD_MODIFIER = 0.5f;
    Vector3 savedScale;
    float savedMass;
    const float SINKING_RATE = 0.997f;

    SceneManagerScript smS;

    float frameNormalizer;
    
    // Start is called before the first frame update
    void Start()
    {
        smS = GameObject.FindWithTag("scenemanager").GetComponent<SceneManagerScript>();
        largeException = false;
        used = false;
        rb = GetComponent<Rigidbody>();
        artRotVel = Random.Range(-20f, 20f);
        newZR = 0f;
        sinking = false;
        savedMass = rb.mass;
    }

    // Update is called once per frame
    void Update()
    {
        frameNormalizer = Time.deltaTime / 0.014f;
        if (used == true && Mathf.Abs(artRotVel) > 0.1f)
        {
            if (artRotVel > 0.05f)
                artRotVel -= BASE_FRICTION * Mathf.Pow(artRotVel, 2) + BASE_FRICTION;
            if (artRotVel < 0.05f)
                artRotVel += BASE_FRICTION * Mathf.Pow(artRotVel, 2) + BASE_FRICTION;
            newZR += artRotVel * frameNormalizer;
            transform.rotation = Quaternion.Euler(90f, 0f, newZR);
        }

        if (sinking && savedScale.x * 0.7f < transform.localScale.x)
        {
            transform.localScale = new Vector3(transform.localScale.x * SINKING_RATE, transform.localScale.y * SINKING_RATE, transform.localScale.z);
        }
    }

    public void PlaceGore(int num, Vector3 pos)
    {
        newZR = Random.Range(-180f,180f);
        rb.mass = savedMass;
        sinking = false;
        if (num == 12)
            largeException = true;
        else
            largeException = false;
        if (num == 115 || num == 116 || num == 117)
            brainsException = true;
        else
            brainsException = false;

        used = true;
        GetComponent<Renderer>().material = goreM[num];
        savedScale = new Vector3(relIS[num * 2], relIS[num * 2 + 1], 1f);
        transform.localScale = savedScale;
        transform.position = new Vector3(pos.x + Random.Range(-1f, 1f), Random.Range(0.0010f, 0.0020f), pos.z + Random.Range(-1f, 1f));
        if (largeException)
        {
            rb.velocity = new Vector3(Random.Range(-10f * LARGE_MODIFIER, 10f * LARGE_MODIFIER), 0f, Random.Range(-10f, 10f) * LARGE_MODIFIER);
            rb.mass *= 2f;
            artRotVel = Random.Range(-20, 20f) * LARGE_MODIFIER;
        }
        else if (brainsException)
        {
            rb.velocity = transform.right * Random.Range(8f, 14f);
            artRotVel = 0;
        }
        else if (smS.GetScene() == 4)
        {
            rb.velocity = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)) * BLOOD_MODIFIER;
            artRotVel = Random.Range(-20, 20f) * 0.25f;
            sinking = true;
        }
        else
        {
            rb.velocity = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
            artRotVel = Random.Range(-20, 20f);
        }
    }

}
