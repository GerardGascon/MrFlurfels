using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.TestScenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnding : MonoBehaviour
{
    public DialoguePrompterManager PrompterManager;

    // Update is called once per frame
    void Update()
    {
        bool finished =
            PrompterManager._gameVariables.ContainsKey("finJuego") &&
            PrompterManager._gameVariables["finJuego"];

        if (finished && !PrompterManager.PlayingDialogue)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
