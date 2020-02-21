using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetUsageDetectorNamespace
{
    public class ReferenceNode
    {
        public struct Link
        {
            public readonly ReferenceNode targetNode;
            public readonly string description;

            public Link( ReferenceNode targetNode, string description )
            {
                this.targetNode = targetNode;
                this.description = description;
            }
        }

        // Unique identifier is used while serializing the node
        private static int uid_last = 0;
        private readonly int uid;

        public object nodeObject;
        private readonly List<Link> links;

        private int? instanceId; // instanceId of the nodeObject if it is a Unity object, null otherwise
        private string description; // String to print on this node
		
        public Object UnityObject { get { return instanceId.HasValue ? EditorUtility.InstanceIDToObject( instanceId.Value ) : null; } }

        public int NumberOfOutgoingLinks { get { return links.Count; } }
        public Link this[int index] { get { return links[index]; } }

        public ReferenceNode()
        {
            links = new List<Link>( 2 );
            uid = uid_last++;
        }

        public ReferenceNode( object obj ) : this()
        {
            nodeObject = obj;
        }

        // Add a one-way connection to another node
        public void AddLinkTo( ReferenceNode nextNode, string description = null )
        {
            if( nextNode != null )
            {
                if( !string.IsNullOrEmpty( description ) )
                    description = "[" + description + "]";

                links.Add( new Link( nextNode, description ) );
            }
        }

        // Initialize node's commonly used variables
        public void InitializeRecursively()
        {
            if( description != null ) // Already initialized
                return;

            Object unityObject = nodeObject as Object;
            if( unityObject != null )
            {
                instanceId = unityObject.GetInstanceID();
                description = unityObject.name + " (" + unityObject.GetType() + ")";
            }
            else if( nodeObject != null )
            {
                instanceId = null;
                description = nodeObject.GetType() + " object";
            }
            else
            {
                instanceId = null;
                description = "<<destroyed>>";
            }

            nodeObject = null; // don't hold Object reference, allow Unity to GC used memory

            for( int i = 0; i < links.Count; i++ )
                links[i].targetNode.InitializeRecursively();
        }

        // Clear this node so that it can be reused later
        public void Clear()
        {
            nodeObject = null;
            links.Clear();
        }

        // Calculate short unique paths that start with this node
        public void CalculateShortUniquePaths( List<ReferenceHolder.ReferencePath> currentPaths )
        {
            CalculateShortUniquePaths( currentPaths, new List<ReferenceNode>( 8 ), new List<int>( 8 ) { -1 }, 0 );
        }

        // Just some boring calculations to find the short unique paths recursively
        private void CalculateShortUniquePaths( List<ReferenceHolder.ReferencePath> shortestPaths, List<ReferenceNode> currentPath, List<int> currentPathIndices, int latestObjectIndexInPath )
        {
            int currentIndex = currentPath.Count;
            currentPath.Add( this );

            if( links.Count == 0 )
            {
                // Check if the path to the reference is unique (not discovered so far)
                bool isUnique = true;
                for( int i = 0; i < shortestPaths.Count; i++ )
                {
                    if( shortestPaths[i].startNode == currentPath[latestObjectIndexInPath] && shortestPaths[i].pathLinksToFollow.Length == currentPathIndices.Count - latestObjectIndexInPath - 1 )
                    {
                        int j = latestObjectIndexInPath + 1;
                        for( int k = 0; j < currentPathIndices.Count; j++, k++ )
                        {
                            if( shortestPaths[i].pathLinksToFollow[k] != currentPathIndices[j] )
                                break;
                        }

                        if( j == currentPathIndices.Count )
                        {
                            isUnique = false;
                            break;
                        }
                    }
                }

                // Don't allow duplicate short paths
                if( isUnique )
                {
                    int[] pathIndices = new int[currentPathIndices.Count - latestObjectIndexInPath - 1];
                    for( int i = latestObjectIndexInPath + 1, j = 0; i < currentPathIndices.Count; i++, j++ )
                        pathIndices[j] = currentPathIndices[i];

                    shortestPaths.Add( new ReferenceHolder.ReferencePath( currentPath[latestObjectIndexInPath], pathIndices ) );
                }
            }
            else
            {
                if( instanceId.HasValue ) // nodeObject is Unity object
                    latestObjectIndexInPath = currentIndex;

                for( int i = 0; i < links.Count; i++ )
                {
                    currentPathIndices.Add( i );
                    links[i].targetNode.CalculateShortUniquePaths( shortestPaths, currentPath, currentPathIndices, latestObjectIndexInPath );
                    currentPathIndices.RemoveAt( currentIndex + 1 );
                }
            }

            currentPath.RemoveAt( currentIndex );
        }

        // Draw all the paths that start with this node on GUI recursively
        public void DrawOnGUIRecursively( string linkToPrevNodeDescription = null )
        {
            GUILayout.BeginHorizontal();

            DrawOnGUI( linkToPrevNodeDescription );

            if( links.Count > 0 )
            {
                GUILayout.BeginVertical();

                for( int i = 0; i < links.Count; i++ )
                {
                    ReferenceNode next = links[i].targetNode;
                    next.DrawOnGUIRecursively( links[i].description );
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
        }

        // Draw only this node on GUI
        public void DrawOnGUI( string linkToPrevNodeDescription )
        {
            string label = GetNodeContent( linkToPrevNodeDescription );
            if( GUILayout.Button( label, AssetUsageDetector.BoxGUIStyle, AssetUsageDetector.GL_EXPAND_HEIGHT ) )
            {
                // If a reference is clicked, highlight it (either on Hierarchy view or Project view)
                UnityObject.SelectInEditor();
            }

            if( AssetUsageDetector.showTooltips && Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains( Event.current.mousePosition ) )
                AssetUsageDetector.tooltip = label;
        }

        // Get the string representation of this node
        private string GetNodeContent( string linkToPrevNodeDescription = null )
        {
            if( !string.IsNullOrEmpty( linkToPrevNodeDescription ) )
                return linkToPrevNodeDescription + "\n" + description;

            return description;
        }

        // Serialize this node and its connected nodes recursively
        public int SerializeRecursively( Dictionary<ReferenceNode, int> nodeToIndex, List<ReferenceHolder.SerializableNode> serializedNodes )
        {
            int index;
            if( nodeToIndex.TryGetValue( this, out index ) )
                return index;

            ReferenceHolder.SerializableNode serializedNode = new ReferenceHolder.SerializableNode()
            {
                instanceId = instanceId ?? 0,
                isUnityObject = instanceId.HasValue,
                description = description
            };

            index = serializedNodes.Count;
            nodeToIndex[this] = index;
            serializedNodes.Add( serializedNode );

            if( links.Count > 0 )
            {
                serializedNode.links = new List<int>( links.Count );
                serializedNode.linkDescriptions = new List<string>( links.Count );

                for( int i = 0; i < links.Count; i++ )
                {
                    serializedNode.links.Add( links[i].targetNode.SerializeRecursively( nodeToIndex, serializedNodes ) );
                    serializedNode.linkDescriptions.Add( links[i].description );
                }
            }

            return index;
        }

        // Deserialize this node and its links from the serialized data
        public void Deserialize( ReferenceHolder.SerializableNode serializedNode, List<ReferenceNode> allNodes )
        {
            if( serializedNode.isUnityObject )
                instanceId = serializedNode.instanceId;
            else
                instanceId = null;

            description = serializedNode.description;

            if( serializedNode.links != null )
            {
                for( int i = 0; i < serializedNode.links.Count; i++ )
                    links.Add( new Link( allNodes[serializedNode.links[i]], serializedNode.linkDescriptions[i] ) );
            }
        }

        public override int GetHashCode()
        {
            return uid;
        }
    }
}