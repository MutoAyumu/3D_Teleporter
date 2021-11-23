using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    [SerializeField] string _playerTag = "";
    [SerializeField] AudioSource _teleportAudio = default;
    [SerializeField] Transform _teleportPos = default;
    [SerializeField] PortalScript _portal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_playerTag))
        {
            _portal.Teleport(other, _teleportPos);

            if (_teleportAudio)
                _teleportAudio.Play();
        }
    }
}
