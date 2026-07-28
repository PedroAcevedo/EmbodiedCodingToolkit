using UnityEngine;

public class ProfessorLookAt : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerHead;

    [Range(0f, 1f)]
    [SerializeField] private float lookWeight = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float bodyWeight = 0.1f;

    [Range(0f, 1f)]
    [SerializeField] private float headWeight = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float clampWeight = 0.5f;

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetLookAtWeight(
            lookWeight,
            bodyWeight,
            headWeight,
            0f,             
            clampWeight
        );


        Vector3 lookTarget = playerHead.position + Vector3.down * 0.1f;

        animator.SetLookAtPosition(lookTarget);
    }
}