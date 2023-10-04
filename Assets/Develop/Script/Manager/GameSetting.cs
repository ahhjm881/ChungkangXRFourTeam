using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetting : MonoBehaviour
{
    public const bool VER_CASE_1 = true;
    
    
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Init()
    {
        _inst = null;
    }
    static private GameSetting _inst;

    static public GameSetting Instance => _inst;
    private void Awake()
    {
        _inst = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(1);
        }
    }

    public bool _isGravityDown;
    public bool IsGravityDown
    {
        get
        {
            return _isGravityDown;
        }
        set
        {
            if (_isGravityDown)
            {
                Physics2D.gravity = new Vector2(0f, 9.81f);
            }
            else
            {
                Physics2D.gravity = new Vector2(0f, -9.81f);
            }

            _isGravityDown = !_isGravityDown;
        }
    }
}