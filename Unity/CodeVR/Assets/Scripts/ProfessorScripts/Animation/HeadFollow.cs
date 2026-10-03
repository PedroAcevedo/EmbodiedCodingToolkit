using UnityEngine;

public class ProfessorLookAt : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private Transform playerHead;

    [Range(0f, 1f)]
    [SerializeField] private float lookWeight = 1f;

    [Range(0f, 90f)]
    [SerializeField] private float maxHeadAngle = 60f;

    private void LateUpdate()
    {
        Vector3 direction = playerHead.position - head.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion animatedRotation = head.rotation;

        Quaternion characterForwardRotation =
            Quaternion.LookRotation(transform.forward, transform.up);

        Quaternion lookRotation =
            Quaternion.LookRotation(direction, transform.up);

        Quaternion rotationDifference =
            lookRotation * Quaternion.Inverse(characterForwardRotation);

        Quaternion targetRotation =
            rotationDifference * animatedRotation;

        targetRotation = Quaternion.RotateTowards(
            animatedRotation,
            targetRotation,
            maxHeadAngle
        );

        head.rotation = Quaternion.Slerp(
            animatedRotation,
            targetRotation,
            lookWeight
        );
    }
}