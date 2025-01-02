using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_SurviveInCircle : MonoBehaviour
{
    void Start(){
        Destroy(this.gameObject,20f);
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")){
            GetComponent<QuestSignal>().GoSignal();
        }
    }
}
