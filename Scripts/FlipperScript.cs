using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipperScript : MonoBehaviour
{
    public GameObject mainCamera;
    bool flipped;

    // Start is called before the first frame update
    void Start()
    {
        flipped = false;

        if (Random.value < 0.33f) // alternate placements
        {
            transform.position = new Vector3(33f, transform.position.y, transform.position.z);
        }
        else if (Random.value < 0.5f)
        {
            transform.position = new Vector3(45.5f, transform.position.y, -19.3f);
            transform.rotation = Quaternion.Euler(0, 90f, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!flipped && other.CompareTag("player"))
        {
            print("flipping: " + transform.name);
            flipped = true;
            mainCamera.transform.rotation = Quaternion.Euler(90f, 180f, 0);
            StartCoroutine("ResetCamera");
        }
    }

    IEnumerator ResetCamera()
    {
        yield return new WaitForSeconds(2f);
        mainCamera.transform.rotation = Quaternion.Euler(90f, 0, 0);
    }
}
