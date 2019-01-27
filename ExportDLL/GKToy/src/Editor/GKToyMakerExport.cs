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
        [MenuItem("GK/ToyMaker/Export/Lua", false, GKEditorConfiger.MenuItemPriorityA)]
        static void _ExportPackageLua()
        {
            string[] path =
            {
                "Assets/Utilities/XLua"
            };
            _Export(path);
        }

        [MenuItem("GK/ToyMaker/Export/GKToy", false, GKEditorConfiger.MenuItemPriorityA)]
        static void _ExportPackageGKToy()
        {
            string[] path =
            {
                "Assets/Utilities/GKToy",
                "Assets/Utilities/Plugins",
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


