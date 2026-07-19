using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocklyCodeManager : MonoBehaviour
{

    [SerializeField] private CodeBlockManager _blockManager;
    [SerializeField] private VariableDeclarationManager _variableDeclarationManager;

    public string GenerateBlocklyCode()
    {
        if (this._blockManager == null) return string.Empty;
        var blockClusters = new List<CodeBlock>();

        foreach (var block in this._blockManager.AllCodeBlocks)
        {
            if (block.ExcludeInAutomaticBlocklyCodeGeneration) continue;
            if (!block.IsRootBlock) continue;
            blockClusters.Add(block);
        }

        var blocklyString = BlocklyXMLGenerator.CreateXMLStringFromRootBlocks(blockClusters, this._variableDeclarationManager.Variables);
        StartCoroutine(
            WebsiteConnection.UpdateBlocklyCode(blocklyString)
        );
        return blocklyString;
    }
    
}
