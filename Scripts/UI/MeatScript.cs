using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeatScript : MonoBehaviour
{
    Image meat1;
    Image meat2;
    RectTransform ownTrans;

    // Start is called before the first frame update
    void Start()
    {
        meat1 = transform.GetChild(0).GetComponent<Image>();
        meat2 = transform.GetChild(1).GetComponent<Image>();
        ownTrans = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        meat1.rectTransform.position = new Vector2(meat1.rectTransform.position.x + 0.1f, meat1.rectTransform.position.y - 0.1f);
        meat2.rectTransform.position = new Vector2(meat2.rectTransform.position.x + 0.1f, meat2.rectTransform.position.y - 0.1f);
        if (meat1.rectTransform.position.y < ownTrans.position.y - 200f)
        {
            meat1.rectTransform.position = new Vector2(ownTrans.position.x - 200f, ownTrans.position.y + 200f);
        }
        if (meat2.rectTransform.position.y < ownTrans.position.y - 200f)
        {
            meat2.rectTransform.position = new Vector2(ownTrans.position.x - 200f, ownTrans.position.y + 200f);
        }
    }
}
