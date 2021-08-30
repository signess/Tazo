using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraHandler : MonoBehaviour
{
    public static BattleCameraHandler Instance;

    [SerializeField] private TrackSwitcher trackSwitcher;
    [SerializeField] private Animator cameraAnimator;

    public bool groupCamera, dollyCartCamera, playerCamera, enemyCamera;
    public float IdleTime;

    private void Awake()
    {
        Instance = this;
    }
    
    public IEnumerator SwitchGroupCamera()
    {
        cameraAnimator.Play("Main");
        IdleTime = 0;
        dollyCartCamera = false;
        playerCamera = false;
        enemyCamera = false;

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
        dollyCartCamera = true;
        groupCamera = false;
        playerCamera = false;
        enemyCamera = false;

        yield return new WaitForSeconds(.8f);
    }

    public IEnumerator SwitchPlayerCamera()
    {
        cameraAnimator.Play("Player");
        playerCamera = true;
        groupCamera = false;
        dollyCartCamera = false;
        enemyCamera = false;

        yield return new WaitForSeconds(.8f);
    }

    public IEnumerator SwitchEnemyCamera()
    {
        cameraAnimator.Play("Enemy");
        enemyCamera = true;
        groupCamera = false;
        dollyCartCamera = false;
        playerCamera = false;

        yield return new WaitForSeconds(.8f);
    }
}
