using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타이틀 화면 UI 빌더
/// 타이틀 씬의 모든 UI 동적 생성
/// </summary>
public class TitleScreenUIBuilder : MonoBehaviour
{
    private Canvas _mainCanvas;
    private TitleScreenManager _titleScreenManager;

    private void Start()
    {
        _mainCanvas = GetComponent<Canvas>();
        if (_mainCanvas == null)
        {
            Debug.LogError("TitleScreenUIBuilder: Canvas not found!");
            return;
        }

        BuildUI();
    }

    private void BuildUI()
    {
        // 배경
        CreateBackground();

        // 타이틀 텍스트
        CreateTitleText();

        // 메인 버튼들
        GameObject mainButtonPanel = CreateMainButtonPanel();

        // 이름 입력 패널 (처음에는 숨김)
        GameObject nameInputPanel = CreateNameInputPanel();

        // TitleScreenManager 추가
        _titleScreenManager = _mainCanvas.gameObject.AddComponent<TitleScreenManager>();
        AssignUIReferences(mainButtonPanel, nameInputPanel);

        Debug.Log("Title Screen UI built successfully");
    }

    private void CreateBackground()
    {
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(_mainCanvas.transform, false);

        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.99f, 0.98f, 0.96f, 1f);

        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
    }

    private void CreateTitleText()
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(_mainCanvas.transform, false);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "xx다방\n(v0.3.0)";
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 50;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.sizeDelta = new Vector2(600, 200);
        titleRect.anchoredPosition = Vector2.zero;
    }

    private GameObject CreateMainButtonPanel()
    {
        GameObject panelObj = new GameObject("MainButtonPanel");
        panelObj.transform.SetParent(_mainCanvas.transform, false);

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = Color.clear;

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.3f);
        panelRect.anchorMax = new Vector2(0.5f, 0.3f);
        panelRect.sizeDelta = new Vector2(400, 300);
        panelRect.anchoredPosition = Vector2.zero;

        // 새 게임 버튼
        CreateMainButton(panelObj, "StartButton", "새 게임", 0, 3);

        // 계속하기 버튼
        CreateMainButton(panelObj, "ContinueButton", "계속하기", 1, 3);

        // 설정 버튼
        CreateMainButton(panelObj, "SettingsButton", "설정", 2, 3);

        return panelObj;
    }

    private void CreateMainButton(GameObject parent, string name, string label, int index, int totalCount)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);

        Button button = btnObj.AddComponent<Button>();
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(1f, 0.63f, 0.26f, 1f);

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(300, 70);
        btnRect.anchoredPosition = new Vector2(0, -index * 90);

        // 버튼 텍스트
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);

        Text btnText = textObj.AddComponent<Text>();
        btnText.text = label;
        btnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        btnText.fontSize = 24;
        btnText.fontStyle = FontStyle.Bold;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    private GameObject CreateNameInputPanel()
    {
        GameObject panelObj = new GameObject("NameInputPanel");
        panelObj.transform.SetParent(_mainCanvas.transform, false);
        panelObj.SetActive(false);

        // 반투명 배경 (모달)
        Image panelBg = panelObj.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.5f);

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 입력 폼 (흰색 박스)
        GameObject formObj = new GameObject("Form");
        formObj.transform.SetParent(panelObj.transform, false);

        Image formImage = formObj.AddComponent<Image>();
        formImage.color = new Color(1f, 1f, 1f, 0.95f);

        RectTransform formRect = formObj.GetComponent<RectTransform>();
        formRect.sizeDelta = new Vector2(400, 300);
        formRect.anchorMin = new Vector2(0.5f, 0.5f);
        formRect.anchorMax = new Vector2(0.5f, 0.5f);
        formRect.anchoredPosition = Vector2.zero;

        // 제목
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(formObj.transform, false);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "캐릭터 이름";
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 24;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.42f, 0.27f, 0.14f, 1f);

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 100);
        titleRect.sizeDelta = new Vector2(350, 40);

        // 입력 필드
        GameObject inputObj = new GameObject("InputField");
        inputObj.transform.SetParent(formObj.transform, false);

        Image inputBg = inputObj.AddComponent<Image>();
        inputBg.color = Color.white;

        InputField inputField = inputObj.AddComponent<InputField>();
        inputField.text = "orangeCat";
        inputField.characterValidation = InputField.CharacterValidation.Alphanumeric;
        inputField.characterLimit = 20;

        RectTransform inputRect = inputObj.GetComponent<RectTransform>();
        inputRect.sizeDelta = new Vector2(300, 50);
        inputRect.anchoredPosition = new Vector2(0, 40);

        // 입력 필드의 텍스트 컴포넌트
        GameObject inputTextObj = new GameObject("Text");
        inputTextObj.transform.SetParent(inputObj.transform, false);

        Text inputText = inputTextObj.AddComponent<Text>();
        inputText.text = "orangeCat";
        inputText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        inputText.fontSize = 18;
        inputText.alignment = TextAnchor.MiddleLeft;

        RectTransform inputTextRect = inputTextObj.GetComponent<RectTransform>();
        inputTextRect.anchorMin = Vector2.zero;
        inputTextRect.anchorMax = Vector2.one;
        inputTextRect.offsetMin = new Vector2(10, 0);
        inputTextRect.offsetMax = new Vector2(-10, 0);

        inputField.textComponent = inputText;

        // Confirm 버튼
        CreateNameInputButton(formObj, "ConfirmButton", "확인", -75);

        // Skip 버튼
        CreateNameInputButton(formObj, "SkipButton", "기본값 사용", -150);

        return panelObj;
    }

    private void CreateNameInputButton(GameObject parent, string name, string label, float yPos)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);

        Button button = btnObj.AddComponent<Button>();
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(1f, 0.63f, 0.26f, 1f);

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(140, 50);
        btnRect.anchoredPosition = new Vector2(0, yPos);

        // 버튼 텍스트
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);

        Text btnText = textObj.AddComponent<Text>();
        btnText.text = label;
        btnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        btnText.fontSize = 16;
        btnText.fontStyle = FontStyle.Bold;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    private void AssignUIReferences(GameObject mainButtonPanel, GameObject nameInputPanel)
    {
        Button startButton = mainButtonPanel.transform.Find("StartButton").GetComponent<Button>();
        Button continueButton = mainButtonPanel.transform.Find("ContinueButton").GetComponent<Button>();
        Button settingsButton = mainButtonPanel.transform.Find("SettingsButton").GetComponent<Button>();

        InputField nameInputField = nameInputPanel.transform.Find("Form/InputField").GetComponent<InputField>();
        Button nameConfirmButton = nameInputPanel.transform.Find("Form/ConfirmButton").GetComponent<Button>();
        Button nameSkipButton = nameInputPanel.transform.Find("Form/SkipButton").GetComponent<Button>();

        _titleScreenManager.SetUIReferences(
            startButton,
            continueButton,
            settingsButton,
            nameInputPanel,
            nameInputField,
            nameConfirmButton,
            nameSkipButton);
    }
}
