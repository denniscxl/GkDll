﻿using UnityEngine;
using UnityEditor;
using GKBase;

public class ToyMakerAssetPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
    {
        var _filename = string.Empty;
        try
        {
            foreach (var file in importedAssets)
            {
                _filename = file;
                
                if (file.StartsWith("Assets/Utilities/GKToy/CSV/_AutoGen/ToyMakerData_"))
                {
                    Debug.Log(string.Format("OnPostprocessAllAssets {0}", file));
                    ToyMakerDataImport.OnImportData(file);
                    continue;
                }

                // ...
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("OnPostprocessAllAssets Exception: " + _filename + "\n" + e);
        }
    }
}



