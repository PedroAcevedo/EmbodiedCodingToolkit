using System;
using UnityEngine;

public class OpenaiAPI : MonoBehaviour
{
    public string API_key;
    public string API_org;

    public void Start()
    {
        //API_key = Environment.GetEnvironmentVariable("OPENAI_KEY");
    }
}
