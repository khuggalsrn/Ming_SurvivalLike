using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererOpt : MonoBehaviour
{
    SkinnedMeshRenderer myrenderer;
    Camera mainCamera;
    void Start()
    {
        myrenderer = GetComponent<SkinnedMeshRenderer>();
        mainCamera = Camera.main;
    }
    void FixedUpdate()
    {
        // if (myrenderer == null || mainCamera == null)
        //     return;

        // Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        // if (GeometryUtility.TestPlanesAABB(planes, myrenderer.bounds))
        // {
        //     if (!myrenderer.enabled)
        //     {
        //         myrenderer.enabled = true;
        //         // Debug.Log($"{gameObject.name} became visible!");
        //     }
        // }
        // else
        // {
        //     if (myrenderer.enabled)
        //     {
        //         myrenderer.enabled = false;
        //         // Debug.Log($"{gameObject.name} became invisible!");
        //     }
        // }
    }
    // void OnBecameInvisible()
    // {
    //     Debug.Log(gameObject.name + "없어서 사라짐");
    //     myrenderer.enabled = false; // 화면에서 벗어나면 비활성화
    // }

    // void OnBecameVisible()
    // {
    //     Debug.Log(gameObject.name + "있어서 보임");
    //     myrenderer.enabled = true; // 화면에 들어오면 다시 활성화
    // }
}
