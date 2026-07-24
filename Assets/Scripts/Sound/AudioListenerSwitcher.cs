using UnityEngine;

public class AudioListenerSwitcher : MonoBehaviour
{
    private AudioListener _mainCameraListener;
    private AudioListener _playerListener;
    private void Start()
    {
        _playerListener = GetComponent<AudioListener>();
        if (_playerListener == null)
        {
            _playerListener = gameObject.AddComponent<AudioListener>();
        }

        _playerListener.transform.localPosition = Vector3.zero;
        _playerListener.enabled = true;

        GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");

        if (mainCam != null)
        {
            _mainCameraListener = mainCam.GetComponent<AudioListener>();

            if (_mainCameraListener != null)
            {
                _mainCameraListener.enabled = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (_mainCameraListener != null)
        {
            _mainCameraListener.enabled = true;
        }
    }
}
