using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    [SerializeField] PlayerController _player = default;
    [SerializeField] string _playerTag = "Player";
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag(_playerTag))
        {
            _player.Damage(25 * Time.deltaTime);
        }
    }
}
