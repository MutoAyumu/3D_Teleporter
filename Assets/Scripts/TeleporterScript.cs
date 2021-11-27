using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    [SerializeField] string _playerTag = "";
    [SerializeField] AudioSource _teleportAudio = default;
    [SerializeField] Transform _teleportPos = default;
    [SerializeField] PortalScript _portal;
    Transform _currentPos;
    float _currentDirection;

    public Transform TeleportPos { get => _teleportPos; set => _teleportPos = value; }

    private void Start()
    {
        _currentPos = this.transform;
        _currentDirection = 180 - this.transform.rotation.y;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_playerTag))
        {
            _currentDirection = TeleportPos.rotation.y - 90;
            _portal.Teleport(other, TeleportPos, _currentDirection);

            if (_teleportAudio)
                _teleportAudio.Play();
        }
    }
}
