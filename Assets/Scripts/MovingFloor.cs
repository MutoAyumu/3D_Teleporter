using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFloor : MonoBehaviour
{
    [SerializeField, Tooltip("ポイント")] Transform[] _points = default;
    [SerializeField, Tooltip("スピード")] float _moveSpeed = 1f;
    [SerializeField, Tooltip("移動先を切り替えるときのポイントとの距離")] float _stopDistance = 0.2f;

    Vector3 _move;
    int _count;

    private void Update()
    {
        Patrol();
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
        if (other.gameObject.CompareTag("Player"))
        {
            _count = (_count + 1) % _points.Length;
        }
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
}
