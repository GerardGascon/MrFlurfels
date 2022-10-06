using System;
using Assets.Scripts.System;
using SimpleTools.AudioManager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    public static Action OnChange;

    [SerializeField] public  Slider           slider;
    private                  WaypointEntity[] entities;
    [SerializeField] private TMP_Text         indexText;
    [SerializeField] private RectTransform    positionCursor;
    [SerializeField] private Animator         anim;
    [SerializeField] private Volume           volume;
    [SerializeField] private Volume           pastVolume;
    [SerializeField] private RectTransform    timeClock;
    [SerializeField] private Image            handleImage;

    private Animator canvasAnim;
    private bool     dialogueOn;

    [HideInInspector] public int currentTime;

    private float volumeChangeVelocity1,
        volumeChangeVelocity2,
        timeClockChangeVelocity;

    public static TimeBar instance;

    private void Awake()
    {
        instance = this;

        entities                  = FindObjectsOfType<WaypointEntity>();
        positionCursor.localScale = Vector3.one / 2f * (1f + slider.value * .75f);
        currentTime               = 100;
        indexText.text = currentTime.ToString();
        canvasAnim     = GetComponent<Animator>();
    }

    public void Start()
    {
        ChangeSliderValue(99);
        ChangeSliderValue(100);
    }

    private void Update()
    {
        if (currentTime < 100f)
        {
            volume.weight = Mathf.SmoothDamp(volume.weight, 1 - currentTime / 100f,
                ref volumeChangeVelocity1, .1f);
            pastVolume.weight = Mathf.SmoothDamp(pastVolume.weight, currentTime / 100f,
                ref volumeChangeVelocity2, .1f);
            timeClock.rotation = Quaternion.Euler(0, 0,
                Mathf.SmoothDampAngle(timeClock.eulerAngles.z, 180, ref timeClockChangeVelocity,
                    .1f));
        }
        else
        {
            volume.weight = Mathf.SmoothDamp(volume.weight, 0f, ref volumeChangeVelocity1, .1f);
            pastVolume.weight =
                Mathf.SmoothDamp(pastVolume.weight, 0f, ref volumeChangeVelocity2, .1f);
            timeClock.localRotation = Quaternion.Euler(0, 0,
                Mathf.SmoothDampAngle(timeClock.eulerAngles.z, 0, ref timeClockChangeVelocity,
                    .1f));
        }

        if (PotatoDialogueManager.Instance.Talking || Time.timeScale < .5f ||
            Notepad.instance.opened)
        {
            if (dialogueOn)
            {
                canvasAnim.SetTrigger("Out");
                dialogueOn = false;
            }
        }
        else if (!dialogueOn)
        {
            canvasAnim.SetTrigger("In");
            dialogueOn = true;
        }
    }

    public void ChangeSliderValue(float value)
    {
        int newValue = Mathf.RoundToInt(value * 100);
        if (newValue != currentTime)
        {
            OnChange?.Invoke();

            if (newValue < 100)
                foreach (WaypointEntity entity in entities)
                    entity.GoToWaypoint(newValue);
            else
                foreach (WaypointEntity entity in entities)
                {
                    entity.OnPresent();
                    entity.GoToWaypoint(newValue);
                }

            if (_clicked)
            {
                if (newValue < currentTime)
                {
                    if (!AudioManager.instance.GetSource("frenar_bucle").isPlaying)
                    {
                        AudioManager.instance.FadeIn("frenar_bucle", .2f);
                        AudioManager.instance.FadeOut("acelerar_bucle", .2f);
                    }
                }
                else
                {
                    if (!AudioManager.instance.GetSource("acelerar_bucle").isPlaying)
                    {
                        AudioManager.instance.FadeIn("acelerar_bucle", .2f);
                        AudioManager.instance.FadeOut("frenar_bucle", .2f);
                    }
                }
            }

            currentTime = newValue;
        }

        indexText.text = newValue.ToString();
        slider.value   = Mathf.Round(value * 100) / 100f;

        positionCursor.localScale = Vector3.one / 2f * (1f + value * .75f);

        handleImage.rectTransform.rotation = Quaternion.Euler(0, 0, -360f / 100f * newValue);
    }

    private bool barClicked;
    private bool _clicked;

    public void Click()
    {
        _clicked = true;
        anim.SetTrigger("In");
        barClicked = true;
    }

    public void Unclick()
    {
        _clicked = false;
        anim.SetTrigger("Out");
        AudioManager.instance.FadeOut("frenar_bucle", .2f);
        AudioManager.instance.FadeOut("acelerar_bucle", .2f);
        barClicked = false;
    }
}