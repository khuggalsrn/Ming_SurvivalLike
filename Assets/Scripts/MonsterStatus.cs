using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewMonster", menuName = "Monster/Monster Data")]
public class MonsterInfo : ScriptableObject
{
    public List<GameObject> items;
    public float MaxHp;
    public float AttackPower;
}
public class MonsterStatus : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    [SerializeField] public readonly MonsterInfo Mob;
    float MaxHp => Mob.MaxHp;
    List<GameObject> items => Mob.items;
    float HitPoint;
    public bool isDead;
    void Start()
    {
        HitPoint = MaxHp;
        animator = GetComponent<Animator>();
    }
    public void OnDamaged(float dmg)
    {
        Debug.Log("몬스터 피격" + dmg);
        HitPoint -= dmg;
        if (HitPoint <= 0 && !isDead)
        {
            isDead = true;
            animator.SetTrigger("Die");
            Die();
        }
        else{
            animator.SetTrigger("Damaged");
        }
        // gameObject.layer = LayerMask.NameToLayer("Immortal");
        // StartCoroutine(LayerBack("Object", 0.1f));
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
        gameObject.layer = LayerMask.NameToLayer("Default");
        Destroy(GetComponent<BoxCollider>());
        Destroy(GetComponent<Rigidbody>());
        foreach (var item in items)
        {
            GameObject go = Instantiate(item, gameObject.transform);
            go.transform.SetParent(null, true);
            // go.transform.position = new Vector3(go.transform.position.x,1,go.transform.position.z);
            int index = go.name.IndexOf("(Clone)");
            if (index > 0)
                go.name = go.name.Substring(0, index) + MaxHp;
        }
        Destroy(gameObject,1f);
    }
    
    // void OnBecameInvisible()
    // {
    //     Debug.Log("가려짐");
    //     animator.enabled = false; // 화면에서 벗어나면 비활성화
    //     transform.GetChild(1).gameObject.SetActive(false);
    //     transform.GetChild(2).gameObject.SetActive(false);
    // }

    // void OnBecameVisible()
    // {
    //     animator.enabled = true; // 화면에 들어오면 다시 활성화
    //     transform.GetChild(1).gameObject.SetActive(true);
    //     transform.GetChild(2).gameObject.SetActive(true);
    // }
}
