using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationContent_Image : MonoBehaviour
{
    public string imageName = "";

    Image image;

    public Sprite[] imageArray;
    LocalizationDataBase_Image localizationDataBase_Image;

    private void Awake()
    {
        image = GetComponent<Image>();

        if (localizationDataBase_Image == null) localizationDataBase_Image = Resources.Load("LocalizationDataBase_Image") as LocalizationDataBase_Image;
    }

    private void Start()
    {
        Sprite[] sp = localizationDataBase_Image.GetImage(imageName);

        if (sp == null || sp.Length == 0)
        {
            Debug.LogError($"[ReLoad] '{name}'�� ���� Sprite �迭�� �������� ����!");
            return;
        }

        imageArray = new Sprite[sp.Length];

        for (int i = 0; i < sp.Length; i++)
        {
            imageArray[i] = sp[i];
        }

        ReLoad();

        if (LocalizationManager.instance != null) LocalizationManager.instance.AddContent_Image(this);
    }

    public void ReLoad()
    {
        int langIndex = (int)GameStateManager.instance.Language - 1;

        if (langIndex < 0 || langIndex >= imageArray.Length)
        {
            Debug.LogWarning($"[ReLoad] �߸��� Language �ε���: {langIndex}, �⺻ �̹����� ��ü");
            langIndex = 1; // �⺻�� ����
        }

        if (imageArray[langIndex] != null)
        {
            image.sprite = imageArray[langIndex];
        }
        else
        {
            Debug.LogWarning($"[ReLoad] '{name}'�� Language {langIndex}�� �ش��ϴ� ��������Ʈ�� ����!");
            image.sprite = imageArray[1]; // �⺻������ ��ü
        }
    }
}
