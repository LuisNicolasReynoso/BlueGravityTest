using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
public class TextPopUp : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    TextMeshProUGUI Text;

    Transform pos;

    void Start()
    {
        Destroy(this.gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        //Transform world position to UI position
        Vector2 PosFix = Camera.main.WorldToScreenPoint(pos.position);
        float offsetY = +15;

        PosFix = new Vector2(PosFix.x, PosFix.y + offsetY);


        this.transform.position = PosFix;
    }

    public void PopText (string text, Color color, Transform position)
    {
        Text.text = text; ;
        Text.color = color;
        pos = position;


    }
}
