using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.TestScenes;
using UnityEngine;

public class PuertaPersonal : MonoBehaviour
{
    public PolygonCollider2D       collider;
    public DialoguePrompterManager PrompterManager;
    public GameObject              puertaCerrada, puertaAbierta;

    // Update is called once per frame
    private void Update()
    {
        bool unlocked =
            PrompterManager._gameVariables.ContainsKey("puerta_sala_maquinas_abierta") &&
            PrompterManager._gameVariables["puerta_sala_maquinas_abierta"];

        //No collision
        if (TimeBar.instance.currentTime == 100 && unlocked)
            collider.enabled = false;
        else
            collider.enabled = true;

        //Physically open
        if (TimeBar.instance.currentTime == 10 || TimeBar.instance.currentTime == 100 && unlocked)
        {
            puertaAbierta.gameObject.SetActive(true);
            puertaCerrada.gameObject.SetActive(false);
        }
        else
        {
            puertaAbierta.gameObject.SetActive(false);
            puertaCerrada.gameObject.SetActive(true);
        }
    }
}