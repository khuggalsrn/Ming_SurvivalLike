using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    GameObject MousePointer;
    [SerializeField] float jumpForce = 100f;
    Rigidbody rb;
    Animator animator;
    float Dashing = 10f;
    bool Dashed = false;
    [SerializeField] bool isGrounded = true;
    Text DashCool;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        DashCool = GameObject.Find("DashCool").GetComponent<Text>();
        MousePointer = DataBase.Instance.transform.GetChild(0).gameObject;
    }
    private void FixedUpdate()
    {
        // 입력 값 받기
        float horizontal = Input.GetAxisRaw("Horizontal"); // A, D 키
        float vertical = Input.GetAxisRaw("Vertical"); // W, S 키
        Dashing -= Dashing < 0f ? 0f : 0.02f;
        // 입력 벡터 계산 (월드 좌표 기준: Z-forward, X-right)
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 캐릭터 움직이기
        if (moveDirection.magnitude > 0)
        {
            // Rigidbody를 이용해 이동 처리
            Vector3 moveVelocity = moveDirection * PlayerStatus.Instance.Value_Speed;
            if (Input.GetKey(KeyCode.LeftShift) && PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum] >= 10 &&
                Dashing <= 0f)
            {
                Dashed = true;
                Dash(moveVelocity);
            }
            rb.velocity = rb.velocity.magnitude<5?new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z) : rb.velocity; // y축 속도 유지

        }
        else
        {
            // 움직임이 없으면 x, z 속도 멈춤
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
        // 캐릭터 회전
        RotateTowardsMouse();

        // 애니메이션 처리
        HandleAnimations(moveDirection.magnitude);
        DashCool.text = $"Dash : {(Mathf.Round(Dashing*100f)/100f).ToString("F2")}s";
    }
    void Update()
    {
        // 점프 처리
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            isGrounded = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    void Dash(Vector3 Dir)
    {
        float dashDistance = Dir.magnitude;
        PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum] -= 10;
        
        // 전방 방향으로 대시 거리만큼 순간이동
        Vector3 dashTarget = transform.position + Dir.normalized * dashDistance;
        transform.position = dashTarget;

        // 아래로 Raycast를 쏴서 Ground 찾기
        RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up * 5f, Vector3.down, 17f, LayerMask.GetMask("Ground","Object"));
        if (hits.Length > 0)
        {
            Debug.Log("바닥찾았다");
            // 현재 위치와 가장 가까운 y값을 찾기
            float closestY = float.MaxValue;
            Vector3 closestPoint = transform.position;

            foreach (var hit in hits)
            {
                if (Mathf.Abs(hit.point.y - transform.position.y) < Mathf.Abs(closestY - transform.position.y))
                {
                    closestY = hit.point.y;
                    closestPoint = hit.point;
                }
            }

            // 가장 가까운 지점으로 위치를 이동
            transform.position = new Vector3(transform.position.x, closestY+1f, transform.position.z);
        }

        GetComponent<PlayerAttack>().AttackFalse();
    }
    void RotateTowardsMouse()
    {
        // 마우스 위치로부터 Ray 생성
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        
        // Ray가 특정 레이어(groundLayer)에 닿았는지 확인
        RaycastHit[] hits =Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Ground","Monster","Object","Roof"));
        if (hits.Length > 0)
        {
            RaycastHit closestHit = hits[0];
            float closestDistance = Mathf.Abs(transform.position.y - hits[0].point.y);
            foreach (RaycastHit hit in hits)
            {
                float distance = Mathf.Abs(transform.position.y - hit.point.y);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestHit = hit;
                }
            }
            MousePointer.transform.position = closestHit.point;
            // 캐릭터가 마우스 커서를 바라보도록 회전
            Vector3 lookDirection = closestHit.point - transform.position; // 캐릭터와 마우스 위치 간의 벡터
            lookDirection.y = 0; // 높이 차이를 무시
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
    private void HandleAnimations(float moveForward)
    {
        if (moveForward != 0)
        {
            if (Dashed)
            {
                animator.SetBool("IsSprinting", true);
                animator.SetBool("IsWalking", false);
                Dashing = 10f;
                Dashed = false;
            }
            else
            {
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsSprinting", false);
            }
        }
        else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsSprinting", false);
        }
    }
}