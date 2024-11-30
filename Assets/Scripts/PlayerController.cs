using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 100f;
    public LayerMask groundLayer; // 마우스 커서를 기준으로 바라볼 레이어
    private Rigidbody rb;
    private Animator animator;
    float Dashing = 10f;
    bool Dashed = false;
    [SerializeField] private bool isGrounded = true;
    Text DashCool;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        DashCool = GameObject.Find("DashCool").GetComponent<Text>();
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
            if (Input.GetKey(KeyCode.LeftShift) && PlayerStatus.Instance.Stamina[PlayerStatus.Instance.CurWeaponnum] >= 10 &&
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
        Debug.Log(collision);
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    void Dash(Vector3 Dir)
    {
        PlayerStatus.Instance.Stamina[PlayerStatus.Instance.CurWeaponnum] -= 10;
        rb.AddForce(Dir * 50, ForceMode.Impulse);
        GetComponent<PlayerAttack>().AttackFalse();
    }
    void RotateTowardsMouse()
    {
        // 마우스 위치로부터 Ray 생성
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Ray가 특정 레이어(groundLayer)에 닿았는지 확인
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // 캐릭터가 마우스 커서를 바라보도록 회전
            Vector3 lookDirection = hit.point - transform.position; // 캐릭터와 마우스 위치 간의 벡터
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