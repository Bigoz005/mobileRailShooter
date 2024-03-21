using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    AppUpdateManager appUpdateManager;

    private void Start()
    {
        
    }
    public void CheckForAppUpdate()
    {
        StartCoroutine(CheckForUpdate());
    }

    private IEnumerator CheckForUpdate()
    {
        try
        {
            appUpdateManager = new AppUpdateManager();
        }
        catch (System.Exception)
        {

        }

        if (appUpdateManager != null) { 
            PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();
            yield return appUpdateInfoOperation;

            if (appUpdateInfoOperation.IsSuccessful)
            {
                var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

                if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
                {
                    StartCoroutine(StartImmediateUpdate(appUpdateInfoResult, AppUpdateOptions.ImmediateAppUpdateOptions()));
                }
            }
        }
        yield return null;
    }

    private IEnumerator StartImmediateUpdate(AppUpdateInfo appUpdateInfoResult, AppUpdateOptions appUpdateOptions)
    {
        var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);
        yield return startUpdateRequest;
    }
}
