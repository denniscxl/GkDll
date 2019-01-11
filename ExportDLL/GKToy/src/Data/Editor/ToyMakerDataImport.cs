using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GKBase;

public class ToyMakerDataImport
{
    static public ToyMakerData LoadOrCreateGameData()
    {
        return GKEditor.LoadOrCreateAssetByNonResource<ToyMakerData>("Assets/Utilities/GKToy/CSV/_AutoGen/ToyMakerData.asset");
    }

    static public void OnImportData(string filename)
    {
        var data = LoadOrCreateGameData();
        EditorUtility.SetDirty(data);

        var basename = System.IO.Path.GetFileName(filename);

        if (basename == "ToyMakerData_LocalizationData.csv") { _OnImportLocalizationData(filename, data); return; }
    }

    static void _OnImportLocalizationData(string filename, ToyMakerData data)
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

            var d = new ToyMakerData.LocalizationData();
            p.RowToObject<ToyMakerData.LocalizationData>(ref d);

            if (null == d || d.id < 0 || d.id >= data._localizationData.Length)
                continue;

            data._localizationData[d.id] = d;
        }
    }
}
