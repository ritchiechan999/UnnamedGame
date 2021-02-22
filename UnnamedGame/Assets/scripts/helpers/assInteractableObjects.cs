using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class assInteractableObjects : MonoBehaviour
{
    public assTeam TeamToInteract = assTeam.Unassigned;
    public assUItoOpen UIToOpen = assUItoOpen.Unassigned;
    private GameObject openedUI = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if (layer.ContainsLayer(assData.EntityLayer)) {
            openedUI = UIManager.Instance.Open(this.gameObject, assGameUI.Interactable, UIToOpen);
            Debug.Log("Collided with entity");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if (layer.ContainsLayer(assData.EntityLayer)) {
            UIManager.Instance.Close(openedUI);
            Debug.Log("Entity Exitted");
        }
    }
}