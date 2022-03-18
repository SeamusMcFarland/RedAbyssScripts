using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeScript : MonoBehaviour
{
    const float FADE_RATE = 0.02f;
    MeshRenderer mr;
    bool swiping;
    Rigidbody rb;

    bool hitboxTriggered; // if collider triggered
    bool hitboxActive; // prevents multi-hits and near-end hits

    const float MOVEMENT_SPEED = 30f;
    const float DAMAGE = 2f;

    GameObject player;
    PlayerScript playerS;

    float frameNormalizer;

    float currentAlpha;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        playerS = player.GetComponent<PlayerScript>();

        mr = GetComponentInChildren<MeshRenderer>();
        mr.enabled = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        mr.material.color = new Color(mr.material.color.r, mr.material.color.g, mr.material.color.b, currentAlpha);

        frameNormalizer = Time.deltaTime / 0.014f;

        if (swiping)
        {
            rb.velocity = transform.right * MOVEMENT_SPEED;
            if (hitboxTriggered && hitboxActive)
            {
                hitboxActive = false;
                float tempXDiff = player.transform.position.x - transform.position.x;
                float tempZDiff = player.transform.position.z - transform.position.z;
                float tempDevide = Mathf.Abs(tempXDiff) + Mathf.Abs(tempZDiff);
                playerS.KnockOver(0.1f * new Vector3(tempXDiff / tempDevide, 0, tempZDiff / tempDevide));
                playerS.Hit(DAMAGE);
            }
        }
    }

    public void Swipe(Vector3 pos, float rot)
    {
        hitboxActive = true;

        transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        print("position IMMEDIATELY before SET: " + transform.position + " of: " + this.name);
        print("position to set to: " + pos + " of: " + this.name);
        rb.MovePosition(new Vector3(pos.x, transform.position.y, pos.z));
        print("position IMMEDIATELY after SET: " + transform.position + " of: " + this.name);

        transform.rotation = Quaternion.Euler(90f, -rot, 0);
        swiping = true;

        mr.enabled = true;
        currentAlpha = 1f;
        StartCoroutine("FadeOut", 0f);

        print("specific called after: " + transform.position);
        print("specific called name: " + this.name);
    }

    IEnumerator FadeOut(float trans)
    {
        yield return new WaitForSeconds(0.01f);
        print("position during fadeout: " + transform.position + " of: " + this.name);
        if (trans < 1f)
        {
            if(trans > 0.5f)
                hitboxActive = false;

            currentAlpha = 1f - trans;
            transform.localScale = new Vector3(1.5f - trans, 1.5f - trans, 1f); // gets smaller over time
            StartCoroutine("FadeOut", trans + FADE_RATE * frameNormalizer);
        }
        else
        {
            hitboxActive = false;
            currentAlpha = 0;
            mr.enabled = false;
            swiping = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
            hitboxTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
            hitboxTriggered = false;
    }
}
