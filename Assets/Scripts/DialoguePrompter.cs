using System;
using System.Linq;
using UnityEngine;

public class DialoguePrompter : MonoBehaviour
{
    [SerializeField] private KeyFramedDialogues[] _dialogues;

    public int GetDialogue()
    {
        int currentFrame = TimeBar.instance.currentTime;

        KeyFramedDialogues[] sortedDialogues = _dialogues.OrderBy(a_dialogue => a_dialogue.frame).ToArray();

        for (int i = 0; i < sortedDialogues.Length; i++)
        {
            KeyFramedDialogues dialogue = sortedDialogues[i];

            //Frame exacto deseado
            if (dialogue.frame == currentFrame)
            {
                return dialogue.dialogueID;
            }

            //Frame inferior al deseado
            if (dialogue.frame < currentFrame)
            {
                if (i != sortedDialogues.Length - 1) continue;
                //No hay más keyframes hacia adelante
                return dialogue.dialogueID;
            }

            //Frame superior al deseado
            if (i == 0)
            {
                Debug.LogError("No hay ningun dialogo");
                return 0;
            }

            return sortedDialogues[i - 1].dialogueID;
        }

        return sortedDialogues.Last().dialogueID;
    }
}

[Serializable]
public struct KeyFramedDialogues
{
    public int frame;
    public int dialogueID;
}