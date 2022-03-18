using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantishScript : MonoBehaviour
{
    StrikeEffectManagerScript semS;

    PlayerScript playerS;
    BloodManagerScript bloodMS;
    GoreManagerScript goreMS;

    bool cooldown;
    bool hitboxTriggered;
    const float BASE_DAMAGE = 1f;

    // Start is called before the first frame update
    void Start()
    {
        semS = GameObject.FindGameObjectWithTag("strikeeffectmanager").GetComponent<StrikeEffectManagerScript>();

        playerS = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerScript>();
        bloodMS = GameObject.FindGameObjectWithTag("bloodmanager").GetComponent<BloodManagerScript>();
        goreMS = GameObject.FindGameObjectWithTag("goremanager").GetComponent<GoreManagerScript>();

        cooldown = false;
        hitboxTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitboxTriggered && !cooldown)
        {
            cooldown = true;
            StartCoroutine("EndCooldown");
            playerS.Hit(BASE_DAMAGE);
        }
    }

    IEnumerator EndCooldown()
    {
        yield return new WaitForSeconds(1f);
        cooldown = false;
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

    public void DestroyPlantish()
    {
        for (int i = 45; i < 49; i++)
            goreMS.SpawnGore(i, new Vector3(transform.position.x, 0.03f, transform.position.z));
        for (int i = 45; i < 49; i++)
            goreMS.SpawnGore(i, new Vector3(transform.position.x, 0.03f, transform.position.z));
        for (int i = 0; i < 2; i++)
            bloodMS.SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), 0.03f, transform.position.z + Random.Range(-1.5f, 1.5f)));
        transform.position = new Vector3(300f,300f,300f);
        semS.SpawnStrikeEffect(transform.position, 1);
    }

}
