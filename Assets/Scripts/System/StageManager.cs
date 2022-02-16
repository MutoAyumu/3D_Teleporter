using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [SerializeField] GameObject[] _selectButton;
    public static int _stageNum = 1;
    [SerializeField] Image _panel = default;
    [SerializeField] float _changeDuration = 3f;

    private void Awake()
    {
        _panel.color = new Color(0, 0, 0, 1);
        _panel.raycastTarget = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void Start()
    {
        DOVirtual.Color(_panel.color, new Color(0, 0, 0, 0), _changeDuration, value => _panel.color = value)
            .OnComplete(() =>
            {
                _panel.raycastTarget = false;

                for (int i = 0; i < _stageNum; i++)
                {
                    _selectButton[i].SetActive(true);
                }
            });
    }
    public void StageChange(string name)
    {
        //ここにDoTweenでいい感じになるようにお願いします
        _panel.raycastTarget = true;
        AudioController._instance.transform.GetChild(0).gameObject.SetActive(false);
        DOVirtual.Color(_panel.color, new Color(0, 0, 0, 1), _changeDuration, value => _panel.color = value)
            .OnComplete(() =>
            {
                SceneManager.LoadScene(name);
             });
    }
    public void StageChange()
    {
        //ここにDoTweenでいい感じになるようにお願いします
        _panel.raycastTarget = true;
        AudioController._instance.transform.GetChild(0).gameObject.SetActive(false);
        DOVirtual.Color(_panel.color, new Color(0, 0, 0, 1), _changeDuration, value => _panel.color = value)
            .OnComplete(() =>
            {
                SceneManager.LoadScene(_stageNum);
            });
    }
    public void GameExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
