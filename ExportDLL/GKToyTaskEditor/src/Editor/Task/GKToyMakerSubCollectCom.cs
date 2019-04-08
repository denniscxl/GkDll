using GKBase;
using GKToy;
using UnityEngine;

namespace GKToyTaskEditor
{
    public class GKToyMakerSubCollectCom : GKToyMakerSubTaskCom
    {
        #region PublicField
        public static GKToyMakerSubCollectCom instance;
        #endregion
        protected GKToySubTaskCollect _collectTask = null;
        // 初始化任务数据(不包含核心任务数据).
        public static void InitSubData(GKToySubTaskCollect task, GKToyData data)
        {
            instance._task = task;
            instance._collectTask = task;
            instance._data = data;
            instance.tmpTaskId = task.TargetID.Value;
        }
        public static void PopupTaskWindow()
        {
            instance = GetWindow<GKToyMakerSubCollectCom>(GKToyTaskMaker._GetTaskLocalization("Gather Task Config"), true);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
            instance.minSize = new Vector2(500, 250);
            instance.maxSize = new Vector2(500, 250);
        }

        void OnEnable()
        {
            if (null == instance)
            {
                instance = GetWindow<GKToyMakerSubCollectCom>(GKToyTaskMaker._GetTaskLocalization("Gather Task Config"), true);
                wantsMouseMove = true;
                minSize = new Vector2(500, 250);
                maxSize = new Vector2(500, 250);
            }
        }

        protected override void DrawSubUI()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Gather Item") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _collectTask.GatherItem.Value, (obj) => { _collectTask.GatherItem.SetValue(obj); });
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Gather Type") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _collectTask.GatherType.Value, (obj) => { _collectTask.GatherType.SetValue(obj); });
                GUILayout.Label("1读条采集 2猎杀采集", GUILayout.Width(NOTE_WIDTH));
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Gather Npc") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _collectTask.GatherNpc.Value, (obj) => { _collectTask.GatherNpc.SetValue(obj); });
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Gather Count") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _collectTask.GatherCount.Value, (obj) => { _collectTask.GatherCount.SetValue(obj); });
            }
            GUILayout.EndHorizontal();
        }

        void OnDestroy()
        {
            instance = null;
        }
    }
}
