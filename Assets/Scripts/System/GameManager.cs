using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] int _nextChapterNumber;
    [SerializeField] Image _panel = default;
    [SerializeField] float _changeDuration = 3f;
    [SerializeField] string _nextSceneName = " ";
    [SerializeField] Image _optionImage = default;
    [SerializeField] string _optionName = "Cancel";

    private void Awake()
    {
        _panel.color = new Color(0, 0, 0, 0);
        _panel.raycastTarget = false;
        _optionImage.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetButtonDown(_optionName))
        {
            Option();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();

        if (player)
        {
            StageManager._stageNum = Mathf.Max(StageManager._stageNum, _nextChapterNumber);
            StageChange(_nextSceneName);
        }
    }
    public void StageChange(string name)
    {
        _panel.raycastTarget = true;
        DOVirtual.Color(_panel.color, new Color(0, 0, 0, 1), _changeDuration, value => _panel.color = value).OnComplete(() => SceneManager.LoadScene(name));
    }
    public void Option()
    {
        if (_optionImage.gameObject.activeSelf)//オプションパネルがアクティブだったらパネルを消してカーソルをロックする
        {
            _optionImage.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            _optionImage.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
