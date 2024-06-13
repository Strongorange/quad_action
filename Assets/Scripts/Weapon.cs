using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type
    {
        Melee,
        Range,
    }

    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    // 플레이어가 무기를 사용
    public void Use()
    {
        if (type == Type.Melee)
        {
            // 보통 로직 꼬임을 방지하기 위해 코루틴 시작 전 코루틴을 중지하고 시작한다.
            StopCoroutine("Swing"); // 코루틴 중지
            StartCoroutine("Swing"); // 코루틴 시작
        }
    }

    IEnumerator Swing()
    {
        // // 1
        // yield return null; // 1프레임 대기 // 코루틴에서 yield 는 반드시 1개 이상은 있어야 함.
        // // 2
        // yield return null; // 1프레임 대기 //

        // yield return new WaitForSeconds(0.1f); // 0.1초 대기

        // yield break; // 코루틴 탈출

        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true; // 비활성화 한 Collider 활성화
        trailEffect.enabled = true; // 비활성화 한 Effect 활성화 잔상효과
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    // Use() 메인루틴 -> Swing() 서브루틴 -> Use() 메인루틴  === 서브루틴 개념
    // Use() 메인루틴 + Swing() 코루틴 === 동시에 실행
}
