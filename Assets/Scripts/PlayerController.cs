using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    public Camera followCamera;
    float hAxis;
    float vAxis;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObj;
    public GameManager manager;

    public AudioSource jumpSound;

    public int maxAmmo;
    public int maxHealth;
    public int maxCoin;
    public int maxHasGrenades;

    public int ammo;
    public int health;
    public int coin;
    public int score;

    public float speed;
    bool walkDown;
    bool fireDown;
    bool reloadDown;
    bool jumpDown;
    bool grenadeDown;
    bool interactionDown;
    bool swapDown1;
    bool swapDown2;
    bool swapDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady;
    bool isReload;
    bool isBorder;
    bool isDamage;
    bool isShop;
    bool isDead;

    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    MeshRenderer[] meshs; // 플레이어는 여러 메쉬 렌더 (여러 부분)으로 이루어져 있어 모든 렌더러를 담기위해 배열로 생성
    Rigidbody rigid;

    GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDelay;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        // PlayerPrefs.SetInt("MaxScore", 112500);
        Debug.Log(PlayerPrefs.GetInt("MaxScore"));
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interaction();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetButton("Walk");
        fireDown = Input.GetButton("Fire1");
        grenadeDown = Input.GetButtonDown("Fire2");
        reloadDown = Input.GetButtonDown("Reload");
        jumpDown = Input.GetButtonDown("Jump");
        interactionDown = Input.GetButtonDown("Interaction");
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
        {
            moveVec = dodgeVec;
        }

        if (isSwap || isDead)
            moveVec = Vector3.zero;

        if (!isBorder)
        {
            transform.position += moveVec * speed * (walkDown ? 0.3f : 1f) * Time.deltaTime;
        }

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", walkDown);
    }

    void Turn()
    {
        // #1. 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec);

        // #2. 마우스에 의한 회전
        if (fireDown && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                // out 키워드란 ray Raycast 함수의 결과를 rayHit 변수에 담는다는 말. 즉 ray가 hit한 리턴의 값이 rayHit에 담겨있음
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if (jumpDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isDead)
        {
            rigid.AddForce(Vector3.up * 20, ForceMode.Impulse); //ForceMode.Impuser는 즉발적인 힘을 가함
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
            jumpSound.Play();
        }
    }

    void Grenade()
    {
        if (hasGrenades == 0)
        {
            return;
        }

        if (grenadeDown && !isReload && !isSwap && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                // out 키워드란 ray Raycast 함수의 결과를 rayHit 변수에 담는다는 말. 즉 ray가 hit한 리턴의 값이 rayHit에 담겨있음
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(
                    grenadeObj,
                    transform.position,
                    transform.rotation
                );
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
        {
            return;
        }

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;
        if (fireDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (equipWeapon == null || equipWeapon.type == Weapon.Type.Melee || ammo == 0 || isDead)
        {
            return;
        }

        if (reloadDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f);
        }
    }

    void ReloadOut()
    {
        int willAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.currentAmmo = willAmmo;
        ammo -= willAmmo;
        isReload = false;
    }

    void Dodge()
    {
        if (
            jumpDown
            && moveVec != Vector3.zero
            && !isJump
            && !isDodge
            && !isSwap
            && !isShop
            && !isDead
        )
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        isDodge = false;
        speed *= 0.5f;
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Floor")
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }
    }

    void Swap()
    {
        if (isDead)
        {
            return;
        }
        if (swapDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (swapDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (swapDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (swapDown1)
            weaponIndex = 0;
        if (swapDown2)
            weaponIndex = 1;
        if (swapDown3)
            weaponIndex = 2;

        if ((swapDown1 || swapDown2 || swapDown3) && !isJump && !isDodge && !isShop)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");
            isSwap = true;

            Invoke("SwapOut", 0.5f);
        }
    }

    void Interaction()
    {
        if (interactionDown && nearObject != null && !isJump && !isDodge && !isShop && !isDead)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }

    void FreezeRotation()
    {
        // 물리 회전 속도. 캐릭터가 다른 물체에 닿았을때 자동으로 회전하는 문제 해결
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(
            transform.position,
            transform.forward,
            5,
            LayerMask.GetMask("Wall")
        );
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                bool isBossAttack = other.name == "Boss Melee Area";

                StartCoroutine(OnDamage(isBossAttack));
            }

            if (other.GetComponent<Rigidbody>() != null)
            {
                // 미사일만 rigidbody를 가짐 이로써 미사일을 구별
                Destroy(other.gameObject);
            }
        }
    }

    IEnumerator OnDamage(bool isBossAttack)
    {
        isDamage = true;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        if (isBossAttack)
        {
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        }

        if (health <= 0 && !isDead)
        {
            OnDie();
        }

        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (isBossAttack)
        {
            rigid.velocity = Vector3.zero;
        }

        if (health <= 0 && !isDead)
        {
            OnDie();
        }
    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
        else if (other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
}
