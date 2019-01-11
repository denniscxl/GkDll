using UnityEditor;
using GKBase;

/// <summary>
/// 神经元导出工具.
/// 1. ToyMaker组件.
/// 2. ToyMaker-Lua支持组件.
/// 3. All.
/// </summary>
namespace GKToy
{
    public class GKToyMakerExport
    {
        [MenuItem("GK/ToyMaker/Export/Base", false, GKEditorConfiger.MenuItemPriorityA)]
        static void _ExportPackageBase()
        {
            string[] path =
            {
                "Assets/Utilities/Examples/Resources/Prefabs",
                "Assets/Utilities/Examples/Resources/UI",
                "Assets/Utilities/Examples/Scenes",
                "Assets/Utilities/Examples/Scripts",
                "Assets/Utilities/GameKit",
                "Assets/Utilities/GKToy/_Font",
                "Assets/Utilities/GKToy/CSV",
                "Assets/Utilities/GKToy/Resources",
                "Assets/Utilities/GKToy/Scripts/Core",
                "Assets/Utilities/GKToy/Scripts/Data",
                "Assets/Utilities/GKToy/Scripts/Editor",
                "Assets/Utilities/GKToy/Scripts/FSM",
                "Assets/Utilities/GKToy/Scripts/Nodes/Actions/Base",
                "Assets/Utilities/GKToy/Scripts/Nodes/Actions/Color",
                "Assets/Utilities/GKToy/Scripts/Nodes/Actions/Input",
                "Assets/Utilities/GKToy/Scripts/Nodes/Actions/Math",
                "Assets/Utilities/GKToy/Scripts/Nodes/Actions/Physics",
                "Assets/Utilities/GKToy/Scripts/Nodes/Actions/Time",
                "Assets/Utilities/GKToy/Scripts/Nodes/Actions/Transform",
                "Assets/Utilities/GKToy/Scripts/Nodes/Conditions",
                "Assets/Utilities/GKToy/Scripts/Nodes/Core",
                "Assets/Utilities/GKToy/Scripts/Nodes/Decorations",
                "Assets/Utilities/GKToy/Scripts/Setting",
                "Assets/Utilities/GKToy/Scripts/Variable",
                "Assets/Utilities/GKToy/Scripts/Nodes/Actions/Base",
                "Assets/Utilities/GKToy/Textures"
            };
            _Export(path);
        }

        [MenuItem("GK/ToyMaker/Export/Lua", false, GKEditorConfiger.MenuItemPriorityA)]
        static void _ExportPackageLua()
        {
            string[] path =
            {
                "Assets/Utilities/Examples/Resources/Lua",
                "Assets/Utilities/GKToy/Scripts/Nodes/Actions/Lua",
                "Assets/Utilities/XLua"
            };
            _Export(path);
        }

        [MenuItem("GK/ToyMaker/Export/All", false, GKEditorConfiger.MenuItemPriorityA)]
        static void _ExportPackageAll()
        {
            string[] path =
            {
                "Assets/Utilities"
            };
            _Export(path);
        }

        static void _Export(string [] path)
        {
            var destPath = EditorUtility.SaveFilePanel("Save path", "", "", "unitypackage");
            if (destPath == "")
                return;
            var assetPathNames = AssetDatabase.GetDependencies(path);
            AssetDatabase.ExportPackage(assetPathNames, destPath, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
        }
    }
}


