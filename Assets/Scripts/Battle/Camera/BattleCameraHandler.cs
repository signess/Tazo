using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraHandler : MonoBehaviour
{
    public static BattleCameraHandler Instance;

    [SerializeField] private TrackSwitcher trackSwitcher;
    [SerializeField] private Animator cameraAnimator;
    private List<bool> cameras;

    public bool groupCamera, dollyCartCamera, playerCamera, playerTrainerCamera, enemyCamera;
    public float IdleTime;

    private void Awake()
    {
        Instance = this;
        cameras = new List<bool> { groupCamera, dollyCartCamera, playerTrainerCamera, playerCamera, enemyCamera };
    }
    
    public IEnumerator SwitchGroupCamera()
    {
        cameraAnimator.Play("Main");
        IdleTime = 0;
        DeactivateCameras();

        if (!groupCamera)
            yield return new WaitForSeconds(.8f);
        else
            yield return null;

        groupCamera = true;
    }

    public IEnumerator SwitchDollyCartCamera()
    {
        cameraAnimator.Play("Idle");
        trackSwitcher.Reset();
        DeactivateCameras();
        dollyCartCamera = true;

        yield return new WaitForSeconds(.8f);
    }

    public IEnumerator SwitchPlayerCamera()
    {
        cameraAnimator.Play("Player");
        DeactivateCameras();
        playerCamera = true;

        yield return new WaitForSeconds(.8f);
    }
    public IEnumerator SwitchPlayerTrainerCamera()
    {
        cameraAnimator.Play("PlayerTrainer");
        DeactivateCameras();
        playerTrainerCamera = true;

        yield return new WaitForSeconds(.8f);
    }

    public IEnumerator SwitchEnemyCamera()
    {
        cameraAnimator.Play("Enemy");
        DeactivateCameras();
        enemyCamera = true;

        yield return new WaitForSeconds(.8f);
    }

    private void DeactivateCameras()
    {
        for(int i = 0; i < cameras.Count; i++)
        {
            cameras[i] = false;
        }
    }
}
