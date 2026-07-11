using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ?쒗넗由ъ뼹 UI 鍮뚮뜑
/// Main ?ъ쓽 Canvas???쒗넗由ъ뼹 UI ?숈쟻 ?앹꽦
/// </summary>
public class TutorialUIBuilder : MonoBehaviour
{
    private Canvas _mainCanvas;

    private void Start()
    {
        _mainCanvas = GetComponent<Canvas>();
        if (_mainCanvas == null)
        {
            Debug.LogError("TutorialUIBuilder: Canvas not found!");
            return;
        }

        BuildTutorialUI();
    }

    private void BuildTutorialUI()
    {
        // ?쒗넗由ъ뼹 ?⑤꼸 (諛섑닾紐?諛곌꼍 + ???諛뺤뒪)
        GameObject tutorialObj = new GameObject("TutorialPanel");
        tutorialObj.transform.SetParent(_mainCanvas.transform, false);

        Image tutorialBg = tutorialObj.AddComponent<Image>();
        tutorialBg.color = new Color(0, 0, 0, 0.3f);

        RectTransform tutorialRect = tutorialObj.GetComponent<RectTransform>();
        tutorialRect.anchorMin = Vector2.zero;
        tutorialRect.anchorMax = Vector2.one;
        tutorialRect.offsetMin = Vector2.zero;
        tutorialRect.offsetMax = Vector2.zero;

        // ???諛뺤뒪 (?섎떒)
        GameObject dialogObj = new GameObject("DialogBox");
        dialogObj.transform.SetParent(tutorialObj.transform, false);

        Image dialogImage = dialogObj.AddComponent<Image>();
        dialogImage.color = new Color(1f, 1f, 1f, 0.95f);

        RectTransform dialogRect = dialogObj.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0, 0);
        dialogRect.anchorMax = new Vector2(1, 0.25f);
        dialogRect.offsetMin = Vector2.zero;
        dialogRect.offsetMax = Vector2.zero;

        // ?띿뒪??
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(dialogObj.transform, false);

        Text tutorialText = textObj.AddComponent<Text>();
        tutorialText.text = "튜토리얼 텍스트";
        tutorialText.font = FontHelper.GetDefaultFont();
        tutorialText.fontSize = 20;
        tutorialText.alignment = TextAnchor.MiddleLeft;
        tutorialText.color = new Color(0.42f, 0.27f, 0.14f, 1f);
        tutorialText.horizontalOverflow = HorizontalWrapMode.Wrap;
        tutorialText.verticalOverflow = VerticalWrapMode.Truncate;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(20, 20);
        textRect.offsetMax = new Vector2(-120, -20);

        // Next 踰꾪듉
        GameObject nextBtnObj = new GameObject("NextButton");
        nextBtnObj.transform.SetParent(dialogObj.transform, false);

        Button nextButton = nextBtnObj.AddComponent<Button>();
        Image nextBtnImage = nextBtnObj.AddComponent<Image>();
        nextBtnImage.color = new Color(1f, 0.63f, 0.26f, 1f);

        RectTransform nextBtnRect = nextBtnObj.GetComponent<RectTransform>();
        nextBtnRect.sizeDelta = new Vector2(100, 50);
        nextBtnRect.anchorMin = new Vector2(1, 0.5f);
        nextBtnRect.anchorMax = new Vector2(1, 0.5f);
        nextBtnRect.anchoredPosition = new Vector2(-20, 0);

        // Next 踰꾪듉 ?띿뒪??
        GameObject nextTextObj = new GameObject("Text");
        nextTextObj.transform.SetParent(nextBtnObj.transform, false);

        Text nextText = nextTextObj.AddComponent<Text>();
        nextText.text = "Next";
        nextText.font = FontHelper.GetDefaultFont();
        nextText.fontSize = 16;
        nextText.fontStyle = FontStyle.Bold;
        nextText.alignment = TextAnchor.MiddleCenter;
        nextText.color = Color.white;

        RectTransform nextTextRect = nextTextObj.GetComponent<RectTransform>();
        nextTextRect.anchorMin = Vector2.zero;
        nextTextRect.anchorMax = Vector2.one;
        nextTextRect.offsetMin = Vector2.zero;
        nextTextRect.offsetMax = Vector2.zero;

        // Skip 踰꾪듉
        GameObject skipBtnObj = new GameObject("SkipButton");
        skipBtnObj.transform.SetParent(dialogObj.transform, false);

        Button skipButton = skipBtnObj.AddComponent<Button>();
        Image skipBtnImage = skipBtnObj.AddComponent<Image>();
        skipBtnImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        RectTransform skipBtnRect = skipBtnObj.GetComponent<RectTransform>();
        skipBtnRect.sizeDelta = new Vector2(100, 50);
        skipBtnRect.anchorMin = new Vector2(1, 0.5f);
        skipBtnRect.anchorMax = new Vector2(1, 0.5f);
        skipBtnRect.anchoredPosition = new Vector2(-130, 0);

        // Skip 踰꾪듉 ?띿뒪??
        GameObject skipTextObj = new GameObject("Text");
        skipTextObj.transform.SetParent(skipBtnObj.transform, false);

        Text skipText = skipTextObj.AddComponent<Text>();
        skipText.text = "Skip";
        skipText.font = FontHelper.GetDefaultFont();
        skipText.fontSize = 14;
        skipText.fontStyle = FontStyle.Bold;
        skipText.alignment = TextAnchor.MiddleCenter;
        skipText.color = Color.black;

        RectTransform skipTextRect = skipTextObj.GetComponent<RectTransform>();
        skipTextRect.anchorMin = Vector2.zero;
        skipTextRect.anchorMax = Vector2.one;
        skipTextRect.offsetMin = Vector2.zero;
        skipTextRect.offsetMax = Vector2.zero;

        // TutorialManager 異붽?
        TutorialManager tutorialManager = tutorialObj.AddComponent<TutorialManager>();
        tutorialManager.enabled = true;

        // 李몄“ ?좊떦
        tutorialManager.GetType().GetField("_tutorialPanel", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(tutorialManager, tutorialObj);
        tutorialManager.GetType().GetField("_tutorialText", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(tutorialManager, tutorialText);
        tutorialManager.GetType().GetField("_nextButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(tutorialManager, nextButton);
        tutorialManager.GetType().GetField("_skipButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(tutorialManager, skipButton);

        Debug.Log("Tutorial UI built successfully");
    }
}

