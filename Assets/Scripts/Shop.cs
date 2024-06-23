using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;
    PlayerController enterPlayer;

    public void Enter(PlayerController player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero; // RectTransform 의 아래로 땡긴 화면 위치 조절
    }

    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000; // RectTransform 의 아래로 땡긴 화면 위치 조절
    }
}
