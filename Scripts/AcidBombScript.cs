using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidBombScript : MonoBehaviour
{
    bool broke;
    Rigidbody rb;
    const int AUDIO_NUM = 1;
    AudioScript audioS;
    int acidOffset;
    public AcidScript[] acidS;
    int iteratedAcid;

    // Start is called before the first frame update
    void Start()
    {
        iteratedAcid = 0;
        broke = false;
        rb = GetComponent<Rigidbody>();
        SetupAudio();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            acidOffset = 1;
            for (int i = 0; i < 9; i++)
            {
                acidS[i + iteratedAcid * 9].Spill(new Vector3(transform.position.x + GetAcidOffsetX(acidOffset) + Random.Range(-1f, 1f), .001f, transform.position.z + GetAcidOffsetZ(acidOffset) + Random.Range(-1f, 1f)));
                acidOffset += 1;
            }
                transform.position = new Vector3(300f, 300f, 300f);
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            
            iteratedAcid++;
            if (iteratedAcid >= acidS.Length / 9)
                iteratedAcid = 0;
        }
    }

    public void Throw(Vector3 pos, float rotation, float force)
    {
        transform.rotation = Quaternion.Euler(90, -rotation, 0);
        transform.position = new Vector3(pos.x, pos.y + 2f, pos.z) + transform.right;
        rb.velocity = (transform.right * force) + new Vector3(0f, force/10f, 0f);
        broke = false;
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "floor" || coll.gameObject.tag == "wall")
        {
            Broken();
        }
    }

    private float GetAcidOffsetX(int fn) // starts in middle, then goes out directly right and travels around counterclockwise
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

    private float GetAcidOffsetZ(int fn) // starts in middle, then goes out directly right and travels around counterclockwise
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
