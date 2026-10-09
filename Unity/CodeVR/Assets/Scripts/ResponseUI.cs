using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _responseContent;
    
    public void setResponseText(string text)
    {
        _responseContent.text = text;
    }
}
