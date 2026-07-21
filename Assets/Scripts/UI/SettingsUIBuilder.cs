using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 설정 화면을 동적으로 생성하고 표시합니다.
/// </summary>
public class SettingsUIBuilder : MonoBehaviour
{
    private Canvas _mainCanvas;
    private SettingsManager _settingsManager;

    private Slider _musicVolumeSlider;
    private Slider _sfxVolumeSlider;
    private Text _musicVolumeValueText;
    private Text _sfxVolumeValueText;
    private Text _soundThemeValueText;

    private void Start()
    {
        _mainCanvas = GetComponent<Canvas>();
        if (_mainCanvas == null)
        {
            Debug.LogError("SettingsUIBuilder: Canvas not found!");
            return;
        }

        _settingsManager = SettingsManager.Instance;
        BuildSettingsPanel();
    }

    private void BuildSettingsPanel()
    {
        GameObject settingsPanel = new GameObject("SettingsPanel");
        settingsPanel.transform.SetParent(_mainCanvas.transform, false);
        settingsPanel.SetActive(false);

        Image overlay = settingsPanel.AddComponent<Image>();
        overlay.color = new Color(0f, 0f, 0f, 0.6f);

        RectTransform settingsRect = settingsPanel.GetComponent<RectTransform>();
        settingsRect.anchorMin = Vector2.zero;
        settingsRect.anchorMax = Vector2.one;
        settingsRect.offsetMin = Vector2.zero;
        settingsRect.offsetMax = Vector2.zero;

        GameObject contentPanel = CreateContentPanel(settingsPanel.transform);
        CreateHeader(contentPanel.transform);
        CreateVolumeControl(contentPanel.transform, "배경음량", new Vector2(0, 120), _settingsManager.MusicVolume, OnMusicVolumeChanged, out _musicVolumeSlider, out _musicVolumeValueText);
        CreateVolumeControl(contentPanel.transform, "효과음량", new Vector2(0, 40), _settingsManager.SfxVolume, OnSfxVolumeChanged, out _sfxVolumeSlider, out _sfxVolumeValueText);
        CreateThemeControl(contentPanel.transform, new Vector2(0, -60));
        CreateCloseButton(contentPanel.transform, new Vector2(0, -180));
        RefreshThemeDisplay();
    }

    private GameObject CreateContentPanel(Transform parent)
    {
        GameObject contentPanel = new GameObject("SettingsContent");
        contentPanel.transform.SetParent(parent, false);

        Image contentBg = contentPanel.AddComponent<Image>();
        Sprite panel = Resources.Load<Sprite>("Sprites/UI_Panel_Element_59");
        if (panel != null)
        {
            contentBg.sprite = panel;
            contentBg.type = Image.Type.Simple;
            contentBg.color = Color.white;
        }
        else
        {
            FontHelper.ApplyDefaultUISprite(contentBg);
            contentBg.color = new Color(1f, 0.98f, 0.93f, 0.98f);
        }

        RectTransform contentRect = contentPanel.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(720, 520);
        contentRect.anchoredPosition = Vector2.zero;

        return contentPanel;
    }

    private void CreateHeader(Transform parent)
    {
        Text header = CreateText(parent, "SettingsHeader", "설정", 32, TextAnchor.UpperCenter);
        RectTransform headerRect = header.GetComponent<RectTransform>();
        headerRect.anchoredPosition = new Vector2(0, 220);
        headerRect.sizeDelta = new Vector2(520, 60);
    }

    private void CreateVolumeControl(Transform parent, string label, Vector2 position, float initialValue, UnityAction<float> onValueChanged, out Slider slider, out Text valueText)
    {
        Text labelText = CreateText(parent, label + "Label", label, 22, TextAnchor.MiddleLeft);
        labelText.rectTransform.anchoredPosition = new Vector2(-250, position.y + 20);

        slider = CreateSlider(parent, label + "Slider", position, initialValue, onValueChanged);
        valueText = CreateText(parent, label + "ValueText", (int)(initialValue * 100) + "%", 18, TextAnchor.MiddleRight);
        RectTransform valueRect = valueText.GetComponent<RectTransform>();
        valueRect.anchoredPosition = new Vector2(250, position.y + 20);
        valueRect.sizeDelta = new Vector2(120, 30);
    }

    private void CreateThemeControl(Transform parent, Vector2 position)
    {
        Text themeLabel = CreateText(parent, "SoundThemeLabel", "사운드 테마", 22, TextAnchor.MiddleLeft);
        themeLabel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250, position.y + 25);

        _soundThemeValueText = CreateText(parent, "SoundThemeValue", _settingsManager.GetCurrentThemeDisplayName(), 22, TextAnchor.MiddleCenter);
        RectTransform themeValueRect = _soundThemeValueText.GetComponent<RectTransform>();
        themeValueRect.anchoredPosition = new Vector2(0, position.y - 10);
        themeValueRect.sizeDelta = new Vector2(360, 40);

        Button prevButton = CreateSmallButton(parent, "ThemePrevButton", "〈", new Vector2(-180, position.y - 10), OnThemePreviousClicked);
        Button nextButton = CreateSmallButton(parent, "ThemeNextButton", "〉", new Vector2(180, position.y - 10), OnThemeNextClicked);
    }

    private void CreateCloseButton(Transform parent, Vector2 position)
    {
        Button closeButton = CreateButton(parent, "CloseSettingsButton", "닫기", new Vector2(180, 60), position, OnCloseClicked);
        Image closeImage = closeButton.GetComponent<Image>();
        closeImage.color = new Color(0.7f, 0.5f, 0.3f, 1f);
    }

    private Text CreateText(Transform parent, string name, string text, int fontSize, UnityEngine.TextAnchor alignment)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = FontHelper.GetDefaultFont();
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = new Color(0.32f, 0.22f, 0.14f, 1f);
        textComponent.raycastTarget = false;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(520, 40);
        textRect.anchoredPosition = Vector2.zero;
        return textComponent;
    }

    private Slider CreateSlider(Transform parent, string name, Vector2 position, float initialValue, UnityAction<float> onValueChanged)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);

        Image sliderBackground = sliderObj.AddComponent<Image>();
        FontHelper.ApplyDefaultUISprite(sliderBackground);
        sliderBackground.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.value = initialValue;
        slider.onValueChanged.AddListener(onValueChanged);

        slider.targetGraphic = sliderBackground;

        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(520, 40);
        sliderRect.anchoredPosition = new Vector2(0, position.y - 10);

        GameObject fillArea = new GameObject("FillArea");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1, 0.75f);
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        FontHelper.ApplyDefaultUISprite(fillImage);
        fillImage.color = new Color(1f, 0.74f, 0.28f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        slider.fillRect = fillRect;

        GameObject handleSlideArea = new GameObject("HandleSlideArea");
        handleSlideArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleSlideRect = handleSlideArea.AddComponent<RectTransform>();
        handleSlideRect.anchorMin = Vector2.zero;
        handleSlideRect.anchorMax = Vector2.one;
        handleSlideRect.offsetMin = Vector2.zero;
        handleSlideRect.offsetMax = Vector2.zero;

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleSlideArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        FontHelper.ApplyDefaultUISprite(handleImage);
        handleImage.color = new Color(1f, 1f, 1f, 1f);

        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(24, 24);
        handleRect.anchorMin = new Vector2(0, 0.5f);
        handleRect.anchorMax = new Vector2(0, 0.5f);
        handleRect.anchoredPosition = Vector2.zero;

        slider.handleRect = handleRect;
        slider.direction = Slider.Direction.LeftToRight;

        return slider;
    }

    private Button CreateButton(Transform parent, string name, string label, Vector2 size, Vector2 position, UnityAction onClick)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        Sprite btn = Resources.Load<Sprite>("Sprites/UI_Button_26");
        if (btn != null)
        {
            buttonImage.sprite = btn;
            buttonImage.type = Image.Type.Simple;
            buttonImage.color = Color.white;
        }
        else
        {
            FontHelper.ApplyDefaultUISprite(buttonImage);
            buttonImage.color = new Color(0.89f, 0.69f, 0.42f, 1f);
        }
        button.targetGraphic = buttonImage;

        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = label;
        buttonText.font = FontHelper.GetDefaultFont();
        buttonText.fontSize = 20;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = new Color(0.2f, 0.12f, 0.06f, 1f);
        buttonText.raycastTarget = false;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        button.onClick.AddListener(onClick);
        return button;
    }

    private Button CreateSmallButton(Transform parent, string name, string label, Vector2 position, UnityAction onClick)
    {
        return CreateButton(parent, name, label, new Vector2(60, 50), position, onClick);
    }

    private void OnMusicVolumeChanged(float newValue)
    {
        if (_musicVolumeValueText != null)
            _musicVolumeValueText.text = Mathf.RoundToInt(newValue * 100) + "%";

        _settingsManager.SetMusicVolume(newValue);
    }

    private void OnSfxVolumeChanged(float newValue)
    {
        if (_sfxVolumeValueText != null)
            _sfxVolumeValueText.text = Mathf.RoundToInt(newValue * 100) + "%";

        _settingsManager.SetSfxVolume(newValue);
    }

    private void OnThemePreviousClicked()
    {
        SoundTheme current = _settingsManager.SelectedTheme;
        SoundTheme previous = GetThemeWithOffset(current, -1);
        _settingsManager.SetSoundTheme(previous);
        RefreshThemeDisplay();
    }

    private void OnThemeNextClicked()
    {
        SoundTheme current = _settingsManager.SelectedTheme;
        SoundTheme next = GetThemeWithOffset(current, 1);
        _settingsManager.SetSoundTheme(next);
        RefreshThemeDisplay();
    }

    private SoundTheme GetThemeWithOffset(SoundTheme current, int offset)
    {
        SoundTheme[] themeOptions =
        {
            SoundTheme.Korea,
            SoundTheme.China,
            SoundTheme.Japan,
            SoundTheme.Vietnam,
            SoundTheme.Kyrgyzstan,
            SoundTheme.Random
        };

        int currentIndex = System.Array.IndexOf(themeOptions, current);
        int nextIndex = (currentIndex + offset + themeOptions.Length) % themeOptions.Length;
        return themeOptions[nextIndex];
    }

    private void RefreshThemeDisplay()
    {
        if (_soundThemeValueText != null)
        {
            _soundThemeValueText.text = _settingsManager.GetCurrentThemeDisplayName();
        }
    }

    private void OnCloseClicked()
    {
        UIManager.Instance.HidePanel("SettingsPanel");
    }
}
