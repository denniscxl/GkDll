using UnityEngine;
using UnityEditor;
using GKBase;
using System.IO;
using Newtonsoft.Json;

namespace GKToy
{
    public class GKToyAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
        {
            var _filename = string.Empty;
            try
            {
                foreach (var file in importedAssets)
                {
                    _filename = file;

                    if (file.StartsWith("Assets/Utilities/GKToy/CSV/_AutoGen/GKToy_"))
                    {
                        Debug.Log(string.Format("OnPostprocessAllAssets {0}", file));
                        GKToyDataImport.OnImportData(file);
                        continue;
                    }
                    // ...
                }
                foreach (var file in deletedAssets)
                {
                    // 删除数据源Asset文件时，删除备份文件.
                    if (file.EndsWith(".Asset"))
                    {
                        string backFile = string.Format("{0}/OverlordBackup/{1}_back.dbak", Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                        if (File.Exists(backFile))
                        {
                            Debug.Log(string.Format("Delete back file {0}", backFile));
                            AssetDatabase.DeleteAsset(backFile);
                            AssetDatabase.Refresh();
                        }
                        continue;
                    }
                    // 删除数据Prefab文件时，删除备份文件.
                    else if (file.EndsWith(".prefab"))
                    {
                        string backFile = string.Format("{0}/OverlordBackup/overlord_{1}_back.dbak", Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                        if (File.Exists(backFile))
                        {
                            Debug.Log(string.Format("Delete back file {0}", backFile));
                            AssetDatabase.DeleteAsset(backFile);
                            AssetDatabase.Refresh();
                        }
                        continue;
                    }
                }
                for(int i =0; i< movedFromPath.Length; ++i)
                {
                    // 重命名数据源Asset文件时，重命名备份文件.
                    if (movedFromPath[i].EndsWith(".Asset"))
                    {
                        string backFile = string.Format("{0}/OverlordBackup/{1}_back.dbak", Path.GetDirectoryName(movedFromPath[i]), Path.GetFileNameWithoutExtension(movedFromPath[i]));
                        if (File.Exists(backFile))
                        {
                            Debug.Log(string.Format("Rename back file {0}", backFile));
                            AssetDatabase.RenameAsset(backFile, string.Format("{0}_back.dbak", Path.GetFileNameWithoutExtension(movedAssets[i])));
                            AssetDatabase.Refresh();
                        }
                        continue;
                    }
                    // 重命名数据Prefab文件时，重命名备份文件.
                    else if (movedFromPath[i].EndsWith(".prefab"))
                    {
                        string backFile = string.Format("{0}/OverlordBackup/overlord_{1}_back.dbak", Path.GetDirectoryName(movedFromPath[i]), Path.GetFileNameWithoutExtension(movedFromPath[i]));
                        if (File.Exists(backFile))
                        {
                            Debug.Log(string.Format("Rename back file {0}", backFile));
                            AssetDatabase.RenameAsset(backFile, string.Format("overlord_{0}_back.dbak", Path.GetFileNameWithoutExtension(movedAssets[i])));
                            AssetDatabase.Refresh();
                        }
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




