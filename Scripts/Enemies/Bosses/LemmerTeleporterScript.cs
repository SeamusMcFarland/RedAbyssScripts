using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemmerTeleporterScript : MonoBehaviour
{
    bool active;
    PlayerScript playerS;

    // Start is called before the first frame update
    void Start()
    {
        playerS = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerScript>();
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (active && other.CompareTag("player"))
        {
            active = false;
            playerS.BeginLemmerBossDialogue();
            StartCoroutine("TeleportPlayer");
        }
    }

    IEnumerator TeleportPlayer()
    {
        yield return new WaitForSeconds(3f);
        if (!playerS.GetPlayerDead())
            playerS.TeleportToLemmer(0,-100f);
    }

}
