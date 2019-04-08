using GKBase;
using GKToy;
using GKToyTaskDialogue;
using UnityEngine;

namespace GKToyTaskEditor
{
    public class GKToyMakerSubInteractCom : GKToyMakerSubTaskCom
    {
        #region PublicField
        public static GKToyMakerSubInteractCom instance;
        #endregion
        protected GKToySubTaskInteract _interactTask = null;
        // 初始化任务数据(不包含核心任务数据).
        public static void InitSubData(GKToySubTaskInteract task, GKToyData data)
        {
            instance._task = task;
            instance._interactTask = task;
            instance._data = data;
            instance.tmpTaskId = task.TargetID.Value;
        }
        public static void PopupTaskWindow()
        {
            instance = GetWindow<GKToyMakerSubInteractCom>(GKToyTaskMaker._GetTaskLocalization("Interact Task Config"), true);
            _styleCenrer.alignment = TextAnchor.MiddleCenter;
            _styleRight.alignment = TextAnchor.MiddleRight;
            instance.minSize = new Vector2(500, 250);
            instance.maxSize = new Vector2(500, 250);
        }

        void OnEnable()
        {
            if (null == instance)
            {
                instance = GetWindow<GKToyMakerSubInteractCom>(GKToyTaskMaker._GetTaskLocalization("Interact Task Config"), true);
                wantsMouseMove = true;
                minSize = new Vector2(500, 250);
                maxSize = new Vector2(500, 250);
            }
        }

        protected override void DrawSubUI()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Interact Npc") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _interactTask.InteractNpc.Value, (obj) => { _interactTask.InteractNpc.SetValue(obj); });
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Interact Type") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _interactTask.InteractType.Value, (obj) => { _interactTask.InteractType.SetValue(obj); });
                GUILayout.Label("1对话 2读条 3踩到", GUILayout.Width(NOTE_WIDTH));
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Interact Dfg") + ": ", GUILayout.Width(LABEL_WIDTH));
                if (GUILayout.Button(_interactTask.InteractDfg.Value))
                {
                    GKToyDialogueMaker.Instance.ShowDialogue(_interactTask.InteractDfgObject.Value.GetComponent<GKToyBaseOverlord>());
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Interact Time") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _interactTask.InteractTime.Value, (obj) => { _interactTask.InteractTime.SetValue(obj); });
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GKToyTaskMaker._GetTaskLocalization("Interact Item") + ": ", GUILayout.Width(LABEL_WIDTH));
                GKEditor.DrawBaseControl(true, _interactTask.InteractItem.Value, (obj) => { _interactTask.InteractItem.SetValue(obj); });
            }
            GUILayout.EndHorizontal();
        }

        void OnDestroy()
        {
            instance = null;
        }
    }
}
