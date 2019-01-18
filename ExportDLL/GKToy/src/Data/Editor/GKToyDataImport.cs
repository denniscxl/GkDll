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

            if (basename == "GKToy_CameraTypeData.csv")
            {
                var cameraData = LoadOrCreateCameraData();
                EditorUtility.SetDirty(cameraData);
                _OnImportCameraTypeData(filename, cameraData);
                return;
            }

            if (basename == "GKToy_ActionTypeData.csv")
            {
                var actionData = LoadOrCreateActionData();
                EditorUtility.SetDirty(actionData);
                _OnImportActionTypeData(filename, actionData);
                return;
            }

            if (basename == "GKToy_SoundTypeData.csv")
            {
                var soundData = LoadOrCreateSoundData();
                EditorUtility.SetDirty(soundData);
                _OnImportSoundTypeData(filename, soundData);
                return;
            }

            if (basename == "GKToy_ConditionTypeData.csv")
            {
                var condData = LoadOrCreateConditionData();
                EditorUtility.SetDirty(condData);
                _OnImportConditionTypeData(filename, condData);
                return;
            }


        }

        #region Localization
        static public GKToyLocalizationData LoadOrCreateLocalizationData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyLocalizationData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyLocalizationData.asset");
        }

        static void _OnImportLocalizationData(string filename, GKToyLocalizationData data)
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
        #endregion

        #region Camera
        static public GKToyCameraTypeData LoadOrCreateCameraData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyCameraTypeData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyCameraTypeData.asset");
        }

        static void _OnImportCameraTypeData(string filename, GKToyCameraTypeData data)
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
            data.ResetTypeDataTypeArray(row);

            while (p.NextRow())
            {
                if (p.IsRowStartWith("#")) continue;

                var d = new GKToyCameraTypeData.CameraTypeData();
                p.RowToObject<GKToyCameraTypeData.CameraTypeData>(ref d);

                if (null == d || d.id < 0 || d.id >= data._typeData.Length)
                    continue;

                data._typeData[d.id] = d;
            }
        }
        #endregion

        #region Sound
        static public GKToySoundTypeData LoadOrCreateSoundData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToySoundTypeData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToySoundTypeData.asset");
        }

        static void _OnImportSoundTypeData(string filename, GKToySoundTypeData data)
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
            data.ResetTypeDataTypeArray(row);

            while (p.NextRow())
            {
                if (p.IsRowStartWith("#")) continue;

                var d = new GKToySoundTypeData.SoundTypeData();
                p.RowToObject<GKToySoundTypeData.SoundTypeData>(ref d);

                if (null == d || d.id < 0 || d.id >= data._typeData.Length)
                    continue;

                data._typeData[d.id] = d;
            }
        }
        #endregion

        #region ActionType
        static public GKToyActionTypeData LoadOrCreateActionData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyActionTypeData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyActionTypeData.asset");
        }

        static void _OnImportActionTypeData(string filename, GKToyActionTypeData data)
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
            data.ResetActionTypeDataTypeArray(row);

            while (p.NextRow())
            {
                if (p.IsRowStartWith("#")) continue;

                var d = new GKToyActionTypeData.ActionTypeData();
                p.RowToObject<GKToyActionTypeData.ActionTypeData>(ref d);

                if (null == d || d.id < 0 || d.id >= data._actionTypeData.Length)
                    continue;

                data._actionTypeData[d.id] = d;
            }
        }
        #endregion

        #region ConditionType
        static public GKToyConditionTypeData LoadOrCreateConditionData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyConditionTypeData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyConditionTypeData.asset");
        }

        static void _OnImportConditionTypeData(string filename, GKToyConditionTypeData data)
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
            data.ResetConditionTypeDataTypeArray(row);

            while (p.NextRow())
            {
                if (p.IsRowStartWith("#")) continue;

                var d = new GKToyConditionTypeData.ConditionTypeData();
                p.RowToObject<GKToyConditionTypeData.ConditionTypeData>(ref d);

                if (null == d || d.id < 0 || d.id >= data._conditionTypeData.Length)
                    continue;

                data._conditionTypeData[d.id] = d;
            }
        }
        #endregion
    }
}
