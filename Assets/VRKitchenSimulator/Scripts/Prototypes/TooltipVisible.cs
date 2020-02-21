using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RoboRyanTron.SceneReference;
using VRTK;
    
public class TooltipVisible : MonoBehaviour {

    [SerializeField]SceneReference mainscene;

	// Use this for initialization
	void Start ()
    {
        Debug.Log($"Active Scene: {SceneManager.GetActiveScene().path} and expected {mainscene.SceneName}");
        if (SceneManager.GetActiveScene().path == mainscene.SceneName)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
	}
}
