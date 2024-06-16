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
    public int maxAmmo;
    public int currentAmmo;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform bulletPos; // 총알을 생성해야할 위치
    public Transform bulletCasePos; // 탄피를 생성해야할 위치
    public GameObject bullet; // 총알 프리팹 변수
    public GameObject bulletCase; // 탄피 프리팹 변수

    // 플레이어가 무기를 사용
    public void Use()
    {
        if (type == Type.Melee)
        {
            // 보통 로직 꼬임을 방지하기 위해 코루틴 시작 전 코루틴을 중지하고 시작한다.
            StopCoroutine("Swing"); // 코루틴 중지
            StartCoroutine("Swing"); // 코루틴 시작
        }
        else if (type == Type.Range && currentAmmo > 0)
        {
            currentAmmo--;
            StartCoroutine("Shot");
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

    IEnumerator Shot()
    {
        // #1. 총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;

        // #2. 탄피 배출
        GameObject instantCase = Instantiate(
            bulletCase,
            bulletCasePos.position,
            bulletCasePos.rotation
        );
        Rigidbody caseRigid = instantBullet.GetComponent<Rigidbody>();
        Vector3 caseVec =
            bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3); // 탄피에 가해지는 방향 설정
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }

    // Use() 메인루틴 -> Swing() 서브루틴 -> Use() 메인루틴  === 서브루틴 개념
    // Use() 메인루틴 + Swing() 코루틴 === 동시에 실행
}
