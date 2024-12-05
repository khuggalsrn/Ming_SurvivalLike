using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    // 시작 지점과 도착 지점 설정
    Vector3 startPoint;
    [SerializeField] Transform endObject;
    Vector3 endPoint;

    // 엘리베이터 이동 속도 (초당 거리)
    [SerializeField] float speed = 2f;
    // 대기 시간 (15초)
    [SerializeField] float waitTime = 15f;
    [SerializeField] bool movingUp = false;
    public float Speed{
        get { return speed; }
        set { speed = value; }
    }
    public float WaitTime{
        get { return waitTime; }
        set { waitTime = value; }
    }
    public bool MovingUp{
        get { return movingUp; }
        set { movingUp = value; }
    }
    //
    //
    //
    //
    //
    //
    void Start()
    {
        // 초기 위치를 시작 지점으로 설정
        startPoint = transform.position;
        endPoint = endObject.position;
        StartCoroutine(ElevatorRoutine());
    }

    IEnumerator ElevatorRoutine()
    {
        while (true)
        {
            // 현재 목표 지점
            Vector3 targetPosition = movingUp ? startPoint: endPoint;

            // 목표 지점으로 이동
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            // 목표 지점에 도달했을 때 반전
            movingUp = !movingUp;

            // 대기
            yield return new WaitForSeconds(waitTime);
        }
    }
}
