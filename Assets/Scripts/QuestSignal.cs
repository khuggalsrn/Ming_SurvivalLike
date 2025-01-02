using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestSignal : MonoBehaviour
{
    public event Action<bool> OnQuestSignal; // 신호 이벤트
    bool signal = false;
    void Start(){
        Invoke("GoSignal",60f);
    }
    void OnDestroy()
    {
        signal = true;
        GoSignal();
    }
    public void GoSignal(){
        OnQuestSignal?.Invoke(signal);
    }
}
