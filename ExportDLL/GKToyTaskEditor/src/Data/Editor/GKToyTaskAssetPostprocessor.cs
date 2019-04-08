using UnityEngine;
using UnityEditor;

namespace GKToy
{
    public class GKToyTaskAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
        {
            var _filename = string.Empty;
            try
            {
                foreach (var file in importedAssets)
                {
                    _filename = file;

                    if (file.StartsWith("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyTask_"))
                    {
                        Debug.Log(string.Format("OnPostprocessAllAssets {0}", file));
                        GKToyTaskDataImport.OnImportData(file);
                        continue;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("OnPostprocessAllAssets Exception: " + _filename + "\n" + e);
            }
        }
    }
}




