using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type
    {
        Ammo,
        Coin,
        Grenade,
        Heart,
        Weapon
    };

    Rigidbody rigid;
    SphereCollider sphereCollider;

    public Type type;
    public int value;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 30 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Floor")
        {
            // 게임 시작되고 땅에 닿으면 물리효과 비활성화
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
