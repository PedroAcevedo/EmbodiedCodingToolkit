using System.Collections.Generic;
using UnityEngine;

public class DynamicListInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CodeBlock _codeBlock;
    [SerializeField] private ExpandableBlock _expandableBlock;
    [SerializeField] private CodeBlockConnector _inputConnectorPrefab;
    [SerializeField] private Transform _connectorParent;

    [Header("Connector Positioning")]
    [SerializeField] private Vector3 _firstConnectorPosition;
    [SerializeField] private float _connectorSpacing = 1.0f;

    [Header("Sizing")]
    [Tooltip("How much additional ExpandScale Y each connector after the first requires.")]
    [SerializeField] private float _heightPerAdditionalConnector = 1.0f;

    private readonly List<CodeBlockConnector> _itemConnectors =
        new List<CodeBlockConnector>();

    private float _baseExpandHeight;

    public int ItemCount => _itemConnectors.Count;

    private void Awake()
    {
        // Remember the height you designed in the prefab.
        _baseExpandHeight = _expandableBlock.ExpandScale.y;
    }

    public void AddItem()
    {
        int index = _itemConnectors.Count;

        CodeBlockConnector connector = Instantiate(
            _inputConnectorPrefab,
            _connectorParent
        );

        connector.BlockAttachedTo = _codeBlock;

        connector.UpdateBlocklyConnectionSetting(
            "value",
            "ADD" + index
        );

        _itemConnectors.Add(connector);
        _codeBlock.AddConnector(connector);

        UpdateLayout();
    }

    public void RemoveItem()
    {
        if (_itemConnectors.Count == 0)
            return;

        int lastIndex = _itemConnectors.Count - 1;
        CodeBlockConnector connector = _itemConnectors[lastIndex];

        if (connector.IsConnected)
            return;

        _itemConnectors.RemoveAt(lastIndex);

        _codeBlock.RemoveConnector(connector);

        Destroy(connector.gameObject);

        UpdateLayout();
    }

    private void UpdateLayout()
    {
        PositionConnectors();
        UpdateHeight();

        // Re-align anything already connected to this block.
        _codeBlock.RealignBlockCluster();
    }

    private void PositionConnectors()
    {
        for (int i = 0; i < _itemConnectors.Count; i++)
        {
            _itemConnectors[i].transform.localPosition =
                _firstConnectorPosition +
                Vector3.down * (_connectorSpacing * i);
        }
    }

    private void UpdateHeight()
    {
        // The base block already has enough room for ADD0.
        int additionalRows =
            Mathf.Max(0, _itemConnectors.Count - 1);

        Vector3 scale = _expandableBlock.ExpandScale;

        scale.y =
            _baseExpandHeight +
            additionalRows * _heightPerAdditionalConnector;

        _expandableBlock.ExpandScale = scale;

        _expandableBlock.ApplyScaleToExpandableSettings();
    }
}