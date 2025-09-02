using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource musicAudio;

    public AudioClip[] musicArray;

    public AudioSource clickAudio;
    public AudioSource[] sfxAudio;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� �̵� �Ŀ��� ����
        }
        else
        {
            Destroy(gameObject); // ���� �ν��Ͻ��� ������ �� ��ü �ı�
        }
    }

    private void Start()
    {
        Invoke("Initialize", 0.2f);
    }

    void Initialize()
    {
        SetSfx(GameStateManager.instance.SfxValue);
    }

    public void PlayBGM()
    {
        StartCoroutine(PlayList());
    }

    public void StopBGM()
    {
        musicAudio.Stop();
    }

    IEnumerator PlayList()
    {
        while (true)
        {
            if (!musicAudio.isPlaying)
            {
                musicAudio.Stop();
                musicAudio.clip = musicArray[Random.Range(0, musicArray.Length)];
                musicAudio.Play();
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    public void SetMusic(float value)
    {
        musicAudio.volume = value;
    }

    public void SetSfx(float value)
    {
        clickAudio.volume = value;
    }

    public void PlayBGM(GameBGMType type)
    {
        //if (!GameStateManager.instance.Music) return;

        string targetName = type.ToString();

        // 이미 같은 BGM이 재생 중이면 무시
        if (musicAudio.isPlaying && musicAudio.clip != null && musicAudio.clip.name.Equals(targetName))
        {
            return;
        }

        for (int i = 0; i < musicArray.Length; i++)
        {
            if (musicArray[i].name.Equals(targetName))
            {
                musicAudio.Stop();
                musicAudio.volume = GameStateManager.instance.MusicValue;
                musicAudio.clip = musicArray[i];
                musicAudio.Play();
                break; // 찾았으니 루프 종료
            }
        }
    }


    public void PlaySFX(GameSfxType type)
    {
        if (!GameStateManager.instance.Sfx) return;

        for (int i = 0; i < sfxAudio.Length; i++)
        {
            if (sfxAudio[i].name.Equals(type.ToString()))
            {
                sfxAudio[i].volume = GameStateManager.instance.SfxValue;
                sfxAudio[i].Play();
            }
        }
    }

    public void StopSFX()
    {
        for (int i = 0; i < sfxAudio.Length; i++)
        {
            sfxAudio[i].Stop();
        }
    }

    public void ResetBGM()
    {
        musicAudio.Stop();
        StopAllCoroutines();
        StartCoroutine(PlayList());
    }
}