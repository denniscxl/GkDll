using GKBase;
using GKToy;
using UnityEditor;

namespace GKToyDialogue
{
    public class GKToyDialogueDataImport
    {
        static public void OnImportData(string filename)
        {
            var basename = System.IO.Path.GetFileName(filename);

            if (basename == "GKToyDialogue_LocalizationData.csv")
            {
                var locaData = LoadOrCreateLocalizationData();
                EditorUtility.SetDirty(locaData);
                GKToyDataImport._OnImportLocalizationData(filename, locaData);
                return;
            }
            if (basename == "GKToyDialogue_CameraTypeData.csv")
            {
                var cameraData = LoadOrCreateCameraData();
                EditorUtility.SetDirty(cameraData);
                _OnImportCameraTypeData(filename, cameraData);
                return;
            }

            if (basename == "GKToyDialogue_ActionTypeData.csv")
            {
                var actionData = LoadOrCreateActionData();
                EditorUtility.SetDirty(actionData);
                _OnImportActionTypeData(filename, actionData);
                return;
            }

            if (basename == "GKToyDialogue_SoundTypeData.csv")
            {
                var soundData = LoadOrCreateSoundData();
                EditorUtility.SetDirty(soundData);
                _OnImportSoundTypeData(filename, soundData);
                return;
            }

            if (basename == "GKToyDialogue_ConditionTypeData.csv")
            {
                var condData = LoadOrCreateConditionData();
                EditorUtility.SetDirty(condData);
                _OnImportConditionTypeData(filename, condData);
                return;
            }

            if (basename == "GKToyDialogue_ConditionOutputTypeData.csv")
            {
                var condOutputData = LoadOrCreateConditionOutputData();
                EditorUtility.SetDirty(condOutputData);
                _OnImportConditionOutputTypeData(filename, condOutputData);
                return;
            }
        }

        #region Localization
        static public GKToyLocalizationData LoadOrCreateLocalizationData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyLocalizationData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyDialogueLocalizationData.asset");
        }
        #endregion

        #region Camera
        static public GKToyDialogueCameraTypeData LoadOrCreateCameraData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyDialogueCameraTypeData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyDialogueCameraTypeData.asset");
        }

        static void _OnImportCameraTypeData(string filename, GKToyDialogueCameraTypeData data)
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

                var d = new GKToyDialogueCameraTypeData.CameraTypeData();
                p.RowToObject<GKToyDialogueCameraTypeData.CameraTypeData>(ref d);

                if (null == d || d.id < 0 || d.id >= data._typeData.Length)
                    continue;

                data._typeData[d.id] = d;
            }
        }
        #endregion

        #region Sound
        static public GKToyDialogueSoundTypeData LoadOrCreateSoundData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyDialogueSoundTypeData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyDialogueSoundTypeData.asset");
        }

        static void _OnImportSoundTypeData(string filename, GKToyDialogueSoundTypeData data)
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

                var d = new GKToyDialogueSoundTypeData.SoundTypeData();
                p.RowToObject<GKToyDialogueSoundTypeData.SoundTypeData>(ref d);

                if (null == d || d.id < 0 || d.id >= data._typeData.Length)
                    continue;

                data._typeData[d.id] = d;
            }
        }
        #endregion

        #region ActionType
        static public GKToyDialogueActionTypeData LoadOrCreateActionData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyDialogueActionTypeData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyDialogueActionTypeData.asset");
        }

        static void _OnImportActionTypeData(string filename, GKToyDialogueActionTypeData data)
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

                var d = new GKToyDialogueActionTypeData.ActionTypeData();
                p.RowToObject<GKToyDialogueActionTypeData.ActionTypeData>(ref d);

                if (null == d || d.id < 0 || d.id >= data._actionTypeData.Length)
                    continue;

                data._actionTypeData[d.id] = d;
            }
        }
        #endregion

        #region ConditionType
        static public GKToyDialogueConditionTypeData LoadOrCreateConditionData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyDialogueConditionTypeData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyDialogueConditionTypeData.asset");
        }

        static void _OnImportConditionTypeData(string filename, GKToyDialogueConditionTypeData data)
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

                var d = new GKToyDialogueConditionTypeData.ConditionTypeData();
                p.RowToObject<GKToyDialogueConditionTypeData.ConditionTypeData>(ref d);

                if (null == d || d.id < 0 || d.id >= data._conditionTypeData.Length)
                    continue;

                data._conditionTypeData[d.id] = d;
            }
        }
        #endregion

        #region ConditionOutputType
        static public GKToyDialogueConditionOutputTypeData LoadOrCreateConditionOutputData()
        {
            return GKEditor.LoadOrCreateAssetByNonResource<GKToyDialogueConditionOutputTypeData>("Assets/Utilities/GKToy/CSV/_AutoGen/GKToyDialogueConditionOutputTypeData.asset");
        }

        static void _OnImportConditionOutputTypeData(string filename, GKToyDialogueConditionOutputTypeData data)
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
            data.ReseDataTypeArray(row);

            while (p.NextRow())
            {
                if (p.IsRowStartWith("#")) continue;

                var d = new GKToyDialogueConditionOutputTypeData.ConditionOutputTypeData();
                p.RowToObject<GKToyDialogueConditionOutputTypeData.ConditionOutputTypeData>(ref d);

                if (null == d || d.id < 0 || d.id >= data._typeData.Length)
                    continue;

                data._typeData[d.id] = d;
            }
        }
        #endregion
    }
}
