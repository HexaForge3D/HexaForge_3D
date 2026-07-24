using UnityEngine;

public class AudioListenerSwitcher : MonoBehaviour
{
    private AudioListener _mainCameraListener;

    private void Start()
    {
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
