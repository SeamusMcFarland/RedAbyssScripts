using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownBottleScript : MonoBehaviour
{
    public GameObject[] allO;
    bool broke;
    const int AUDIO_NUM = 1;
    AudioScript audioS;
    public FireScript[] fireS;
    int fireOffset;

    // Start is called before the first frame update
    void Start()
    {
        broke = true;
        fireOffset = 1;
        SetupAudio();
    }

    private void SetupAudio()
    {
        GameObject[] audioM = GameObject.FindGameObjectsWithTag("audiomanager");
        int searching = 0;
        while (searching != -1)
        {
            if (audioM[searching].GetComponent<AudioScript>().GetAudioNum() == AUDIO_NUM)
            {
                audioS = audioM[searching].GetComponent<AudioScript>();
                searching = -1;
            }
            else
                searching++;
        }
    }

    public void Broken()
    {
        if (broke == false)
        {
            broke = true;
            audioS.PlayGlassBreak();
            fireOffset = 1;
            foreach (FireScript f in fireS)
            {
                f.Enflame(new Vector3(allO[0].transform.position.x + GetFireOffsetX(fireOffset) + Random.Range(-1f, 1f), allO[0].transform.position.y, allO[0].transform.position.z + GetFireOffsetZ(fireOffset) + Random.Range(-1f, 1f)));
                fireOffset += 1;
            }
            foreach (GameObject o in allO)
            {
                o.transform.position = new Vector3(300f, 300f, 300f);
                o.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            }
        }
    }

    public void Throw(Vector3 pos, float rotation, float force)
    {
        foreach (GameObject o in allO)
        {
            o.transform.position = new Vector3(pos.x, pos.y + 2f, pos.z);
            o.transform.rotation = Quaternion.Euler(90, rotation, 0);
            o.GetComponent<Rigidbody>().velocity = (o.transform.right * force) + new Vector3 (0f, 1f*(force/10f), 0f);
        }
        broke = false;
    }

    private float GetFireOffsetX(int fn) // starts in middle, then goes out directly right and travels around counterclockwise
    {
        if (fn == 1)
            return 0;
        else if (fn == 2)
            return 1f;
        else if (fn == 3)
            return 1f;
        else if (fn == 4)
            return 0f;
        else if (fn == 5)
            return -1f;
        else if (fn == 6)
            return -1f;
        else if (fn == 7)
            return -1f;
        else if (fn == 8)
            return 0;
        else if (fn == 9)
            return 1f;
        else
            return 100f; //returns if error
    }

    private float GetFireOffsetZ(int fn) // starts in middle, then goes out directly right and travels around counterclockwise
    {
        if (fn == 1)
            return 0;
        else if (fn == 2)
            return 0f;
        else if (fn == 3)
            return 1f;
        else if (fn == 4)
            return 1f;
        else if (fn == 5)
            return 1f;
        else if (fn == 6)
            return 0f;
        else if (fn == 7)
            return -1f;
        else if (fn == 8)
            return -1f;
        else if (fn == 9)
            return -1f;
        else
            return 100f; //returns if error
    }

}
