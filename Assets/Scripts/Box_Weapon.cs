using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Box_Weapon : MonoBehaviour
{
    [SerializeField] WeaponData weapon;
    //
    //
    //
    //
    //
    //
    void Start()
    {
        weapon = DataBase.Instance.Items[ExcludeRandom(DataBase.Instance.Items.Count, PlayerStatus.Instance.WeaponsLabel)];
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus.Instance.InvWeapon.Add(weapon);
            PlayerStatus.Instance.WeaponAdd();
            Destroy(gameObject);
        }
    }
    int ExcludeRandom(int n, IReadOnlyList<int> exclude)
    {
        // 0부터 n-1까지의 값을 배열로 만듦
        List<int> numbers = new List<int>();
        for (int i = 0; i < n; i++)
        {
            if (!exclude.Contains(i)) numbers.Add(i); // 제외할 숫자는 추가하지 않음
        }

        // numbers 배열에서 랜덤 값 선택
        if (numbers.Count > 0) return numbers[Random.Range(0, numbers.Count)];
        else return 0;
    }
}
