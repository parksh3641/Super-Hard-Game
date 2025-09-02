using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationContent : MonoBehaviour
{
    public bool isOutlineBlack = false;
    public bool isItalic = true;

    public Text localizationText;
    public TMP_Text localizationTMPText;

    public string localizationName = "";
    public string plusText = "";

    public bool purchase = false;

    private void Awake()
    {
        if (localizationText == null && TryGetComponent<Text>(out var textComponent))
        {
            localizationText = textComponent;
            localizationText.color = Color.white;

            Outline outline = textComponent.gameObject.AddComponent<Outline>();

            if(isOutlineBlack)
            {
                outline.effectColor = new Color32(1, 1, 1, 255);
            }
            else
            {
                outline.effectColor = new Color32(172, 104, 92, 255);
            }

            // 아웃라인 두께 설정 (x, y 값 조절)
            outline.effectDistance = new Vector2(2f, -2f);
            localizationText.fontStyle = FontStyle.Bold | FontStyle.Italic;

            if (!isItalic)
            {
                localizationText.fontStyle = FontStyle.Bold;
            }
        }

        if (localizationTMPText == null && TryGetComponent<TMP_Text>(out var tmpTextComponent))
        {
            localizationTMPText = tmpTextComponent;
            localizationTMPText.color = Color.white;

            if(isOutlineBlack)
            {
                localizationTMPText.outlineColor = new Color32(1, 1, 1, 255);
            }
            else
            {
                localizationTMPText.outlineColor = new Color32(172, 104, 92, 255); // 빨간색
            }

            localizationTMPText.outlineWidth = 0.2f;
            localizationTMPText.fontStyle = FontStyles.Bold | FontStyles.Italic;

            if (!isItalic)
            {
                localizationTMPText.fontStyle = FontStyles.Bold;
            }
        }
    }

    private void Start()
    {
        if (localizationName.Length != 0)
        {
            ReLoad();

            if (LocalizationManager.instance != null) LocalizationManager.instance.AddContent(this);
        }
    }

    public void ReLoad()
    {
        if (localizationName.Length > 0 && localizationText != null)
        {
            localizationText.text = "";

            if (!purchase)
            {
                localizationText.text = LocalizationManager.instance.GetString(localizationName);
            }
            else
            {
#if UNITY_IOS || UNITY_EDITOR_OSX
                localizationText.text += LocalizationManager.instance.GetString(localizationName + "_IOS");
#else
                localizationText.text += LocalizationManager.instance.GetString(localizationName + "_AOS");
#endif
            }

            if (plusText.Length > 0)
            {
                localizationText.text += plusText;
            }
        }

        if (localizationName.Length > 0 && localizationTMPText != null)
        {
            localizationTMPText.text = "";

            if (!purchase)
            {
                localizationTMPText.text = LocalizationManager.instance.GetString(localizationName);
            }
            else
            {
#if UNITY_IOS || UNITY_EDITOR_OSX
                localizationTMPText.text += LocalizationManager.instance.GetString(localizationName + "_IOS");
#else
                localizationTMPText.text += LocalizationManager.instance.GetString(localizationName + "_AOS");
#endif
            }

            if (plusText.Length > 0)
            {
                localizationTMPText.text += plusText;
            }
        }
    }

    public Text GetText()
    {
        return localizationText;
    }

    public TMP_Text GetTMPText()
    {
        return localizationTMPText;
    }
}