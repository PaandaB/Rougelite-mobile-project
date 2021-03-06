﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class JEnemy : MonoBehaviour
{
    float overlapRange = 15f;
    public LayerMask mask;

    public EnemyStats EnemyStats;
    public EnemyType thisType;

    //private enum WhatTypeOfEnemy { weak, normal, strong };
    //private WhatTypeOfEnemy myType;

    public enum EnemyState { non, idle, backOff, walking, attack, damage, dead };
    public EnemyState myState = EnemyState.non;

    public Collider2D myHitbox;
    public Collider2D myRoom;
    public Rigidbody2D rb;
    SpriteRenderer myRend;

    public AIPath aIPath;
    public AIDestinationSetter destination;

    public HealthSystem healthSystem;

    public GameObject damagePopup;
    public TextMeshProUGUI dmgtext;
    public GameObject attackPrefab;
    public GameObject loot;
    public GameObject player;
    public GameObject destroyChildren;

    public float speed = 30f;
    public float myHealth;
    public float debugHealth;
    public float damage;


    public float dirRotation;
    public string direction;
    public Vector2 walkPoint;
    public Vector3 startPos;
    public GameObject startPosO;

    public bool blocksLight;
    public bool hidesInDark;
    public bool hidesInLight;
    public bool attacked;
    public bool swordprojectileattacked;
    public bool hasAttacked = false;
    private bool isDead = false;
    bool nonStateChecker = true;
    bool arrowKnockback = true;

    public int level;

    public int backOffCounter = 0;

    public Transform pointA, pointB, pointC, pointD;

    #region Loot drops
    [Header("Loot dropping")]
    private int drop;
    public int[] Table;
    public int commondropRange;
    public int raredropRange;
    public int legendarydropRange;
    public int ancientdropRange;
    public int potiondropRange;
    public int none;
    public int lootTotal;
    public bool dropping;

    public GameObject commonLoot;
    public GameObject rareLoot;
    public GameObject legendaryLoot;
    public GameObject ancientLoot;
    public GameObject potion;
    #endregion

    #region Animation
    Animator anim;
    bool isIdle;
    public bool isWalking;
    bool isHurt;
    bool isAttacking;
    bool playHurtAnim, playAttackAnim;
    #endregion

    EnemySound mySounds;
    void Start()
    {
        mySounds = GetComponent<EnemySound>();
        GameObject newStartPosO = Instantiate(startPosO, transform.position, Quaternion.identity, transform.parent);
        startPosO = newStartPosO;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        myRoom = transform.parent.GetComponent<Collider2D>();
        myHitbox = GetComponentInChildren<Collider2D>();

        aIPath = GetComponent<AIPath>();
        destination = GetComponent<AIDestinationSetter>();

        pointA = GetComponentInChildren<Animation>().transform;
        pointB = GetComponentInChildren<BoxCollider>().transform;
        pointC = GetComponentInChildren<MeshFilter>().transform;
        pointD = GetComponentInChildren<SphereCollider>().transform;

        //DecideEnemyType();

        startPos = transform.position;

        thisType = EnemyType.trash;
        level = SceneManager.GetActiveScene().buildIndex;
        EnemyStats = new EnemyStats(thisType, level);

        myHealth = EnemyStats.health;
        damage = EnemyStats.damage;
        speed = EnemyStats.speed;
        healthSystem = new HealthSystem(myHealth);


        Table = EnemyStats.Table;
        commondropRange = EnemyStats.commondropRange;
        raredropRange = EnemyStats.raredropRange;
        legendarydropRange = EnemyStats.legendarydropRange;
        ancientdropRange = EnemyStats.ancientdropRange;
        potiondropRange = EnemyStats.potiondropRange;
        none = EnemyStats.none;
        lootTotal = EnemyStats.lootTotal;

        blocksLight = EnemyStats.blocksLight;
        hidesInDark = EnemyStats.hidesInDark;
        hidesInLight = EnemyStats.hidesInLight;

        aIPath.maxSpeed = speed;

        anim = GetComponentInChildren<Animator>();
        myRend = GetComponentInChildren<SpriteRenderer>();
    }

    /*public void DecideEnemyType()
    {
        int randoNumber = Mathf.RoundToInt(Random.Range(0, 3));

        if (randoNumber == 0)
        {
            myType = WhatTypeOfEnemy.weak;
            healthSystem = new HealthSystem(30);
            GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else if (randoNumber == 1)
        {
            myType = WhatTypeOfEnemy.normal;
            healthSystem = new HealthSystem(50);
            GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        else if (randoNumber == 2)
        {
            myType = WhatTypeOfEnemy.strong;
            healthSystem = new HealthSystem(60);
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }*/
    public void GetDirFromPlayer()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        int dirTwo = Mathf.RoundToInt(dir.x);
        int dirThree = Mathf.RoundToInt(dir.y);
        Vector2 dirVector = new Vector2(dirTwo, dirThree);
        if (myState != EnemyState.attack && !isDead)
        {
            if (dirVector == new Vector2(0f, 1f))
            {
                direction = "U";
                dirRotation = 0;
            }
            else if (dirVector == new Vector2(0f, -1f))
            {
                direction = "D";
                dirRotation = 180;
            }
            else if (dirVector == new Vector2(1f, 0f))
            {
                direction = "R";
                dirRotation = 270;
            }
            else if (dirVector == new Vector2(-1f, 0f))
            {
                direction = "L";
                dirRotation = 90;
            }
        }
    }

    void Anims()
    {
        anim.SetBool("IsWalking", isWalking);
        anim.SetBool("IsIdle", isIdle);
        anim.SetBool("HasAttacked", hasAttacked);

        if (direction == "L")
        {
            myRend.sortingOrder = 149;
            Transform animScale = anim.gameObject.transform;
            animScale.localScale = new Vector3(-1, 1, 1);
        }
        if (direction == "R")
        {
            myRend.sortingOrder = 149;
            Transform animScale = anim.gameObject.transform;
            animScale.localScale = new Vector3(1, 1, 1);
        }
        if (direction == "U")
        {
            myRend.sortingOrder = 151;
        }
        if (direction == "D")
        {
            myRend.sortingOrder = 149;
        }
    }

    void ResetAnimForAttacknHurt(int i)
    {
        if(i == 0) //Hurt anim
        {

        }
        else if(1 == 1) //Attack anim
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        myHealth = healthSystem.GetHealth();

        attacked = false;
        swordprojectileattacked = false;
        switch (myState)
        {
            case EnemyState.non:
                NonState();
                break;
            case EnemyState.idle:
                destination.enabled = false;
                isIdle = true;
                isWalking = false;
                StartCoroutine(IdleState());
                break;
            case EnemyState.walking:
                destination.enabled = true;
                isWalking = true;
                isIdle = false;
                StartCoroutine(WalkState());
                break;
            case EnemyState.attack:
                StartCoroutine(AttackState());
                break;
            case EnemyState.backOff:
                BackOffState();
                break;
            case EnemyState.damage:
                StartCoroutine(DamageState());
                break;
            case EnemyState.dead:
                DeathState();
                break;

        }

        GetDirFromPlayer();

        if (myHealth <= 0 && !isDead)
        {
            mySounds.PlayDeathSound();
            //DropLootAndDie();
            myState = EnemyState.dead;
        } //ded

        if (player == null)
        {
            Destroy(gameObject);
        }

        Anims();
    }

    void NonState()
    {
        if (nonStateChecker)
        {
            StopAllCoroutines();
            nonStateChecker = true;
        }
        if (myRoom.bounds.Contains(player.transform.position))
        {
            myState = EnemyState.idle;
        }
    }

    IEnumerator IdleState()
    {
        StopCoroutine(WalkState());
        if (player == null)
        {
            Destroy(gameObject);
        }
        else if (!myRoom.bounds.Contains(player.transform.position))
        {
            myState = EnemyState.non;
        }
        yield return new WaitForSeconds(2f);
        mySounds.PlayVoiceSound(); //VOICE SOUND
        myState = EnemyState.walking;

        yield return null;
    }

    IEnumerator WalkState()
    {
        StopCoroutine(IdleState());
        StopCoroutine(AttackState());
        //StopCoroutine(IdleState());
        //yield return new WaitForSeconds(Random.Range(0, 3));
        aIPath.enabled = true;
        aIPath.canMove = true;
        destination.target = player.transform;
        aIPath.maxSpeed = speed;
        while (myRoom.bounds.Contains(player.transform.position)) //Go to rue
        {
            Collider2D overlap = Physics2D.OverlapCircle(transform.position, overlapRange, mask);
            yield return new WaitForSeconds(.01f);
            if (aIPath.reachedEndOfPath && overlap)
            {
                mySounds.PlayVoiceSound(); //VOICE SOUND
                if (!hasAttacked && !isDead)
                {
                    mySounds.PlayAttackSound();
                }
                aIPath.enabled = false;
                myState = EnemyState.attack;
            }
        }
        if (!myRoom.bounds.Contains(player.transform.position)) //Return to startPos
        {
            yield return new WaitForSeconds(.2f);
            transform.position = startPosO.transform.position;
            aIPath.canMove = false;
            aIPath.enabled = false;
            myState = EnemyState.idle;
        }
        aIPath.canMove = false;
        myState = EnemyState.idle;

        yield return null;
    }

    IEnumerator AttackState()
    {
        StopCoroutine(WalkState());
        anim.SetTrigger("DoAttack");
        yield return new WaitForSeconds(.5f);
        anim.ResetTrigger("DoAttack");
        isAttacking = true;
        aIPath.canMove = false;
        aIPath.enabled = false;
        if (!hasAttacked && !isDead)
        {
            mySounds.PlayAttackSound();
            InstanstiateAttack();
            hasAttacked = true;
        }
        yield return new WaitForSeconds(.5f);
        myState = EnemyState.backOff;
        yield return null;
    }

    void InstanstiateAttack()
    {
        GameObject attackClone = Instantiate(attackPrefab, transform.position, Quaternion.Euler(0, 0, dirRotation), transform); //Add an attackPoint
        Destroy(attackClone, 0.3f);
    }

    void BackOffState()
    {
            destination.enabled = true;
            aIPath.enabled = true;
            aIPath.canMove = true;
        if (backOffCounter == 0)
        {
            StopAllCoroutines();
            if (direction == "U")
            {
                destination.target = pointC;
            }
            else if (direction == "D")
            {
                destination.target = pointD;
            }
            else if (direction == "L")
            {
                destination.target = pointB;
            }
            else if (direction == "R")
            {
                destination.target = pointA;
            }

            //aIPath.maxSpeed = speed * 2;


            
            Invoke("BackToWalk", 2f);
            backOffCounter = 1;
        }
    }

    void BackToWalk()
    {
        hasAttacked = false;
        myState = EnemyState.walking;
        backOffCounter = 0;
    }

    IEnumerator DamageState()
    {
        if (!arrowKnockback)
        {
            anim.SetTrigger("TakeDamage");
            yield return new WaitForSeconds(.1f);
            anim.ResetTrigger("TakeDamage");
            arrowKnockback = true;
            isHurt = true;
            yield return new WaitForSeconds(.9f);
            myState = EnemyState.walking;
            yield return null;
        }
        else
        {
            anim.SetTrigger("TakeDamage");
            yield return new WaitForSeconds(.1f);
            anim.ResetTrigger("TakeDamage");
            aIPath.enabled = false;
            destination.enabled = false;
            isHurt = true;
            yield return new WaitForSeconds(.9f);
            myState = EnemyState.walking;
            yield return null;
        }

    }

    void DeathState()
    {
        rb.bodyType = RigidbodyType2D.Static;
        StopAllCoroutines();
        isDead = true;
        aIPath.enabled = false;
        destination.enabled = false;
        anim.SetTrigger("Dead");
        GetComponent<Collider2D>().enabled = false;
        //this.enabled = false;
        //DropLootAndDie();
        if (!dropping)
        {
            dropping = true;
            Invoke("DropLootAndDie", 2f);
        }
    }

    void DamagePopUp(float damage, bool crit)
    {
        float fadetime = 0.7f;

        double dmgprint = System.Math.Round(damage, 2);
        if (!crit)
        {
            mySounds.PlayDamageSound();
            GameObject dmgpopupclone = Instantiate(damagePopup, transform.position + transform.up * 15, Quaternion.identity);
            dmgtext = dmgpopupclone.GetComponentInChildren<TextMeshProUGUI>();
            dmgpopupclone.AddComponent<Rigidbody2D>();
            dmgpopupclone.GetComponent<Rigidbody2D>().velocity = RandomVector(-10f, 10f);
            dmgtext.CrossFadeAlpha(0, fadetime, false);
            dmgtext.text = dmgprint.ToString();
            dmgpopupclone.SetActive(true);
            Destroy(dmgpopupclone, fadetime);
        }
        else
        {
            mySounds.PlayDamageSound();
            GameObject dmgpopupclone = Instantiate(damagePopup, transform.position + transform.up * 18, Quaternion.identity);
            dmgtext = dmgpopupclone.GetComponentInChildren<TextMeshProUGUI>();
            dmgpopupclone.AddComponent<Rigidbody2D>();
            dmgpopupclone.GetComponent<Rigidbody2D>().velocity = RandomVector(-20f, 20f);
            dmgtext.color = Color.red;
            dmgtext.fontSize = 42;
            dmgtext.CrossFadeAlpha(0, fadetime, false);
            dmgtext.text = dmgprint.ToString();
            dmgpopupclone.SetActive(true);
            Destroy(dmgpopupclone, fadetime);
        }
    }

    Vector3 RandomVector(float min, float max)
    {
        float x = UnityEngine.Random.Range(min, max);
        float y = 10;
        float z = 0;
        return new Vector3(x, y, z);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.tag == "Sword" && !isDead || collider.tag == "Arrow" && !isDead || collider.tag == "SwordProjectile" && !isDead)
        {
            
            PlayerStats playerstats = player.GetComponent<PlayerStats>();
            Player rue = player.GetComponent<Player>();
            Arrow arrowCrit = collider.GetComponent<Arrow>();
            Swordscript swordCrit = collider.GetComponent<Swordscript>();
            SwordProjectile swordProjectileCrit = collider.GetComponent<SwordProjectile>();
            float str = playerstats.Strength.Value;
            float dex = playerstats.Dexterity.Value;
            float critdmg = playerstats.CritDamage.Value / 100;
            float currenthpdmg = playerstats.PercentHpDmg.Value;
            float ruehpdmg = playerstats.RueHPDmgOnHit.Value;
            float bowmod = playerstats.CrossbowAttackModifier.Value;
            float swordmod = playerstats.SwordAttackModifier.Value;
            float rapidfire = playerstats.RapidFire.Value;
            float firearrow = playerstats.FireArrows.Value;
            float swordexecute = playerstats.SwordExecute.Value;
            bool crit = false;



            
            if (collider.tag == "Arrow")
            {
                if (attacked)
                {
                    return;
                }
                float damage = 0;
                if (rapidfire > 0)
                {
                    damage = ((str / 10) + dex) * bowmod;
                }
                if (rapidfire == 0)
                {
                    damage = str * bowmod;
                }
                if (firearrow > 0)
                {
                    damage *= 2;
                }
                if (arrowCrit.crit)
                {
                    damage *= critdmg;
                    crit = true;
                }
                if (currenthpdmg > 0)
                {
                    damage = damage + (myHealth * 0.1f);
                }
                if (ruehpdmg > 0)
                {
                    damage += (playerstats.Health.Value * 0.1f);
                }
                DamagePopUp(damage, crit);
                healthSystem.Damage(damage);
                rue.HealthSystem.Heal(playerstats.LifeOnHit.Value);
                attacked = true;
                
                if (playerstats.ArrowKnockback.Value < 0)
                {
                    arrowKnockback = false;
                    myState = EnemyState.damage;
                }
                else
                {
                    myState = EnemyState.damage;
                }
            }
            else if (collider.tag == "Sword")
            {
                if (attacked)
                {
                    return;
                }
                float damage = str * swordmod;
                if (swordexecute > 0 && myHealth <= (EnemyStats.health * 0.2))
                {
                    damage = myHealth;
                }
                if (swordCrit.Crit)
                {
                    damage *= critdmg;
                    crit = true;
                }
                if (currenthpdmg > 0)
                {
                    damage = damage + (myHealth * 0.1f);
                }
                if (ruehpdmg > 0)
                {
                    damage += (playerstats.Health.Value * 0.1f);
                }
                DamagePopUp(damage, crit);
                healthSystem.Damage(damage);
                rue.HealthSystem.Heal(playerstats.LifeOnHit.Value);
                attacked = true;
                myState = EnemyState.damage;
            }
            else if (collider.tag == "SwordProjectile")
            {
                if (swordprojectileattacked)
                {
                    return;
                }
                float damage = str * swordmod;
                if (swordexecute > 0 && myHealth <= (EnemyStats.health * 0.2))
                {
                    damage = myHealth;
                }
                if (swordProjectileCrit.crit)
                {
                    damage *= critdmg;
                    crit = true;
                }
                if (currenthpdmg > 0)
                {
                    damage = damage + (myHealth * 0.1f);
                }
                if (ruehpdmg > 0)
                {
                    damage += (playerstats.Health.Value * 0.1f);
                }
                DamagePopUp(damage, crit);
                healthSystem.Damage(damage);
                rue.HealthSystem.Heal(playerstats.LifeOnHit.Value);
                swordprojectileattacked = true;
                myState = EnemyState.damage;
            }
        }
    } //Deal damage to enemy

    void DropLootAndDie()
    {
        PlayerStats playerstats = player.GetComponent<PlayerStats>();
        if (playerstats.DropGarantueed.Value == 0)
        {
            foreach (var item in Table) //checks table
            {
                lootTotal += item;
            }
            float randomNumber = Random.Range(0, (lootTotal + 1)); //pulls random number based on table total + 1


            foreach (var weight in Table) //weight, is the number listed in the table of drop chance.
            {
                if (randomNumber <= weight) //if less or equal to a weight, give item
                {


                    if (weight == commondropRange)
                    {
                        Instantiate<GameObject>(commonLoot, transform.position, Quaternion.identity);
                        Corpse();
                        return;
                    }

                    if (weight == raredropRange)
                    {
                        Instantiate<GameObject>(rareLoot, transform.position, Quaternion.identity);
                        Corpse();
                        return;
                    }

                    if (weight == legendarydropRange)
                    {
                        Instantiate<GameObject>(legendaryLoot, transform.position, Quaternion.identity);
                        Corpse();
                        return;
                    }

                    if (weight == ancientdropRange)
                    {
                        Instantiate<GameObject>(ancientLoot, transform.position, Quaternion.identity);
                        Corpse();
                        return;
                    }

                    if (weight == potiondropRange)
                    {
                        Instantiate<GameObject>(potion, transform.position, Quaternion.identity);
                        Corpse();
                        return;
                    }

                    if (weight == none)
                    {
                        Corpse();
                        return;
                    }
                }

                else //if not, roll -= highest value weight.

                {
                    randomNumber -= weight;
                }
            }
        }

        if (playerstats.DropGarantueed.Value > 0)
        {
            float randomNumberDrop = Random.Range(0, 2);
            if (randomNumberDrop == 0)
            {
                float randomnumberLoot = Random.Range(0, 4);
                if (randomnumberLoot == 0)
                {
                    Instantiate<GameObject>(commonLoot, transform.position, Quaternion.identity);
                    Corpse();
                    return;
                }
                if (randomnumberLoot == 1)
                {
                    Instantiate<GameObject>(rareLoot, transform.position, Quaternion.identity);
                    Corpse();
                    return;
                }
                if (randomnumberLoot == 2)
                {
                    Instantiate<GameObject>(legendaryLoot, transform.position, Quaternion.identity);
                    Corpse();
                    return;
                }
                if (randomnumberLoot == 3)
                {
                    Instantiate<GameObject>(ancientLoot, transform.position, Quaternion.identity);
                    Corpse();
                    return;
                }
            }
            if (randomNumberDrop == 1)
            {
                float randomnumberLoot = Random.Range(0, 4);
                if (randomnumberLoot == 0)
                {
                    Instantiate<GameObject>(commonLoot, transform.position, Quaternion.identity);
                    Instantiate<GameObject>(potion, transform.position + transform.right * 10, Quaternion.identity);
                    Corpse();
                    return;
                }
                if (randomnumberLoot == 1)
                {
                    Instantiate<GameObject>(rareLoot, transform.position, Quaternion.identity);
                    Instantiate<GameObject>(potion, transform.position + transform.right * 10, Quaternion.identity);
                    Corpse();
                    return;
                }
                if (randomnumberLoot == 2)
                {
                    Instantiate<GameObject>(legendaryLoot, transform.position, Quaternion.identity);
                    Instantiate<GameObject>(potion, transform.position + transform.right * 10, Quaternion.identity);
                    Corpse();
                    return;
                }
                if (randomnumberLoot == 3)
                {
                    Instantiate<GameObject>(ancientLoot, transform.position, Quaternion.identity);
                    Instantiate<GameObject>(potion, transform.position + transform.right * 10, Quaternion.identity);
                    Corpse();
                    return;
                }
            }
        }
    }
    public void Corpse()
    {
        Destroy(destroyChildren);
        GetComponentInChildren<Animator>().enabled = false;
        transform.DetachChildren();
        Destroy(gameObject);
    }

    private void OnBecameVisible()
    {
        this.enabled = true;
    }

    private void OnBecameInvisible()
    {
        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, overlapRange);
    }
}
