using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : SimpleSingleton<UIManager>
{
    public string UIBagFolder = "ui_bags";
    Dictionary<assGameUI, UIBags> _uiBags = new Dictionary<assGameUI, UIBags>();

    public GameObject SceneCanvasPrefab;
    public GameObject WorldSpaceCanvasPrefab;
    public Canvas SceneCanvas;
    public Canvas WorldSpaceCanvas;

    protected override void Awake()
    {
        base.Awake();

        UIBags[] uis = Resources.LoadAll<UIBags>(UIBagFolder);
        foreach (UIBags ui in uis) {
            _uiBags[ui.UI] = ui;
            //Debug.Log(ui.name);
        }

        CreateCanvas();
    }

    private void CreateCanvas()
    {
        if (SceneCanvas == null && WorldSpaceCanvas == null) {
            GameObject sceneCanvasObj = Instantiate(SceneCanvasPrefab);
            SceneCanvas = sceneCanvasObj.GetComponent<Canvas>();

            GameObject worldCanvasObj = Instantiate(WorldSpaceCanvasPrefab);
            WorldSpaceCanvas = worldCanvasObj.GetComponent<Canvas>();
        }
    }

    public IEnumerator Fade(UIFade fade, float fadeTime, CanvasGroup cg, Action onDone = null)
    {
        if (cg == null)
            yield break;

        float targetValue = 0;
        float t = 0;
        switch (fade) {
            case UIFade.In:
                targetValue = 1f;
                cg.alpha = 0;
                //t = cg.alpha;
                break;
            case UIFade.Out:
                targetValue = 0;
                t = 1 - cg.alpha;
                //cg.alpha = 1;
                break;
        }

        float alpha = cg.alpha;
        for (t = 0; t < 1.0f; t += Time.unscaledDeltaTime / fadeTime) {
            if (cg == null)
                yield break;
            cg.alpha = Mathf.Lerp(alpha, targetValue, t);
            yield return new WaitForEndOfFrame();
        }

        cg.alpha = targetValue;

        onDone?.Invoke();
    }

    /// <summary>
    /// Open UI in Screen Space
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="fadeTime"></param>
    /// <returns></returns>
    public GameObject Open(assGameUI ui, float fadeTime = 0.4f)
    {
        GameObject opened = null;
        GameObject orig = _uiBags[ui].UIPrefab;
        if (orig != null) {
            CreateCanvas();
            GameObject newObj = (GameObject)Instantiate(orig);
            newObj.transform.SetParent(SceneCanvas.transform, false);
            opened = newObj;
            //CurrentUIObjects.Add(newObj);
            CanvasGroup cg = newObj.GetComponent<CanvasGroup>();
            if (cg != null)
                StartCoroutine(Fade(UIFade.In, fadeTime, cg));
        }
        return opened;
    }

    /// <summary>
    /// Open UI in World Space
    /// </summary>
    /// <param name="target"></param>
    /// <param name="ui"></param>
    /// <param name="fadeTime"></param>
    /// <returns></returns>
    public GameObject Open(GameObject target, assGameUI ui, 
            assUItoOpen interactableType = assUItoOpen.Unassigned ,float fadeTime = 0.4f)
    {
        GameObject opened = null;
        GameObject orig = _uiBags[ui].UIPrefab;
        if (orig != null) {
            CreateCanvas();
            WorldSpaceCanvas.transform.SetParent(target.transform, false);
            WorldSpaceCanvas.transform.localPosition = Vector2.up * 2;
            GameObject newObj = (GameObject)Instantiate(orig);
            newObj.TryGetComponent(out assButton button);
            button.UIToOpenIsNull(interactableType);
            newObj.transform.SetParent(WorldSpaceCanvas.transform, false);
            opened = newObj;
            //CurrentUIObjects.Add(newObj);
            CanvasGroup cg = newObj.GetComponent<CanvasGroup>();
            if (cg != null)
                StartCoroutine(Fade(UIFade.In, fadeTime, cg));
        }
        return opened;
    }

    public void Close(GameObject uiObj, Coroutine co = null, float fadeTime = 0.4f)
    {
        if (uiObj != null) {
            if (co != null)
                StopCoroutine(co);
            //CurrentUIObjects.Remove(uiObj);
            CanvasGroup cg = uiObj.GetComponent<CanvasGroup>();
            if (cg != null) {
                StartCoroutine(Fade(UIFade.Out, fadeTime, cg, () => Destroy(cg.gameObject)));
            }
        }
    }
}

public enum UIFade
{
    None,
    In,
    Out
}

public enum assUItoOpen
{
    Unassigned,
    ShopTrigger,
    ShopPanel,

}