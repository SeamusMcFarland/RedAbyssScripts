using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rb;
    private float zoomHeight;
    private float newVelX;
    private float newVelY;
    private float newVelZ;
    private float xOffset;
    private float yOffset;
    private float zOffset;
    private float ACC_SPEED = 2f;

    bool detatched;

    int shakeIterator;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        rb = this.GetComponent<Rigidbody>();
        zoomHeight = 20f;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + zoomHeight, player.transform.position.z);
        shakeIterator = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!detatched)
        {
            //note that offset is flipped so that it is always directed towards the player
            xOffset = player.transform.position.x - this.transform.position.x;
            yOffset = player.transform.position.y - (this.transform.position.y - zoomHeight);
            zOffset = player.transform.position.z - this.transform.position.z;
            newVelX = Mathf.Sign(xOffset) * Mathf.Pow(xOffset, 2) * ACC_SPEED;
            newVelY = Mathf.Sign(yOffset) * Mathf.Pow(yOffset, 2) * ACC_SPEED;
            newVelZ = Mathf.Sign(zOffset) * Mathf.Pow(zOffset, 2) * ACC_SPEED;
            if (Mathf.Abs(newVelX) < 0.1f)
                newVelX = 0;
            if (Mathf.Abs(newVelY) < 0.1f)
                newVelY = 0;
            if (Mathf.Abs(newVelZ) < 0.1f)
                newVelZ = 0;
            rb.velocity = new Vector3(newVelX, newVelY, newVelZ);
            if (Vector3.Distance(player.transform.position, transform.position) > 70f) // for re-focusing
            {
                rb.velocity = new Vector3(0, 0, 0);
                transform.position = new Vector3(player.transform.position.x, player.transform.position.y + zoomHeight, player.transform.position.z);
            }
        }
    }

    public void ScreenShake(float range)
    {
        if (!detatched)
        {
            shakeIterator = 6;
            StartCoroutine("Shake", range);
        }
    }

    public void ScreenShake(float range, int duration)
    {
        if (!detatched)
        {
            shakeIterator = 6 * duration;
            StartCoroutine("Shake", range);
        }
    }

    IEnumerator Shake(float range)
    {
        if(!detatched)
            transform.position = new Vector3(transform.position.x + Random.Range(-range, range), transform.position.y, transform.position.z + Random.Range(-range, range));
        yield return new WaitForSeconds(0.01f);
        if (!detatched)
        {
            shakeIterator--;
            if (shakeIterator > 0)
                StartCoroutine("Shake", range);
        }
    }

    public void DetatchEndGame()
    {
        detatched = true;
        rb.velocity = new Vector3(0,0,0);
        transform.position = new Vector3(-211.5f,18.1f,120.9f);
    }

}
