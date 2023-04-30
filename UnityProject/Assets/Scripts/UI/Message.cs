using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


public class Message : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Text;

    Transform DesiredPosition;

    float duration = 3f;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine("Remove");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 PosFix = Camera.main.WorldToScreenPoint(DesiredPosition.position);
        float offsetY = -15;

        PosFix = new Vector2(PosFix.x, PosFix.y + offsetY);

        
        this.transform.position = PosFix;
    }

    public void SetMessage(string message, Transform position)
    {
        Text.text = message;
        DesiredPosition = position;


    }

    public IEnumerator Remove()
    {
        yield return new WaitForSeconds(duration);
        anim.SetTrigger("Remove");
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
