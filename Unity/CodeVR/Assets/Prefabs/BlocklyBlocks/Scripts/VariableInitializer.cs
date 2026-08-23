using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableInitializer : MonoBehaviour
{
    [SerializeField] private string _varName;
    [SerializeField] private VariableDropdownHandler _variableDropdownHandler;

    private void Awake()
    {
        var manager = FindObjectOfType<VariableDeclarationManager>();
        if (manager == null)
            return;
        
        _varName = _varName.Trim();
        if (_varName == string.Empty)
            return;

        var variable = manager.Variables.Find(v => v.Name == _varName);

        if (variable == null)
            variable = manager.AddVariable(_varName);

        _variableDropdownHandler.SetStartValueByVariableID(variable.ID);
    }
}
