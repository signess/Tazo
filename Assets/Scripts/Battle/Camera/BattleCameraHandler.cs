using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public async void CheckForDynamicCamera()
    {
        if (dollyCartCamera)
            return;
        else if (IdleTime < 5)
            IdleTime += Time.deltaTime;
        else if (IdleTime >= 5)
        {
            await SwitchDollyCartCamera();
            IdleTime = 0;
        }
    }
    
    public async Task SwitchGroupCamera()
    {
        cameraAnimator.Play("Main");
        IdleTime = 0;
        DeactivateCameras();

        if (!groupCamera)
            await Task.Delay(800);
        else
            await Task.Yield();

        groupCamera = true;
    }

    public async Task SwitchDollyCartCamera()
    {
        cameraAnimator.Play("Idle");
        trackSwitcher.Reset();
        DeactivateCameras();
        dollyCartCamera = true;

        await Task.Delay(880);
    }

    public async Task SwitchPlayerCamera()
    {
        cameraAnimator.Play("Player");
        DeactivateCameras();
        playerCamera = true;

        await Task.Delay(800);
    }
    public async Task SwitchPlayerTrainerCamera()
    {
        cameraAnimator.Play("PlayerTrainer");
        DeactivateCameras();
        playerTrainerCamera = true;

        await Task.Delay(800);
    }

    public async Task SwitchEnemyCamera()
    {
        cameraAnimator.Play("Enemy");
        DeactivateCameras();
        enemyCamera = true;

        await Task.Delay(800);
    }

    private void DeactivateCameras()
    {
        for(int i = 0; i < cameras.Count; i++)
        {
            cameras[i] = false;
        }
    }
}
