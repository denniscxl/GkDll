using GKBase;
using GKToy;
using UnityEditor;

namespace GKToy
{
    public class GKToyTaskDataImport
    {
        static public void OnImportData(string filename)
        {
            var basename = System.IO.Path.GetFileName(filename);

            if (basename == "GKToyTask_LocalizationData.csv")
            {
                var locaData = LoadOrCreateLocalizationData();
                EditorUtility.SetDirty(locaData);
                GKToyDataImport._OnImportLocalizationData(filename, locaData);
                return;
            }
        }

        static public GKToyLocalizationData LoadOrCreateLocalizationData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyLocalizationData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyTaskLocalizationData.asset");
        }
    }
}
