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
                    // merge数据导入.
                    if (file.EndsWith("_back.dbak"))
                    {
                        try
                        {
                            Debug.Log(string.Format("OnPostprocessAllAssets {0}", file));
                            string fileName = Path.GetFileName(file);
                            string assetFile = string.Format("{0}/{1}.Asset", Directory.GetParent(file).Parent.FullName, fileName.Substring(0, fileName.IndexOf("_back.dbak")));
                            Debug.Log("asset" + assetFile);
                            if (File.Exists(assetFile))
                            {
                                GKToyExternalData assetData = AssetDatabase.LoadAssetAtPath<GKToyExternalData>(assetFile.Substring(assetFile.IndexOf("Assets")));
                                GKToyData tData = JsonConvert.DeserializeObject<GKToyData>(File.ReadAllText(file).Replace("\t\n", "\\n"));
                                tData.Init(null);
                                assetData.data = tData;
                                Debug.Log(string.Format("Load changed backup file: {0}", file));
                            }
                            else
                            {
                                Debug.Log(string.Format("Fail to load changed backup file: {0}", file));
                            }
                        }
                        catch
                        {
                            Debug.Log(string.Format("Fail to load changed backup file: {0}", file));
                        }
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
                        }
                        continue;
                    }
                    // 删除数据Prefab文件时，删除备份文件.
                    if (file.EndsWith(".prefab"))
                    {
                        string backFile = string.Format("{0}/OverlordBackup/overlord_{1}_back.dbak", Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                        if (File.Exists(backFile))
                        {
                            Debug.Log(string.Format("Delete back file {0}", backFile));
                            AssetDatabase.DeleteAsset(backFile);
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




