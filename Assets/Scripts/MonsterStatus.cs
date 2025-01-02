using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterStatus : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    [SerializeField] public MonsterInfo Mob;
    float MaxHp => Mob.MaxHp;
    List<GameObject> items => Mob.items;
    List<float> DefenseForType => Mob.DefenseForType;
    [SerializeField] public float HitPoint;
    public bool isDead;
    public Dictionary<Debuff, int> CurDebuffs = new Dictionary<Debuff, int>();
    void Start()
    {
        foreach (Debuff debuff in Enum.GetValues(typeof(Debuff)))
        {
            CurDebuffs[debuff] = 0;
        }
        // //150초뒤에 자동으로 사라짐. Limit 개체수 유지를 위함.
        // StartCoroutine(Erase());
        HitPoint = MaxHp;
        animator = GetComponent<Animator>();
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            // SkinnedMeshRenderer 컴포넌트를 가진 경우
            SkinnedMeshRenderer skinnedMeshRenderer = child.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                // Renderer Opt 스크립트 추가 (이미 있다면 중복 추가 방지)
                if (child.gameObject.GetComponent<RendererOpt>() == null)
                {
                    child.gameObject.AddComponent<RendererOpt>();
                }
                else
                {
                }
            }
        }
    }
    void FixedUpdate(){
        if (CurDebuffs[Debuff.Exposed]==1){
            PlayerStatus.Instance.transform.LookAt(new Vector3(transform.position.x,PlayerStatus.Instance.transform.position.y, transform.position.z));
        }
    }
    public bool GetDebuff(Debuff debuff, DamageType type, int times, float dmg = 1f)
    {
        bool ReturnValue = false;
        CurDebuffs[debuff] += times;
        ReturnValue = SkillManager.Instance.Bleeding(gameObject, ref dmg, DefenseForType[(int)type], ref CurDebuffs, ref HitPoint);
        if (HitPoint <= 0 && !isDead)
        {
            isDead = true;
            animator.SetTrigger("Die");
            Die();
        }
        else
        {
            animator.SetTrigger("Damaged");
        }

        return ReturnValue;
    }
    public void OnDamaged(float dmg, DamageType type)
    {
        SkillManager.Instance.RangeAddiDamage(transform, ref dmg);
        SkillManager.Instance.FocusAddiDamage(ref CurDebuffs, ref dmg);

        dmg = dmg * 100f / (100f + DefenseForType[(int)type]);
        Debug.Log("몬스터가 피해를 입음" + dmg + type.ToString());
        HitPoint -= dmg;
        if (HitPoint <= 0 && !isDead)
        {
            isDead = true;
            animator.SetTrigger("Die");
            Die();
        }
        else
        {
            animator.SetTrigger("Damaged");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Attack"))
        {
            Debug.Log($"{other.name}");
        }
    }
    void Die()
    {
        GameManager.Instance.CurMonsterNum -= 1;
        gameObject.layer = LayerMask.NameToLayer("Default");
        Destroy(GetComponent<CapsuleCollider>());
        Destroy(GetComponent<Rigidbody>());
        foreach (var item in items)
        {
            GameObject go = Instantiate(item, gameObject.transform);
            go.transform.SetParent(null, true);
            int index = go.name.IndexOf("(Clone)");
            if (index > 0)
                go.name = go.name.Substring(0, index);
        }
        Destroy(gameObject, 1f);
    }
    IEnumerator Erase()
    {
        yield return new WaitForSeconds(150f);
        GameManager.Instance.CurMonsterNum -= 1;
        Destroy(this.gameObject);
    }
}
