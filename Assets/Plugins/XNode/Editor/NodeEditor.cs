using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace XNodeEditor
{
  /// <summary> Base class to derive custom Node editors from. Use this to create your own custom inspectors and editors for your nodes. </summary>
  [CustomNodeEditor(typeof(XNode.Node))]
  public class NodeEditor : XNodeEditor.Internal.NodeEditorBase<NodeEditor, NodeEditor.CustomNodeEditorAttribute, XNode.Node>
  {
    public enum RenamingState
    {
      None, AwaitRenaming, RenamingActive
    }
    /// <summary> Fires every whenever a node was modified through the editor </summary>
    public static Action<XNode.Node> onUpdateNode;
    public static Dictionary<XNode.NodePort, Vector2> portPositions;
    RenamingState renaming;

    public virtual void OnHeaderGUI()
    {
      string title = target.name;
      if (renaming != RenamingState.None && Selection.Contains(target))
      {
        int controlID = EditorGUIUtility.GetControlID(FocusType.Keyboard) + 1;
        if (renaming == RenamingState.AwaitRenaming)
        {
          EditorGUIUtility.keyboardControl = controlID;
          EditorGUIUtility.editingTextField = true;
          renaming = RenamingState.RenamingActive;
        }

        target.name = EditorGUILayout.TextField(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        if (!EditorGUIUtility.editingTextField)
        {
          Rename(target.name);
          renaming = RenamingState.None;
        }
      }
      else
      {
        GUILayout.Label(title, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        if (HasFocus && IsRenameKeyPressed())
        {
          InitiateRename();
        }
      }
    }

    bool IsRenameKeyPressed()
    {
      return (Event.current != null && Event.current.isKey && Event.current.keyCode == KeyCode.F2) ;
    }

    bool HasFocus
    {
      get
      {
        return Selection.activeObject == this.target;
      }
    }

    /// <summary> Draws standard field editors for all public fields </summary>
    public virtual void OnBodyGUI()
    {
      // Unity specifically requires this to save/update any serial object.
      // serializedObject.Update(); must go at the start of an inspector gui, and
      // serializedObject.ApplyModifiedProperties(); goes at the end.
      serializedObject.Update();
      string[] excludes = {"m_Script", "graph", "position", "ports"};
      portPositions = new Dictionary<XNode.NodePort, Vector2>();

      SerializedProperty iterator = serializedObject.GetIterator();
      bool enterChildren = true;
      EditorGUIUtility.labelWidth = 84;
      while (iterator.NextVisible(enterChildren))
      {
        enterChildren = false;
        if (excludes.Contains(iterator.name)) continue;
        NodeEditorGUILayout.PropertyField(iterator, true);
      }

      serializedObject.ApplyModifiedProperties();
    }

    public virtual int GetWidth()
    {
      Type type = target.GetType();
      int width;
      if (NodeEditorWindow.nodeWidth.TryGetValue(type, out width)) return width;
      else return 208;
    }

    public virtual Color GetTint()
    {
      Type type = target.GetType();
      Color color;
      if (NodeEditorWindow.nodeTint.TryGetValue(type, out color)) return color;
      else return Color.white;
    }

    public virtual GUIStyle GetBodyStyle()
    {
      return NodeEditorResources.styles.nodeBody;
    }

    public void InitiateRename()
    {
      renaming = RenamingState.AwaitRenaming;
    }

    public void Rename(string newName)
    {
      target.name = newName;
      AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CustomNodeEditorAttribute : Attribute,
                                             XNodeEditor.Internal.NodeEditorBase<NodeEditor, NodeEditor.CustomNodeEditorAttribute, XNode.Node>.INodeEditorAttrib
    {
      private Type inspectedType;

      /// <summary> Tells a NodeEditor which Node type it is an editor for </summary>
      /// <param name="inspectedType">Type that this editor can edit</param>
      public CustomNodeEditorAttribute(Type inspectedType)
      {
        this.inspectedType = inspectedType;
      }

      public Type GetInspectedType()
      {
        return inspectedType;
      }
    }
  }
}
