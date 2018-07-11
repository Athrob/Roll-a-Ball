using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyLogHandler : ILogHandler
{
    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        Debug.unityLogger.logHandler.LogFormat(logType, context, format, args);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        Debug.unityLogger.LogException(exception, context);
    }
}

public class PlayerController : MonoBehaviour
{

    private Rigidbody _rb;

    /// <summary>
    /// 移动速度
    /// </summary>
    public float Speed = 1;

    public Text CountText;
    public Text WinText;
    public AudioClip WinAudioClip;
    private int _count;

    private bool _isMobile;

    private Logger _logger;
    // Use this for initialization
    void Start()
    {
        _logger = new Logger(new MyLogHandler());
        _isMobile = Application.isMobilePlatform;
        _rb = GetComponent<Rigidbody>();
        _count = 0;
        WinText.text = "";
        SetCountText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (_isMobile)
        {
            var movement = new Vector3(Input.acceleration.x, 0, Input.acceleration.y);
            _rb.AddForce(movement * Speed);
        }
        else
        {
            var moveHorizontal = Input.GetAxis("Horizontal");
            var moveVertical = Input.GetAxis("Vertical");
            if (moveHorizontal == 0f && moveVertical == 0f)
            {
                return;
            }
            var movement = new Vector3(moveHorizontal, 0, moveVertical);
            _rb.AddForce(movement * Speed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            var source = other.gameObject.GetComponent<AudioSource>();
            source.PlayOneShot(source.clip);
            Destroy(other.gameObject, source.clip.length);
            // other.gameObject.SetActive(false);
            _count++;
            SetCountText();
        }
    }

    void SetCountText()
    {
        CountText.text = $"分数: {_count}";
        if (_count >= 12)
        {
            AudioSource.PlayClipAtPoint(WinAudioClip, Camera.main.transform.position);
            WinText.text = "你赢了!";
        }
    }
}
