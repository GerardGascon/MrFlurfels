using Assets.Scripts.TestScenes;
using DG.Tweening;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public PlayerController        player;
    public DialoguePrompterManager _dialoguePrompterManager;
    public BoxCollider2D[] dialogues;
    public CanvasGroup continueCanvasGroup, movementCanvasGroup;

    // Start is called before the first frame update
    private void Start()
    {
        continueCanvasGroup.alpha = 0;
        movementCanvasGroup.alpha = 0;

        foreach (BoxCollider2D d in dialogues)
        {
            d.enabled = false;
        }

        player.moveBlockers.Add(this);
        TimeBar.instance.slider.enabled = false;
        DOTween.Sequence().AppendInterval(1f).AppendCallback(() =>
        {
            continueCanvasGroup.DOFade(1, 0.4f);
            _dialoguePrompterManager.PlayDialogue(44);
            _dialoguePrompterManager.OnNextText += fadeContinueCanvas;

            _dialoguePrompterManager.OnFinish += SecondStep;
        });
    }

    private void fadeContinueCanvas()
    {
        _dialoguePrompterManager.OnNextText -= fadeContinueCanvas;
        continueCanvasGroup.DOFade(0, 0.4f);
    }

    private void SecondStep()
    {
        DOTween.Sequence().AppendInterval(1.5f).AppendCallback(() =>
        {
            _dialoguePrompterManager.OnFinish -= SecondStep;
            _dialoguePrompterManager.OnFinish += ThirdStep;
            _dialoguePrompterManager.PlayDialogue(45);
        });
    }

    private void ThirdStep()
    {
        _dialoguePrompterManager.OnFinish -= ThirdStep;
        TimeBar.instance.slider.enabled   =  true;
        TimeBar.OnChange                  += FourthStep;
    }

    private void FourthStep()
    {
        TimeBar.OnChange -= FourthStep;
        _dialoguePrompterManager.OnFinish += fadeInMovementCanvas;
        _dialoguePrompterManager.PlayDialogue(46);
    }

    private void fadeInMovementCanvas()
    {
        _dialoguePrompterManager.OnFinish -= fadeInMovementCanvas;
        player.moveBlockers.Remove(this);
        player.OnMove += fadeOutMovementCanvas;
        movementCanvasGroup.DOFade(1, 0.4f);
    }

    private void fadeOutMovementCanvas(Vector2 a_arg1, Vector2 a_arg2)
    {
        player.OnMove -= fadeOutMovementCanvas;
        movementCanvasGroup.DOFade(0, 0.4f);
    }

    // Update is called once per frame
    private void Update()
    {
        bool finished =
            _dialoguePrompterManager._gameVariables.ContainsKey("tutorial_muffinRecuperado") &&
            _dialoguePrompterManager._gameVariables["tutorial_muffinRecuperado"];

        if (finished)
        {
            foreach (BoxCollider2D d in dialogues)
            {
                d.enabled = true;
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == player.gameObject)
        {
            _dialoguePrompterManager.PlayDialogue(51);
        }
    }

}