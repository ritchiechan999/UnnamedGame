using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class assButton : MonoBehaviour
{
    [Header("Options")]
    public bool OpenToWorldSpace = false;
    public bool CloseUI = false;
    public assGameUI UIToShow = assGameUI.Unassigned;
    private Button button;
    public Transform parent;

    private void Awake()
    {
        if (parent == null)
            parent = this.gameObject.transform;
        button = this.GetComponent<Button>();
        button.onClick.AddListener(() => { ButtonInteract(); UIManager.Instance.Close(parent.gameObject); });
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(() => { ButtonInteract(); UIManager.Instance.Close(parent.gameObject); });
    }

    public void UIToOpenIsNull(assUItoOpen type)
    {
        switch (type) {
            case assUItoOpen.Unassigned:
                break;
            case assUItoOpen.ShopTrigger:
                UIToShow = assGameUI.ShopTrigger;
                break;
            case assUItoOpen.ShopPanel:
                UIToShow = assGameUI.ShopPanel;
                break;
        }
    }

    private void ButtonInteract()
    {
        if (CloseUI)
            return;

        if (OpenToWorldSpace) {
            UIManager.Instance.Open(parent.gameObject, UIToShow);
        } else {
            UIManager.Instance.Open(UIToShow);
        }
    }
}
