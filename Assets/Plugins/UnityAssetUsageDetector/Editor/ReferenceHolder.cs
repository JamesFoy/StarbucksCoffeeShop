using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetUsageDetectorNamespace
{
    [Serializable]
    public class ReferenceHolder : ISerializationCallbackReceiver
    {
        // Custom struct to hold a single path to a reference
        public struct ReferencePath
        {
            public readonly ReferenceNode startNode;
            public readonly int[] pathLinksToFollow;

            public ReferencePath( ReferenceNode startNode, int[] pathIndices )
            {
                this.startNode = startNode;
                pathLinksToFollow = pathIndices;
            }
        }

        // Credit: https://docs.unity3d.com/Manual/script-Serialization-Custom.html
        [Serializable]
        public class SerializableNode
        {
            public int instanceId;
            public bool isUnityObject;
            public string description;

            public List<int> links;
            public List<string> linkDescriptions;
        }
		
        private string title;
        private bool clickable;
        private List<ReferenceNode> references;
        private List<ReferencePath> referencePathsShortUnique;
        private List<ReferencePath> referencePathsShortest;

        private List<SerializableNode> serializedNodes;
        private List<int> initialSerializedNodes;

        public int NumberOfReferences { get { return references.Count; } }

        public ReferenceHolder( string title, bool clickable )
        {
            this.title = title;
            this.clickable = clickable;
            references = new List<ReferenceNode>();
            referencePathsShortUnique = null;
            referencePathsShortest = null;
        }

        // Add a reference to the list
        public void AddReference( ReferenceNode node )
        {
            references.Add( node );
        }

        // Initializes commonly used variables of the nodes
        public void InitializeNodes()
        {
            for( int i = 0; i < references.Count; i++ )
                references[i].InitializeRecursively();
        }

        // Check if node exists in this results set
        public bool Contains( ReferenceNode node )
        {
            for( int i = 0; i < references.Count; i++ )
            {
                if( references[i] == node )
                    return true;
            }

            return false;
        }

        // Add all the Object's in this container to the set
        public void AddObjectsTo( HashSet<Object> objectsSet )
        {
            CalculateShortestPathsToReferences();

            for( int i = 0; i < referencePathsShortUnique.Count; i++ )
            {
                Object obj = referencePathsShortUnique[i].startNode.UnityObject;
                if( obj != null && !obj.Equals( null ) )
                    objectsSet.Add( obj );
            }
        }

        // Add all the GameObject's in this container to the set
        public void AddGameObjectsTo( HashSet<GameObject> gameObjectsSet )
        {
            CalculateShortestPathsToReferences();

            for( int i = 0; i < referencePathsShortUnique.Count; i++ )
            {
                Object obj = referencePathsShortUnique[i].startNode.UnityObject;
                if( obj != null && !obj.Equals( null ) )
                {
                    if( obj is GameObject )
                        gameObjectsSet.Add( (GameObject) obj );
                    else if( obj is Component )
                        gameObjectsSet.Add( ( (Component) obj ).gameObject );
                }
            }
        }

        // Calculate short unique paths to the references
        public void CalculateShortestPathsToReferences()
        {
            if( referencePathsShortUnique != null )
                return;

            referencePathsShortUnique = new List<ReferencePath>( 32 );
            for( int i = 0; i < references.Count; i++ )
                references[i].CalculateShortUniquePaths( referencePathsShortUnique );

            referencePathsShortest = new List<ReferencePath>( referencePathsShortUnique.Count );
            for( int i = 0; i < referencePathsShortUnique.Count; i++ )
            {
                int[] linksToFollow = referencePathsShortUnique[i].pathLinksToFollow;

                // Find the last two nodes in this path
                ReferenceNode nodeBeforeLast = referencePathsShortUnique[i].startNode;
                for( int j = 0; j < linksToFollow.Length - 1; j++ )
                    nodeBeforeLast = nodeBeforeLast[linksToFollow[j]].targetNode;

                // Check if these two nodes are unique
                bool isUnique = true;
                for( int j = 0; j < referencePathsShortest.Count; j++ )
                {
                    ReferencePath path = referencePathsShortest[j];
                    if( path.startNode == nodeBeforeLast && path.pathLinksToFollow[0] == linksToFollow[linksToFollow.Length - 1] )
                    {
                        isUnique = false;
                        break;
                    }
                }

                if( isUnique )
                    referencePathsShortest.Add( new ReferencePath( nodeBeforeLast, new int[1] { linksToFollow[linksToFollow.Length - 1] } ) );
            }
        }

        // Draw the results found for this container
        public void DrawOnGUI( PathDrawingMode pathDrawingMode )
        {
            Color c = GUI.color;
            GUI.color = Color.cyan;

            if( GUILayout.Button( title, AssetUsageDetector.BoxGUIStyle, AssetUsageDetector.GL_EXPAND_WIDTH, AssetUsageDetector.GL_HEIGHT_40 ) && clickable )
            {
                // If the container (scene, usually) is clicked, highlight it on Project view
                AssetDatabase.LoadAssetAtPath<SceneAsset>( title ).SelectInEditor();
            }

            GUI.color = Color.yellow;

            if( pathDrawingMode == PathDrawingMode.Full )
            {
                for( int i = 0; i < references.Count; i++ )
                {
                    GUILayout.Space( 5 );

                    references[i].DrawOnGUIRecursively();
                }
            }
            else
            {
                if( referencePathsShortUnique == null )
                    CalculateShortestPathsToReferences();

                List<ReferencePath> pathsToDraw;
                if( pathDrawingMode == PathDrawingMode.ShortRelevantParts )
                    pathsToDraw = referencePathsShortUnique;
                else
                    pathsToDraw = referencePathsShortest;

                for( int i = 0; i < pathsToDraw.Count; i++ )
                {
                    GUILayout.Space( 5 );

                    GUILayout.BeginHorizontal();

                    ReferencePath path = pathsToDraw[i];
                    path.startNode.DrawOnGUI( null );

                    ReferenceNode currentNode = path.startNode;
                    for( int j = 0; j < path.pathLinksToFollow.Length; j++ )
                    {
                        ReferenceNode.Link link = currentNode[path.pathLinksToFollow[j]];
                        link.targetNode.DrawOnGUI( link.description );
                        currentNode = link.targetNode;
                    }

                    GUILayout.EndHorizontal();
                }
            }

            GUI.color = c;

            GUILayout.Space( 10 );
        }

        // Assembly reloading; serialize nodes in a way that Unity can serialize
        public void OnBeforeSerialize()
        {
            if( references == null )
                return;

            if( serializedNodes == null )
                serializedNodes = new List<SerializableNode>( references.Count * 5 );
            else
                serializedNodes.Clear();

            if( initialSerializedNodes == null )
                initialSerializedNodes = new List<int>( references.Count );
            else
                initialSerializedNodes.Clear();

            Dictionary<ReferenceNode, int> nodeToIndex = new Dictionary<ReferenceNode, int>( references.Count * 5 );
            for( int i = 0; i < references.Count; i++ )
                initialSerializedNodes.Add( references[i].SerializeRecursively( nodeToIndex, serializedNodes ) );
        }

        // Assembly reloaded; deserialize nodes to construct the original graph
        public void OnAfterDeserialize()
        {
            if( initialSerializedNodes == null || serializedNodes == null )
                return;

            if( references == null )
                references = new List<ReferenceNode>( initialSerializedNodes.Count );
            else
                references.Clear();

            List<ReferenceNode> allNodes = new List<ReferenceNode>( serializedNodes.Count );
            for( int i = 0; i < serializedNodes.Count; i++ )
                allNodes.Add( new ReferenceNode() );

            for( int i = 0; i < serializedNodes.Count; i++ )
                allNodes[i].Deserialize( serializedNodes[i], allNodes );

            for( int i = 0; i < initialSerializedNodes.Count; i++ )
                references.Add( allNodes[initialSerializedNodes[i]] );

            referencePathsShortUnique = null;
            referencePathsShortest = null;

            serializedNodes.Clear();
            initialSerializedNodes.Clear();
        }
    }
}