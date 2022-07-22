using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualcue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset InkJSON;

    private bool playerInRange;

    private void Awake()
    {
        playerInRange = false;
        visualcue.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && !DialogManager.GetInstance().dialogPlaying)
        {
            visualcue.SetActive(true);
            if(InputManager.GetInstance().GetInteractPressed())
            {
                DialogManager.GetInstance().EnterDialogueMode(InkJSON);
            }
        }
        else
        {
            visualcue.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "player")
        {
            playerInRange=true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        playerInRange = false;

    }
}
