using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CronenburgHitboxScript : MonoBehaviour
{
    CronenburgBossScript cbS;
    CronenburgChaseScript ccS;
    public bool isBoss;
    public bool isCharge;

    // Start is called before the first frame update
    void Start()
    {
        if(isBoss)
            cbS = transform.parent.GetComponent<CronenburgBossScript>();
        else
            ccS = transform.parent.GetComponent<CronenburgChaseScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("player"))
        {
            if (isCharge)
                cbS.SetChargeHitbox(true);
            else if (isBoss)
                cbS.SetHitboxTriggered(true);
            else
                ccS.SetHitboxTriggered(true);
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("player"))
        {
            if (isCharge)
                cbS.SetChargeHitbox(false);
            else if (isBoss)
                cbS.SetHitboxTriggered(false);
            else
                ccS.SetHitboxTriggered(false);
        }
    }
}
