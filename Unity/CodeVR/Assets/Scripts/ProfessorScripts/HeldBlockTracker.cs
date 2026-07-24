using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HeldCodeBlockTracker : MonoBehaviour
{
    [SerializeField] private XRBaseInteractor leftInteractor;
    [SerializeField] private XRBaseInteractor rightInteractor;

    private CodeBlock leftHandBlock;
    private CodeBlock rightHandBlock;

    private void OnEnable()
    {
        leftInteractor.selectEntered.AddListener(OnLeftGrab);
        leftInteractor.selectExited.AddListener(OnLeftRelease);

        rightInteractor.selectEntered.AddListener(OnRightGrab);
        rightInteractor.selectExited.AddListener(OnRightRelease);
    }

    private void OnDisable()
    {
        leftInteractor.selectEntered.RemoveListener(OnLeftGrab);
        leftInteractor.selectExited.RemoveListener(OnLeftRelease);

        rightInteractor.selectEntered.RemoveListener(OnRightGrab);
        rightInteractor.selectExited.RemoveListener(OnRightRelease);
    }

    private void OnLeftGrab(SelectEnterEventArgs args)
    {
        CodeBlock block =
            args.interactableObject.transform.GetComponentInParent<CodeBlock>();

        if (block != null)
        {
            leftHandBlock = block;
            Debug.Log($"Left hand holding: {block.BlocklyTypeString}");
        }
    }

    private void OnLeftRelease(SelectExitEventArgs args)
    {
        CodeBlock block =
            args.interactableObject.transform.GetComponentInParent<CodeBlock>();

        if (block != null && leftHandBlock == block)
        {
            leftHandBlock = null;
        }
    }

    private void OnRightGrab(SelectEnterEventArgs args)
    {
        CodeBlock block =
            args.interactableObject.transform.GetComponentInChildren<CodeBlock>();

        if (block != null)
        {
            rightHandBlock = block;
            Debug.Log($"Right hand holding: {block.BlocklyTypeString}");
        }
    }

    private void OnRightRelease(SelectExitEventArgs args)
    {
        CodeBlock block =
            args.interactableObject.transform.GetComponentInChildren<CodeBlock>();

        if (block != null && rightHandBlock == block)
        {
            rightHandBlock = null;
        }
    }

    public string GetHeldBlocksDescription()
    {
        bool hasLeft = leftHandBlock != null;
        bool hasRight = rightHandBlock != null;

        if (!hasLeft && !hasRight)
        {
            return "The student is not currently holding a block.";
        }

        if (hasLeft && hasRight)
        {
            return
                $"Left hand: {leftHandBlock.BlocklyTypeString}\n" +
                $"Right hand: {rightHandBlock.BlocklyTypeString}";
        }

        if (hasLeft)
        {
            return $"Left hand: {leftHandBlock.BlocklyTypeString}";
        }

        return $"Right hand: {rightHandBlock.BlocklyTypeString}";
    }

    public CodeBlock GetLeftHandBlock()
    {
        return leftHandBlock;
    }

    public CodeBlock GetRightHandBlock()
    {
        return rightHandBlock;
    }
}