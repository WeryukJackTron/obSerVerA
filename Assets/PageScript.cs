using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PageScript : MonoBehaviour
{
    int pageNumber = 1;
    public TextMeshProUGUI Text;

    public void nextPage()
    {
        if (pageNumber <= 1)
        {
            pageNumber++;
        }
        Text.text = pageNumber.ToString() + "/2";
    }

    public void prevPage()
    {
        if (pageNumber == 2)
        {
            pageNumber--;
        }
        Text.text = pageNumber.ToString() + "/2";
    }

}
