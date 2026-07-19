using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationAnimation : MonoBehaviour
{
    
    [SerializeField]
    private Animator Anim;
    private int speaktype;

    public void AnalyzeResponse(string response) 
    { 
        //analyze agent response to generate corresponding animation
    }

    public void SetSpeakingAnimation(int type)
    {
        speaktype = type;
    }

    public void StartAnimation()
    {
        Anim.SetInteger("SpeakType", speaktype);
    }

    public void EndAnimation()
    {
        Anim.SetInteger("SpeakType", 0);
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
                
    }
}
