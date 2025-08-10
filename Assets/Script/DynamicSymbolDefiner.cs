using UnityEngine;
using UnityEditor;

public class DynamicSymbolDefiner
{
#if UNITY_EDITOR
    [MenuItem("Debug/Define DEV Symbol")]
    static void DefineDEVSymbol()
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        const string devSymbol = "DEV";
        if (!defines.Contains(devSymbol))
        {
            if (string.IsNullOrEmpty(defines))
            {
                defines = devSymbol;
            }
            else
            {
                defines += $";{devSymbol}";
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Debug/Remove DEV Symbol")]
    static void RemoveDEVSymbol()
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        const string devSymbol = "DEV";
        if (defines.Contains(devSymbol))
        {
            defines = defines.Replace($";{devSymbol}", "");
            defines = defines.Replace(devSymbol, "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
            AssetDatabase.Refresh();
        }
    }
#endif
}