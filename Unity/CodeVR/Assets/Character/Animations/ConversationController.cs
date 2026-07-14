using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private int speakType;
    
    
    void Update()
    {
        animator.SetInteger("SpeakType", speakType);
    }
}
