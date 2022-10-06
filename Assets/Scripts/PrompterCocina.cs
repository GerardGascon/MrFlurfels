using System;
using Assets.Scripts.TestScenes;
using Cinemachine;
using DG.Tweening;
using SimpleTools.AudioManager;
using UnityEngine;

public class PrompterCocina : MonoBehaviour
{
    public GameObject cortinaFull,
        cortinaApartada,
        tapaAlcantarillaAbierta,
        tapaAlcantarillaCerrada;

    public PlayerController player;
    public HelperController helper;

    private bool                    canPlayMetal = true;
    private bool                    contacting   = false;
    public  DialoguePrompterManager _dialoguePrompterManager;
    private bool                    done = false;
    public  CinemachineBrain        cmb;

    // Start is called before the first frame update
    private void Start()
    {
        cortinaApartada.SetActive(false);
        cortinaFull.SetActive(true);
        tapaAlcantarillaAbierta.SetActive(false);
        tapaAlcantarillaCerrada.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == player.gameObject && canPlayMetal && TimeBar.instance.currentTime == 100)
        {
            AudioManager.instance.Play("SFX_metal");
            canPlayMetal = false;
            contacting   = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == player.gameObject)
        {
            canPlayMetal = true;
            contacting   = false;
        }
    }

    private void Update()
    {
        //Interaction
        if (Input.GetButtonDown("Submit") && contacting && !Notepad.instance.opened && !done && TimeBar.instance.currentTime == 100)
        {
            _dialoguePrompterManager.OnFinish += SecondStep;
            _dialoguePrompterManager.PlayDialogue(61);
            done = true;
            player.moveBlockers.Add(this);
            TimeBar.instance.slider.enabled = false;
        }
    }

    private void SecondStep()
    {
        DOTween.Sequence().AppendInterval(1f).AppendCallback(() =>
            {
                cortinaApartada.SetActive(true);
                cortinaFull.SetActive(false);
                tapaAlcantarillaAbierta.SetActive(true);
                //tapaAlcantarillaCerrada.SetActive(false);
            })
            .AppendInterval(3f)
            .AppendCallback(() =>
            {
                _dialoguePrompterManager.OnFinish -= SecondStep;
                _dialoguePrompterManager.OnFinish += ThirdStep;
                _dialoguePrompterManager.PlayDialogue(62);
            });
    }

    private void ThirdStep()
    {
        player.transform.position = new Vector3(-15.42f, 21.25f, 0);
        helper.transform.position = new Vector3(-14.92f, 21.25f, 0);

        float cameraZ = Camera.main.transform.position.z;
        cmb.enabled = false;
        float speed = 0.5f;

        DOTween.Sequence()
            .Append(Camera.main.transform.DOMove(new Vector3(0.3f, 33.7f, cameraZ), speed)
                .SetSpeedBased(true))
            .Append(Camera.main.transform.DOMove(new Vector3(-9.88f, 33.7f, cameraZ), speed)
                .SetSpeedBased(true))
            .Append(Camera.main.transform.DOMove(new Vector3(-9.88f, 31.69f, cameraZ), speed)
                .SetSpeedBased(true))
            .Append(Camera.main.transform.DOMove(new Vector3(-14.6f, 31.69f, cameraZ), speed)
                .SetSpeedBased(true))
            .Append(Camera.main.transform.DOMove(new Vector3(-14.6f, 34.48f, cameraZ), speed)
                .SetSpeedBased(true))
            .Append(Camera.main.transform.DOMove(new Vector3(-12.1f, 34.48f, cameraZ), speed)
                .SetSpeedBased(true))
            .Append(Camera.main.transform.DOMove(new Vector3(-12.1f, 29.2f, cameraZ), speed)
                .SetSpeedBased(true))
            .Append(Camera.main.transform.DOMove(new Vector3(-16.1f, 29.2f, cameraZ), speed)
                .SetSpeedBased(true))
            .Append(Camera.main.transform.DOMove(new Vector3(-16.1f, 31.9f, cameraZ), speed)
                .SetSpeedBased(true))
            .Append(Camera.main.transform.DOMove(new Vector3(-17.5f, 31.9f, cameraZ), speed)
                .SetSpeedBased(true))
            .Append(Camera.main.transform.DOMove(new Vector3(-17.5f, 25.9f, cameraZ), speed)
                .SetSpeedBased(true)).AppendCallback(() =>
            {
                _dialoguePrompterManager.OnFinish -= ThirdStep;
                _dialoguePrompterManager.PlayDialogue(63);
                cmb.enabled = true;
                player.moveBlockers.Remove(this);
                TimeBar.instance.slider.enabled = true;
                Destroy(this.gameObject);
            });
    }
}