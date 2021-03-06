using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFloor : MonoBehaviour
{
    [SerializeField, Tooltip("ポイント")] Transform[] _points = default;
    [SerializeField, Tooltip("スピード")] float _moveSpeed = 1f;
    [SerializeField, Tooltip("移動先を切り替えるときのポイントとの距離")] float _stopDistance = 0.2f;
    [SerializeField, Tooltip("起動用のフラグ")] bool isLaunch;

    Vector3 _move;
    int _count;
    GameManager _gmanager;
    bool isPause;

    private void Awake()
    {
        _gmanager = GameObject.FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        if(_gmanager)
        {
            _gmanager.OnPauseResume += PauseResume;
        }
    }
    private void OnDisable()
    {
        if(_gmanager)
        {
            _gmanager.OnPauseResume -= PauseResume;
        }
    }
    private void Update()
    {
        if (!isPause && isLaunch)
        {
            Patrol();
        }
    }
    void Patrol()
    {
        float distance = Vector3.Distance(this.transform.position, _points[_count].position);

        if (distance > _stopDistance)
        {
            _move = (_points[_count].transform.position - this.transform.position);
            this.transform.Translate(_move.normalized * _moveSpeed * Time.deltaTime);
        }
        else
        {
            _count = (_count + 1) % _points.Length;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
            _count = (_count + 1) % _points.Length;
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(gameObject.transform);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = null;
        }
    }
    public void IsLaunch()
    {
        isLaunch = true;
    }
    void PauseResume(bool isPause)
    {
        if (isPause)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }
    public void Pause()
    {
        isPause = true;
    }
    public void Resume()
    {
        isPause = false;
    }
}
