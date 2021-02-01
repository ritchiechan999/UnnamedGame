using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class assInteractableObjects : MonoBehaviour
{
    public Team TeamToInteract = Team.Unassigned;
    private GameObject openedUI = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if (layer.ContainsLayer(Data.EntityLayer)) {
            openedUI = UIManager.Instance.Open(this.gameObject, assGameUI.Interactable, 
                assUItoOpen.ShopTrigger);
            Debug.Log("Collided with entity");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if (layer.ContainsLayer(Data.EntityLayer)) {
            UIManager.Instance.Close(openedUI);
            Debug.Log("Entity Exitted");
        }
    }
}