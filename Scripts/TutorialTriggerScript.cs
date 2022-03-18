using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerScript : MonoBehaviour
{

    DialogueScript dialogueS;
    public string textFile;
    bool triggered;

    // Start is called before the first frame update
    void Start()
    {
        triggered = false;
        dialogueS = GameObject.Find("Dialogue").GetComponent<DialogueScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && !triggered)
        {
            triggered = true;
            dialogueS.PrintDialogue(textFile);
        }
    }

}
