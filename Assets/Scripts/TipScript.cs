using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipScript : MonoBehaviour
{
    public TextMeshProUGUI Text;

    void Start()
    {
        string[] tips = { "TIP:  Open  a  farm's  log  to  see  where  it  sent or received a herd.", "TIP:  Set  up  a  quarantine  zone  around  a  farm  to  automatically  send  vets  to  surrounding  farms.", "TIP:  Use  the  Show  ID-button  to  easily identify farms." };
        string randomTip = tips[Random.Range(0, tips.Length)];
        Text.SetText(randomTip);
    }
}
