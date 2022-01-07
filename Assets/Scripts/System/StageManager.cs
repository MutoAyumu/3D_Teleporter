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
        _panel.color = new Color(0, 0, 0, 0);
        _panel.raycastTarget = false;
        Cursor.lockState = CursorLockMode.None;
    }
    private void Start()
    {
        for (int i = 0; i < _stageNum; i++)
        {
            _selectButton[i].SetActive(true);
        }
    }
    public void StageChange(string name)
    {
        //ここにDoTweenでいい感じになるようにお願いします
        _panel.raycastTarget = true;
        DOVirtual.Color(_panel.color, new Color(0,0,0,1), _changeDuration, value => _panel.color = value).OnComplete(() => SceneManager.LoadScene(name));
    }
}
