using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GKBase;

namespace GKToy
{
    public class GKToyDataImport
    {
        static public void OnImportData(string filename)
        {
            var basename = System.IO.Path.GetFileName(filename);

            if (basename == "GKToy_LocalizationData.csv")
            {
                var locaData = LoadOrCreateLocalizationData();
                EditorUtility.SetDirty(locaData);
                _OnImportLocalizationData(filename, locaData);
                return;
            }
        }

        static public GKToyLocalizationData LoadOrCreateLocalizationData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyLocalizationData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyLocalizationData.asset");
        }

        static public void _OnImportLocalizationData(string filename, GKToyLocalizationData data)
        {
            var p = GKCSVParser.OpenFile(filename, "#columns");
            if (p == null) return;

            int row = 0;

            // Calc valid lines.
            while (p.NextRow())
            {
                if (p.IsRowStartWith("#")) continue;

                row++;
            }

            // Reset readIndex to 3.
            p.ResetReadIndex();
            // Init item data array.
            data.ResetLocalizationDataTypeArray(row);

            while (p.NextRow())
            {
                if (p.IsRowStartWith("#")) continue;

                var d = new GKToyLocalizationData.LocalizationData();
                p.RowToObject<GKToyLocalizationData.LocalizationData>(ref d);

                if (null == d || d.id < 0 || d.id >= data._localizationData.Length)
                    continue;

                data._localizationData[d.id] = d;
            }
        }
    }
}
