using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using CrazyMinnow.SALSA;

public class SalsaFeedExternalAnalysis : MonoBehaviour
{
    public bool isFeeding;
    //private Salsa salsa;

    private void Start()
    {
        //salsa = GetComponent<Salsa>();
    }
    // Update is called once per frame
    void Update()
    {
        if (isFeeding)
        {
            //salsa.analysisValue = Random.Range(0.0f, 0.3f);
        }
        else
        {
            //salsa.analysisValue = 0f;
        }
    }
}
