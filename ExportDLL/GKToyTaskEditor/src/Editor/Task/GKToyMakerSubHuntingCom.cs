using GKBase;
using GKToy;
using UnityEngine;

namespace GKToyTaskEditor
{
    public class GKToyMakerSubHuntingCom : GKToyMakerSubTaskCom
    {
        #region PublicField
        public static GKToyMakerSubHuntingCom instance;
        #endregion
        protected GKToySubTaskHunting _huntTask = null;
        // 初始化任务数据(不包含核心任务数据).
        public static void InitSubData(GKToySubTaskHunting task, GKToyData data)
        {
            instance._task = task;
            instance._huntTask = task;
            instance._data = data;
            instance.tmpTaskId = task.TargetID.Value;
        }
        public static void PopupTaskWindow()
        {
            instance = GetWindow<GKToyMakerSubHuntingCom>(GKToyTaskMaker._GetTaskLocalization("Hunt Task Config"), true);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
            instance.minSize = new Vector2(500, 250);
            instance.maxSize = new Vector2(500, 250);
        }

        void OnEnable()
        {
            if (null == instance)
            {
                instance = GetWindow<GKToyMakerSubHuntingCom>(GKToyTaskMaker._GetTaskLocalization("Hunt Task Config"), true);
                wantsMouseMove = true;
                minSize = new Vector2(500, 250);
                maxSize = new Vector2(500, 250);
            }
        }

        protected override void DrawSubUI()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Hunt Npc") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _huntTask.HuntNpc.Value, (obj) => { _huntTask.HuntNpc.SetValue(obj); });
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Hunt Count") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _huntTask.HuntCount.Value, (obj) => { _huntTask.HuntCount.SetValue(obj); });
            }
            GUILayout.EndHorizontal();
        }

        void OnDestroy()
        {
            instance = null;
        }
    }
}
