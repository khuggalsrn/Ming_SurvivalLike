using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerCamera : MonoBehaviour
{
    Transform player => PlayerStatus.Instance.transform; // 캐릭터 Transform
    [SerializeField] LayerMask TargetLayer1; // 바꿀 레이어
    [SerializeField] LayerMask TargetLayer2; // 바꿀 레이어
    LayerMask TargetLayer3; // 바꿀 레이어
    [SerializeField] Material Alpha0;
    [SerializeField] Material Alpha005;
    private List<Renderer> transparentRenderers = new List<Renderer>(); // 투명 처리된 Material
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>(); // 원래 Material 저장
    // 이미 투명화된 Renderer 목록 초기화
    List<Renderer> currentlyHitRenderers = new List<Renderer>();
    private RaycastHit[] buffer; // 재사용할 버퍼
    void Start()
    {
        TargetLayer1 = LayerMask.GetMask("Roof");
        TargetLayer2 = LayerMask.GetMask("Object");
        // TargetLayer3 = LayerMask.GetMask("Object");
    }
    void Update()
    {
        transform.rotation = Quaternion.Euler(52.5f, 0, 0);
        if (PlayerStatus.Instance)
        {
            transform.position = PlayerStatus.Instance.transform.position + new Vector3(0, 30, -25);
             // 이미 투명화된 Renderer 목록 초기화
            currentlyHitRenderers = new List<Renderer>();
            HandleCeilingTransparency(TargetLayer1, Alpha0);
            HandleCeilingTransparency(TargetLayer2, Alpha005);
            UnuseRenderer();
        }
    }
    RaycastHit[] MergeArrays(RaycastHit[][] arrays)
    {
        // 전체 길이 계산
        int totalLength = 0;
        foreach (var array in arrays)
        {
            totalLength += array.Length;
        }

        // 기존 버퍼 크기 확인 후 필요 시 재할당
        if (buffer == null || buffer.Length < totalLength)
        {
            buffer = new RaycastHit[totalLength];
        }

        // 배열을 합쳐 버퍼에 복사
        int currentIndex = 0;
        foreach (var array in arrays)
        {
            Array.Copy(array, 0, buffer, currentIndex, array.Length);
            currentIndex += array.Length;
        }

        // 필요한 길이만큼 반환 (buffer 내부 데이터를 잘라 사용)
        RaycastHit[] result = new RaycastHit[totalLength];
        Array.Copy(buffer, 0, result, 0, totalLength);
        return result;
    }
    void HandleCeilingTransparency(LayerMask TargetLayer, Material transparentMaterial)
    {
        // 카메라와 캐릭터 사이에 Ray를 쏨
        Vector3 cameraPosition = transform.position;
        Vector3 playerPosition = player.position;
        Vector3 direction = playerPosition - cameraPosition;
        float distance = Vector3.Distance(cameraPosition, playerPosition)-2.5f;


        // Raycast로 감지
        RaycastHit[] hits = Physics.RaycastAll(cameraPosition, direction, distance, TargetLayer);
        RaycastHit[] hits1 = Physics.RaycastAll(cameraPosition, direction + Vector3.left, distance, TargetLayer);
        RaycastHit[] hits2 = Physics.RaycastAll(cameraPosition, direction + Vector3.right, distance, TargetLayer);
        RaycastHit[] hits3 = Physics.RaycastAll(cameraPosition, direction + Vector3.forward, distance, TargetLayer);
        RaycastHit[] hits4 = Physics.RaycastAll(cameraPosition, direction + Vector3.back, distance, TargetLayer);
        RaycastHit[] hits5 = Physics.RaycastAll(cameraPosition, direction + Vector3.left*2, distance, TargetLayer);
        RaycastHit[] hits6 = Physics.RaycastAll(cameraPosition, direction + Vector3.right*2, distance, TargetLayer);
        RaycastHit[] hits7 = Physics.RaycastAll(cameraPosition, direction + Vector3.forward*2, distance, TargetLayer);
        RaycastHit[] hits8 = Physics.RaycastAll(cameraPosition, direction + Vector3.back*2, distance, TargetLayer);
        RaycastHit[] hits9 = Physics.RaycastAll(cameraPosition, direction + Vector3.left*3, distance, TargetLayer);
        RaycastHit[] hits10 = Physics.RaycastAll(cameraPosition, direction + Vector3.right*3, distance, TargetLayer);
        RaycastHit[] hits11 = Physics.RaycastAll(cameraPosition, direction + Vector3.forward*3, distance, TargetLayer);
        RaycastHit[] hits12 = Physics.RaycastAll(cameraPosition, direction + Vector3.back*3, distance, TargetLayer);

        hits = MergeArrays(new RaycastHit[][] {hits, hits1,hits2,hits3,hits4,hits5,hits6,hits7,hits8,hits9,hits10,hits11,hits12});

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                currentlyHitRenderers.Add(renderer);

                // 원래의 Material 저장
                if (!originalMaterials.ContainsKey(renderer))
                {
                    originalMaterials[renderer] = renderer.materials;
                    // 투명 Material 적용
                }
                renderer.material = transparentMaterial;
                
                // Renderer를 투명화 처리된 목록에 추가
                if (!transparentRenderers.Contains(renderer))
                {
                    transparentRenderers.Add(renderer);
                }
            }
        }
    }
    void UnuseRenderer(){
        // 투명화가 필요 없어진 Renderer 복원
        for (int i = transparentRenderers.Count - 1; i >= 0; i--)
        {
            Renderer renderer = transparentRenderers[i];
            if (!currentlyHitRenderers.Contains(renderer))
            {
                RestoreRenderer(renderer);
                transparentRenderers.RemoveAt(i);
            }
        }
    }
    void RestoreRenderer(Renderer renderer)
    {
        if (originalMaterials.ContainsKey(renderer))
        {
            renderer.materials = originalMaterials[renderer]; // 원래의 Material로 복원
            originalMaterials.Remove(renderer);
        }
    }
}
