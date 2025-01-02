using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Item : MonoBehaviour
{
    void Update()
    {
        // y축 기준으로 계속 회전
        transform.Rotate(0, 120f * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UI_Manager.Instance.MakeItem();
            Destroy(gameObject);
        }
    }
}
