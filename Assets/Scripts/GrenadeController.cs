using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero; // 속도 0
        rigid.angularVelocity = Vector3.zero; // 회전속도 0
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        RaycastHit[] rayHits = Physics.SphereCastAll(
            transform.position,
            15,
            Vector3.up,
            0, // Ray길이 0으로 해야 범위 감지
            LayerMask.GetMask("Enemy")
        );

        foreach (RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<EnemyController>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
