using Assets.Scripts.TestScenes;
using System.Collections;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [SerializeField] public DialoguePrompterManager PROMPTER;
    [SerializeField] public int dialogueID;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LateStart());
        
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        //Your Function You Want to Call
        PROMPTER.PlayDialogue(dialogueID);
    }
}
