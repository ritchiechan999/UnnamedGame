using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "uibag", menuName = "Assimilate/UIBag", order = 1)]
public class UIBags : ScriptableObject
{
    [Header("UI Type + Prefab Path")]
    public assGameUI UI;
    public GameObject UIPrefab;
}

public enum assGameUI
{
    Unassigned,
    //BlockingActorLeft,
    //BlockingActorRight,
    //UnblockingLeftActorLeft,
    //UnblockingLeftActorRight,
    //UnblockingRightActorLeft,
    //UnblockingRightActorRight,
    LoadingScreen,
    ShopTrigger,
    ShopPanel,
    Interactable,

}