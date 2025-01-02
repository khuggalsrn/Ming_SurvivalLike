using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
[System.Serializable]
public class SoundInfo
{
    public string name; // 
    public AudioClip clip; // 
}
public class SoundManager : MonoBehaviour
{
    public List<SoundInfo> Sources;
    [SerializeField] private List<AudioSource> AudioSources;
    private int NextPlayi = 0;
    public int MaxAudioNum = 10;
    Canvas OptionWindow;
    Text VolumeText;
    public void CallAudio(string name)
    {
        foreach (var sound in Sources)
        {
            if (sound.name == name)
            {
                PlayAudio(sound.clip);
            }
        }
    }
    void PlayAudio(AudioClip sound)
    {
        AudioSources[NextPlayi].clip = sound;
        AudioSources[NextPlayi].Play();
        NextPlayi = (NextPlayi + 1) % MaxAudioNum;
    }
    static private SoundManager instance = null; // 싱글톤
    public static SoundManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    //
    //
    //
    //
    //
    //
    void Awake()
    {
        if (null == instance)
        {
            AudioSources = new List<AudioSource>();
            instance = this;
            for (int i = 0; i < MaxAudioNum; i++)
            {
                GameObject audioSourceObject = new GameObject($"AudioSource_{i + 1}");

                // 새 GameObject를 현재 오브젝트의 자식으로 설정
                audioSourceObject.transform.parent = this.transform;

                // AudioSource 컴포넌트를 추가
                AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();

                // 리스트에 추가
                AudioSources.Add(audioSource);

                // AudioSources.Add(gameObject.AddComponent<AudioSource>());
                AudioSources[i].volume = 0.5f;
            }
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            if (!OptionWindow)
            {
                OptionWindow = Instantiate(UI_Manager.Instance.Option, transform);
                OptionWindow.GetComponentsInChildren<Button>()[0].onClick.AddListener(()=>SetVolume(0.1f));
                OptionWindow.GetComponentsInChildren<Button>()[1].onClick.AddListener(()=>SetVolume(-0.1f));
                VolumeText = OptionWindow.GetComponentsInChildren<Text>()[0];
                SetVolume(0);
            }
            if (Time.timeScale == 0)
            {
                OptionWindow.gameObject.SetActive(true);
            }
            else
            {
                OptionWindow.gameObject.SetActive(false);
            }
        }
    }
    private void SetVolume(float value)
    {
        foreach (AudioSource audioSource in AudioSources)
        {
            if (audioSource != null) // AudioSource가 null인지 확인
            {
                audioSource.volume +=value; // Scrollbar 값에 따라 볼륨 설정
                VolumeText.text =  Mathf.RoundToInt(audioSource.volume*10f).ToString();
            }
        }
    }
}
