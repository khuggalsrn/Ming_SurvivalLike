using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterController : MonoBehaviour
{
    MonsterStatus stat;
    [SerializeField] float Range;
    Animator animator;
    [SerializeField] bool isAttacking = false;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] float moveSpeed;
    float time = 0;
    float AttackPower => stat.Mob.AttackPower;
    public float MoveSpeed{
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }
    //
    //
    //
    //
    //
    //
    void Awake()
    {

        stat = GetComponent<MonsterStatus>();
    }
    void Start()
    {
        Range = Range == 0 ? 3.25f : Range;
        animator = GetComponent<Animator>();
        // NavMeshAgent 컴포넌트를 가져옴
        navMeshAgent = GetComponent<NavMeshAgent>();
        // 초기 속도를 설정
        navMeshAgent.speed = MoveSpeed;
    }
    void FixedUpdate()
    {
        if (stat.isDead) return;
        time += 0.02f;
        // if (isAttacking)
        // {
        //     navMeshAgent.speed = 0;
        // }
        // else
        // {
        //     navMeshAgent.speed = MoveSpeed;
        // }
        // if (isOnRange && !isAttacking)
        // {
        //     isAttacking = true;
        //     animator.SetTrigger("Attack");
        // }
        // float distanceToPlayer = Vector3.Distance(transform.position, PlayerStatus.Instance.transform.position);

        // if (distanceToPlayer <= Range) isOnRange = true;
        // else isOnRange = false;
        if (time >= 0.1f)
        {
            navMeshAgent.speed = MoveSpeed;
            time = 0f;
            UpdatePath();
        }
    }
    void UpdatePath()
    {

        if (PlayerStatus.Instance != null)
        {

            if (navMeshAgent.enabled == true)
            {
                // 플레이어의 위치를 목표로 설정
                navMeshAgent.SetDestination(PlayerStatus.Instance.transform.position);

                Vector3 moveDirection = navMeshAgent.velocity; // 현재 이동 벡터
                if (moveDirection.magnitude > 0.1f) // 이동 중일 때만 회전
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
            }
        }
    }
    public void AttackTrue()
    {
        StartCoroutine(AttackFalse());
    }
    IEnumerator AttackFalse()
    {
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.LogWarning("플레이어 닿았음" + other.name);
            PlayerStatus.Instance.OnDamaged(AttackPower);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("플레이어 닿았음" + other.gameObject.name);
            PlayerStatus.Instance.OnDamaged(AttackPower);
        }
    }
    // void OnBecameInvisible()
    // {
    //     gameObject.SetActive(false); // 화면에서 벗어나면 비활성화
    // }

    // void OnBecameVisible()
    // {
    //     GetComponent<Animator>();gameObject.SetActive(true); // 화면에 들어오면 다시 활성화
    // }
}
