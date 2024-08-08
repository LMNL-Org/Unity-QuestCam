using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toast : MonoBehaviour
{
    public TMP_Text textObj;

    public void SetText(string text)
    {
        textObj.text = text;
    }
}
