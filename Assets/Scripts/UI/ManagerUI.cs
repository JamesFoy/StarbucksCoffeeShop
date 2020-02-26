using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManagerUI : MonoBehaviour
{
    private static ManagerUI instance = null;

    public TMP_Text correctText;
    public TMP_Text incorrectText;

    int correctScore;
    int incorrectScore;

    public static ManagerUI Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        correctScore = 0;
        incorrectScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        correctText.text = "Correct steps: " + correctScore;
        incorrectText.text = "Incorrect steps: " + incorrectScore;
    }

    public void UpdateScore(GameEvent myEvent)
    {
        if (myEvent.isCorrect)
        {
            correctScore++;
        }
        else
        {
            incorrectScore++;
        }
    }
}
