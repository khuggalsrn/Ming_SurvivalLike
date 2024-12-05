using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    void HandleCeilingTransparency(LayerMask TargetLayer, Material transparentMaterial)
    {
        // 카메라와 캐릭터 사이에 Ray를 쏨
        Vector3 cameraPosition = transform.position;
        Vector3 playerPosition = player.position;
        Vector3 direction = playerPosition - cameraPosition;
        float distance = Vector3.Distance(cameraPosition, playerPosition)-1f;


        // Raycast로 감지
        RaycastHit[] hits = Physics.RaycastAll(cameraPosition, direction, distance, TargetLayer);
        RaycastHit[] hits1 = Physics.RaycastAll(cameraPosition, direction + Vector3.left, distance, TargetLayer);
        RaycastHit[] hits2 = Physics.RaycastAll(cameraPosition, direction + Vector3.right, distance, TargetLayer);
        RaycastHit[] hits3 = Physics.RaycastAll(cameraPosition, direction + Vector3.up, distance, TargetLayer);
        RaycastHit[] hits4 = Physics.RaycastAll(cameraPosition, direction + Vector3.down, distance, TargetLayer);
        hits = hits.Concat(hits1).Concat(hits2).Concat(hits3).Concat(hits4).ToArray();

       

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
