using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Plugins.XNode.Editor
{
  public static class NodeEditorMenuItems
  {
    [MenuItem("Tools/XNode/Node Editor", isValidateFunction: true)]
    public static bool ShouldEditGraph()
    {
      var graph = Selection.activeObject as NodeGraph;
      return graph != null;
    }

    [MenuItem("Tools/XNode/Node Editor", isValidateFunction: false)]
    public static void EditGraph()
    {
      var graph = Selection.activeObject as NodeGraph;
      if (graph == null)
      {
        return;
      }

      NodeEditorWindow w = EditorWindow.GetWindow(typeof(NodeEditorWindow), false, "xNode", true) as NodeEditorWindow;
      w.wantsMouseMove = true;
      w.graph = graph;
      w.Show();
    }

    [MenuItem("Tools/XNode/Clean Graph", isValidateFunction: true)]
    public static bool ShouldCleanGraph()
    {
      var graph = Selection.activeObject as NodeGraph;
      return graph != null;
    }

    [MenuItem("Tools/XNode/Clean Graph", isValidateFunction: false)]
    public static void CleanGraph()
    {
      var graph = Selection.activeObject as NodeGraph;
      if (graph == null)
      {
        Debug.Log("Not a graph");
        return;
      }

      var assetPath = AssetDatabase.GetAssetPath(graph);
      foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(assetPath))
      {
        if (asset == graph)
        {
          continue;
        }

        if (asset == null)
        {
          Debug.Log("NullAsset at ");
          continue;
        }

        Debug.Log("Inspecting asset " + asset + " of type " + asset.GetType());

        if (asset is Node)
        {
          var node = (Node) asset;
          if (!graph.nodes.Contains(node))
          {
            Debug.LogWarning("Destroyed asset " + node);
            Object.DestroyImmediate(node, true);
          }
        }
        else
        {
            Debug.LogWarning("Destroyed foreign asset " + asset);
            Object.DestroyImmediate(asset, true);

        }
      }


    }
  }
}
