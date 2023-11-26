using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

public class EndingEvent : ITalkingEvent
{
    private string _sceneName;
    private GameObject _player;
    private GameObject _kennel;
    private GameObject _upWall;
    private GameObject _observer;
    private GameObject _boss;
    private GameObject _sceneChangeImage;

    private SpriteRenderer _kennelRenderer;
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFramingTransposer _cinemachineFramingTransposer;
    private CinemachineConfiner _cinemachineConfiner;

    
    private BezierTransform _observerBezier;
    private BezierTransform _playerBezier;
    
    private Vector2 _kennelPos;
    private Vector2 _kennelEnd;

    private Transform startPos;
    private PolygonCollider2D _confiner;

    private PlayerAnimationController _playerAnimationController;
    
    protected List<Dictionary<string, object>> _eventTexts;
    protected TalkingPanelInfo _playerPanel;
    protected TalkingPanelInfo _targetPanel;
    protected string _scriptPath = "EventTextScript/";
    protected  List<string> _comments;
    protected int _textCount;
    
    public async UniTask OnEventBefore()
    { 
        _scriptPath += "Ending/OpenEnd1";
        _eventTexts = CSVReader.Read(_scriptPath);
        _comments = new List<string>();
        
        for (int i = 0; i < _eventTexts.Count; i++)
        {
            _comments.Add(_eventTexts[i][EventTextType.Content.ToString()].ToString());
        }
        
        await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        _virtualCamera = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        _cinemachineFramingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _cinemachineConfiner = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineConfiner>();
        _confiner = GameObject.Find("Confiner").GetComponent<PolygonCollider2D>();
        _cinemachineConfiner.m_BoundingShape2D = null;
        _cinemachineFramingTransposer.m_YDamping = 0;
        
        _sceneChangeImage = GameObject.Find("SceneChangeImage");
        _upWall = GameObject.Find("UpWall");
        _boss = GameObject.FindWithTag("Boss");
        _boss.SetActive(false);
        InputManager.Instance.DisableMainGameAction();
        InputManager.Instance.InitTalkEventAction();

        startPos = GameObject.FindWithTag("ThemeStart").transform;
        
        _player = GameObject.FindWithTag("Player");
        _observer = GameObject.FindWithTag("Observer");
        
        _observerBezier = _observer.GetComponent<BezierTransform>();
        _playerBezier = _player.GetComponent<BezierTransform>();
        
        _playerPanel = GameObject.FindGameObjectWithTag("Player").GetComponent<TalkingPanelInfo>();
        _targetPanel = GameObject.FindWithTag("Observer").GetComponent<TalkingPanelInfo>();
        _kennel = GameObject.FindWithTag("Kennel");
        _kennelRenderer = _kennel.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        _kennelPos = _kennel.transform.position;

        _playerAnimationController = _player.GetComponent<PlayerAnimationController>();

        _playerBezier.simulated = true;
        _playerBezier.enabled = false;
        _observerBezier.simulated = true;
        _observerBezier.enabled = false;
        

        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        
        _playerAnimationController.SetState(new PAniState()
        {
            State = EPCAniState.Falling_Dash,
            Rotation = Quaternion.Euler(0,0,0),
            Restart = true
        });
        

    }

    public async UniTask OnEvent()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        _player.transform.rotation = quaternion.Euler(0, 0, 0);
        await UniTask.WaitUntil(() => _player.transform.position.y < startPos.position.y + 5f);
        _cinemachineFramingTransposer.m_YDamping = 1;
        _cinemachineConfiner.m_BoundingShape2D = _confiner;
        _playerAnimationController.SetState(new PAniState()
        {
            State = EPCAniState.Landing_Dash,
            Rotation = Quaternion.Euler(0,180,0),
            Restart = true
        });

        await UniTask.Delay(TimeSpan.FromSeconds(0.7f));

        await MoveToPosition(_player, new Vector2(_player.transform.position.x + 5, 0),0.1f);
        
        Color kennelColor = _kennelRenderer.color;
        
        
        while (_kennelRenderer.color.a >= 0)
        {
            _kennelRenderer.color = new Color(kennelColor.r, kennelColor.g, kennelColor.b, _kennelRenderer.color.a - 0.05f);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        _kennel.SetActive(false);

        InputAction action = InputManager.GetTalkEventAction("NextText");
        if (action != null)
        {
            string[] contents = _comments.ToArray();
            while (_textCount != _comments.Count)
            {
                string target = _eventTexts[_textCount][EventTextType.Target.ToString()].ToString();
                Talk(contents,target);
                await UniTask.WaitUntil(() => TypingSystem.Instance.isTypingEnd);
                SetEndbutton(target);
                await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                ClosePanel(target);

                if (_textCount == 2)
                {
                    _observerBezier.enabled = true;
                    _playerBezier.enabled = true;
                    _observerBezier.startAnimation();
                    await UniTask.Delay(TimeSpan.FromSeconds(0.23f));
                    _playerBezier.startAnimation();
                    await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
                }
            }
            
            EventFadeChanger.Instance.FadeIn(2.0f);
            
            await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1.0f);
            
            _observer.SetActive(false);
            _boss.SetActive(true);
            
            EventFadeChanger.Instance.FadeOut(1.0f);
            
            await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha <= 0f);
            
        }
        
        await UniTask.Yield();
        
    }
    
    public async UniTask OnEventEnd()
    {
        
        _playerPanel._panel.SetActive(false);
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        
        _cinemachineFramingTransposer.m_YDamping = 1;
        
        startPos.gameObject.SetActive(false);
        
        _boss.SendMessage("EngaugeBoss",SendMessageOptions.DontRequireReceiver);
        
        InputManager.Instance.DisableTalkEventAction();
        InputManager.Instance.InitMainGameAction();
        
    }

    public async UniTask MoveToPosition(GameObject target, Vector2 posistion, float speed)
    {
        Vector2 dir = target.transform.position.x - posistion.x > 0 ? Vector2.left : Vector2.right;
        
        float fliped = dir.x > 0 ? 180 : 0;
        
        _playerAnimationController.SetState(new PAniState()
        {
            State = EPCAniState.Run,
            Rotation = Quaternion.Euler(0,fliped,0),
            Restart = true
        });
        while (Mathf.Abs(target.transform.position.x - posistion.x) >= 0.1f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        target.transform.Rotate(0,0,0);
        _playerAnimationController.SetState(new PAniState()
        {
            State = EPCAniState.Idle,
            Rotation = Quaternion.identity,
            Restart = false
        });
    }

    public bool IsInvalid()
    {
        return true;
    }
    
    void Talk(string[] contents, string target)
    {
        _textCount++;
        switch (target)
        {
            case "Player" : 
                _playerPanel._panel.SetActive(true);
                _playerPanel._endButton.SetActive(false);
                if(_playerPanel._eventText.TryGetComponent(out TextMeshProUGUI playerComponent)) 
                    TypingSystem.Instance.Typing(contents,playerComponent);
                break;
            case "Observer" : 
                _targetPanel._panel.SetActive(true);
                _targetPanel._endButton.SetActive(false);
                if(_targetPanel._eventText.TryGetComponent(out TextMeshProUGUI observerComponent)) 
                    TypingSystem.Instance.Typing(contents,observerComponent);
                break;
                
        }
    }

    void SetEndbutton(string target)
    {
        switch (target)
        {
            case "Player" : 
                _playerPanel._endButton.SetActive(true);
                break;
            case "Observer" : 
                _targetPanel._endButton.SetActive(true);
                break;
                
        }
    }

    void ClosePanel(string target)
    {
        switch (target)
        {
            case "Player" : 
                _playerPanel._panel.SetActive(false);
                break;
            case "Observer" : 
                _targetPanel._panel.SetActive(false);
                break;
                
        }
    }
}
