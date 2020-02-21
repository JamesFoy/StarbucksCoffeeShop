using UnityEngine;
using VRTK;
using VRTK.Highlighters;

namespace VRKitchenSimulator.VRTK
{
    [RequireComponent(typeof(VRTK_SnapDropZone))]
    public class SpawnSnapDropZone : MonoBehaviour
    {
        VRTK_SnapDropZone snapDrop;
#pragma warning disable 649
        [SerializeField] GameObject highlightObject;
#pragma warning restore 649
        VRTK_BaseHighlighter objectHighlighter;
        bool willSnap;

        void Awake()
        {
            snapDrop = GetComponent<VRTK_SnapDropZone>();
            var existingHighlighter = VRTK_BaseHighlighter.GetActiveHighlighter(gameObject);
            //If no highlighter is found on the GameObject then create the default one
            if (existingHighlighter == null)
            {
                highlightObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
            }
            else
            {
                VRTK_SharedMethods.CloneComponent(existingHighlighter, highlightObject);
            }

            //Initialise highlighter and set highlight colour
            objectHighlighter = highlightObject.GetComponent<VRTK_BaseHighlighter>();
            objectHighlighter.unhighlightOnDisable = false;
            objectHighlighter.Initialise(snapDrop.highlightColor);
            objectHighlighter.Highlight(snapDrop.highlightColor);

            //if the object highlighter is using a cloned object then disable the created highlight object's renderers
            if (objectHighlighter.UsesClonedObject())
            {
                var renderers = GetComponentsInChildren<Renderer>(true);
                for (var i = 0; i < renderers.Length; i++)
                {
                    if (!VRTK_PlayerObject.IsPlayerObject(renderers[i].gameObject, VRTK_PlayerObject.ObjectTypes.Highlighter))
                    {
                        renderers[i].enabled = false;
                    }
                }
            }
        }

        void OnEnable()
        {
            snapDrop.ObjectEnteredSnapDropZone += OnWillSnap;
            snapDrop.ObjectExitedSnapDropZone += OnWillNotSnap;
        }

        void OnDisable()
        {
            snapDrop.ObjectEnteredSnapDropZone -= OnWillSnap;
            snapDrop.ObjectExitedSnapDropZone -= OnWillNotSnap;
        }

        void OnWillNotSnap(object sender, SnapDropZoneEventArgs e)
        {
            Debug.Log("Will Snap");
            willSnap = false;
            ToggleHighlightColor();
        }

        void OnWillSnap(object sender, SnapDropZoneEventArgs e)
        {
            Debug.Log("Will Snap");
            willSnap = true;
            ToggleHighlightColor();
        }

        protected virtual void ToggleHighlightColor()
        {
            if (Application.isPlaying && snapDrop.highlightAlwaysActive && (objectHighlighter != null))
            {
                var color = willSnap && (snapDrop.validHighlightColor != Color.clear) ? snapDrop.validHighlightColor : snapDrop.highlightColor;
                objectHighlighter.Highlight(color);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            if (objectHighlighter != null)
            {
                objectHighlighter.Unhighlight();
            }
        }
    }
}