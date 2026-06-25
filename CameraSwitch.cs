using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Base Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera mainVcam;

    private CinemachineVirtualCamera activeZone;

    void Start()
    {
        ForceMain();
    }

    void Update()
    {
        if (mainCamera == null || mainVcam == null)
            return;

        if (!Input.GetMouseButtonDown(2))
            return;

        if (activeZone != null)
        {
            ForceMain();
            return;
        }

        TryEnterZone();
    }

    void TryEnterZone()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var vcam = hit.collider.GetComponentInParent<CinemachineVirtualCamera>();

            if (vcam != null)
            {
                ActivateZone(vcam);
            }
        }
    }

    void ActivateZone(CinemachineVirtualCamera zoneCam)
    {
        activeZone = zoneCam;

        mainVcam.Priority = 0;
        zoneCam.Priority = 20;
    }

    void ForceMain()
    {
        if (activeZone != null)
        {
            activeZone.Priority = 0;
            activeZone = null;
        }

        mainVcam.Priority = 100;
    }
}