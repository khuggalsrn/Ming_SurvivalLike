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
    [SerializeField] float moveSpeed = 0;
    [SerializeField] float time = 0;
    [SerializeField] float distance = 0;
    float AttackPower => stat.Mob.AttackPower;
    public float MoveSpeed
    {
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
        moveSpeed = stat.Mob.MoveSpeed;
        Range = Range == 0 ? 3.25f : Range;
        animator = GetComponent<Animator>();
        // NavMeshAgent 컴포넌트를 가져옴
        navMeshAgent = GetComponent<NavMeshAgent>();
        // 초기 속도를 설정
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
        navMeshAgent.isStopped = false;
        UpdatePath();
    }
    void FixedUpdate()
    {
        if (stat.isDead) return;
        time += 0.02f;
        if (navMeshAgent.isOnOffMeshLink) // navMeshAgent가 offmeshlink 타고있을때
        {
            // Off Mesh Link를 커스터마이징하여 동작
            StartCoroutine(TraverseOffMeshLink(navMeshAgent));
        }

        if (PlayerStatus.Instance != null)
        {
            distance = Vector3.Distance(PlayerStatus.Instance.transform.position, transform.position);
            if (distance < 10f)
            {
                if (time >= 0.2f)
                {
                    time = 0f;
                    navMeshAgent.speed = moveSpeed;
                    UpdatePath();
                }
            }
            else if (distance < 25f)
            {
                if (time >= 0.5f)
                {
                    time = 0f;
                    navMeshAgent.speed = moveSpeed;
                    UpdatePath();
                }
            }
            else if (distance < 50f)
            {
                if (time >= 1f)
                {
                    time = 0f;
                    navMeshAgent.speed = MoveSpeed;
                    UpdatePath();
                }
            }
            else
            {
                if (time >= 5f)
                {
                    navMeshAgent.speed = moveSpeed;
                    time = 0f;
                    navMeshAgent.speed = moveSpeed;
                    UpdatePath();
                }
            }
        }
    }
    IEnumerator TraverseOffMeshLink(NavMeshAgent agent)
    {
        OffMeshLinkData linkData = agent.currentOffMeshLinkData;

        // 링크의 시작과 끝 위치
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = linkData.endPos;

        float duration = 0.5f * (startPos - endPos).magnitude; // 이동 시간
        float elapsedTime = 0f;
        agent.CompleteOffMeshLink();
        // 링크 이동 완료시켜버리기 = 다른 몹들 탈 수 있음.

        while (elapsedTime < duration)
        {
            agent.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 링크 끝에 도달
        agent.transform.position = endPos;

        // agent.CompleteOffMeshLink();
    }
    void UpdatePath()
    {

        if (PlayerStatus.Instance != null)
        {

            if (navMeshAgent.enabled == true)
            {
                // 플레이어의 위치를 목표로 설정
                if(!navMeshAgent.SetDestination(PlayerStatus.Instance.transform.position)){
                    Debug.LogWarning("SetDest is Failed");
                }

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
}
