using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TypingSystem : MonoBehaviour
{
    public static TypingSystem _instance;
    
    [SerializeField]
    private float typingTimer = 0.08f;
    private float typingTimer_fast = 0.03f;
    
    private float typingTime;
    private string[] texts;
    private TextMeshProUGUI tmpSave;

    public static bool isDialogEnd;
    public bool isTypingEnd;
    private int dialogNumber = 0;

    private float timer;

    void Awake()
    {
        timer = typingTimer;
        typingTime = typingTimer;
    }
    
    public static TypingSystem Instance
    {
        get
        {
            if (_instance == null)
                return null;

            return _instance;
        }
        
    }

    public static void Init()
    {
        if (_instance)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
        _instance = new GameObject("[TypingSystem]").AddComponent<TypingSystem>();
        DontDestroyOnLoad(_instance.gameObject);
    }

    public void Typing(string[] dialogs, TextMeshProUGUI textObj, bool isClear = true)
    {
        isDialogEnd = false;
        texts = dialogs;
        tmpSave = textObj;
        if(isClear) 
            textObj.text = "";
        typingTime = typingTimer;
        if (dialogNumber < dialogs.Length)
        {
            char[] chars = dialogs[dialogNumber].ToCharArray();
            StartCoroutine(Typer(chars,textObj));
        }
        else
        {
            tmpSave.text = "";
            isDialogEnd = true;
            texts = null;
            tmpSave = null;
            dialogNumber = 0;
        }
    }

    public void GetInputDown()
    {
        if (texts != null)
        {
            if (isTypingEnd)
            {
                tmpSave.text = "";
                Typing(texts,tmpSave);
                timer = typingTime;
            }
            else
            {
                typingTime = typingTimer_fast;
            }
            
        }
        
    }
    
    public bool GetInputUp()
    { 
        typingTime = typingTimer;

        return true;
    }
    
    IEnumerator Typer(char[] chars, TextMeshProUGUI textObj)
    {
        int currentChar = 0;
        int charLength = chars.Length;
        typingTime = typingTimer;
        isTypingEnd = false;

        while (currentChar < charLength)
        {
            if (timer >= 0)
            {
                yield return null;
                timer -= Time.unscaledDeltaTime;
            }
            else
            {
                if (chars[currentChar] == '<')
                {
                    string richText = "";
                    while (true)
                    {
                        richText += chars[currentChar].ToString();
                        if (chars[currentChar] == '>')
                        {
                            currentChar++;
                            break;
                        }
                        currentChar++;
                    }

                    textObj.text += richText;
                }
                else
                {
                    textObj.text += chars[currentChar].ToString();
                    currentChar++;
                }
                timer = typingTime;
            }

            if (currentChar >= charLength)
            {
                isTypingEnd = true;
                dialogNumber++;
                if (texts.Length == dialogNumber)
                {
                    isDialogEnd = true;
                    texts = null;
                    tmpSave = null;
                    dialogNumber = 0;
                }
                yield break;
            }
            
        }
    }

}
