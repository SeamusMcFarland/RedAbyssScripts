using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmileyStrikeScript : MonoBehaviour
{

    MeshRenderer mr;
    public Material redM;
    public Material whiteM;

    GameObject player;
    PlayerScript playerS;
    float startup;
    
    int shakeCount;
    Vector3 savedPosition;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(300f,transform.position.y,300f);
        mr = GetComponent<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SmileyStrike(float dam, float st)
    {
        mr.material = redM;
        transform.rotation = Quaternion.Euler(90f, Random.Range(0,360f), transform.rotation.z);
        startup = st;
        transform.position = new Vector3(player.transform.position.x, 0.21f, player.transform.position.z);
        savedPosition = transform.position;
        StartCoroutine("CheckHitbox");
    }

    IEnumerator CheckHitbox() // normal colliders won't activate if player is standing still
    {
        yield return new WaitForSeconds(startup);
        mr.material = whiteM;
        if (Mathf.Abs(player.transform.position.x - transform.position.x) < .7f && Mathf.Abs(player.transform.position.z - transform.position.z) < .7f)
        {
            playerS.Hit(3f);
        }
        shakeCount = 10;
        StartCoroutine("ShakeTillRemove");
    }

    IEnumerator ShakeTillRemove()
    {
        transform.position = new Vector3(savedPosition.x + Random.Range(-0.4f,0.4f), savedPosition.y, savedPosition.z + Random.Range(-0.4f, 0.4f));
        yield return new WaitForSeconds(0.01f);
        if (shakeCount > 0)
        {
            shakeCount--;
            StartCoroutine("ShakeTillRemove");
        }
        else
            transform.position = new Vector3(300f, 300f, 300f);

    }

}
