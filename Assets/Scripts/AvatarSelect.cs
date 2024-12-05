using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelect : MonoBehaviour
{
    [SerializeField] Canvas UI_AvatarSelect;
    [SerializeField] List<GameObject> Avatars;
    List<Button> avatarSelectButtons = new List<Button>();
    Canvas CurUI_AvatarSelect;
    GameObject avatar = null;
    public IReadOnlyList<Button> AvatarSelectButtons => avatarSelectButtons;
    //
    //
    //
    //
    //
    //
    void Start(){
        
        CurUI_AvatarSelect = Instantiate(UI_AvatarSelect);
        var a = CurUI_AvatarSelect.transform.GetComponentInChildren<Transform>();
        foreach (Transform item in a)
        {
            Debug.Log(item);
            avatarSelectButtons.Add(item.GetComponent<Button>());
        }
        for(int i = 0; i < AvatarSelectButtons.Count; i++)
        {
            int t = i;
            avatarSelectButtons[i].onClick.AddListener(()=>AvatarMake(t));
            avatarSelectButtons[i].onClick.AddListener(()=>BtnOff(avatarSelectButtons));
        }
        Time.timeScale = 0;
    }
    void AvatarMake(int i){
        Debug.Log("아바타만들기시도"+i);
        if (i>=Avatars.Count) return;
        Debug.Log("아바타만듦");
        avatar = Instantiate(Avatars[i], transform, false);
        avatar.transform.position += new Vector3(0,2,0);
    }
    void BtnOff(IReadOnlyList<Button> list){
        foreach(var Btn in list){
            Btn.gameObject.SetActive(false);
        }
        Time.timeScale = 1;
        DestroyImmediate(this);
    }

}
