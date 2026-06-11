using System.Collections;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif

public class AndroidInAppUpdateManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool checkOnStart = true;

    [Tooltip("Si está activo, intenta usar actualización inmediata cuando Google Play lo permita.")]
    [SerializeField] private bool useImmediateUpdate = false;

#if UNITY_ANDROID && !UNITY_EDITOR
    private AppUpdateManager appUpdateManager;
#endif

    private static AndroidInAppUpdateManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (checkOnStart)
        {
            CheckForUpdate();
        }
    }

    public void CheckForUpdate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(CheckForUpdateRoutine());
#else
        Debug.Log("[InAppUpdate] Skipped. Only works on Android device installed from Google Play.");
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private IEnumerator CheckForUpdateRoutine()
    {
        appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
            appUpdateManager.GetAppUpdateInfo();

        yield return appUpdateInfoOperation;

        if (!appUpdateInfoOperation.IsSuccessful)
        {
            Debug.LogWarning("[InAppUpdate] Failed to get update info: " + appUpdateInfoOperation.Error);
            yield break;
        }

        AppUpdateInfo appUpdateInfo = appUpdateInfoOperation.GetResult();

        if (appUpdateInfo == null)
        {
            Debug.LogWarning("[InAppUpdate] AppUpdateInfo is null.");
            yield break;
        }

        if (appUpdateInfo.UpdateAvailability != UpdateAvailability.UpdateAvailable)
        {
            Debug.Log("[InAppUpdate] No update available.");
            yield break;
        }

        Debug.Log("[InAppUpdate] Update available. VersionCode: " + appUpdateInfo.AvailableVersionCode);

        AppUpdateOptions immediateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
        AppUpdateOptions flexibleOptions = AppUpdateOptions.FlexibleAppUpdateOptions();

        bool immediateAllowed = appUpdateInfo.IsUpdateTypeAllowed(immediateOptions);
        bool flexibleAllowed = appUpdateInfo.IsUpdateTypeAllowed(flexibleOptions);

        if (useImmediateUpdate && immediateAllowed)
        {
            yield return StartImmediateUpdate(appUpdateInfo, immediateOptions);
        }
        else if (flexibleAllowed)
        {
            yield return StartFlexibleUpdate(appUpdateInfo, flexibleOptions);
        }
        else if (immediateAllowed)
        {
            yield return StartImmediateUpdate(appUpdateInfo, immediateOptions);
        }
        else
        {
            Debug.LogWarning("[InAppUpdate] Update available, but no update flow is allowed.");
        }
    }

    private IEnumerator StartFlexibleUpdate(AppUpdateInfo appUpdateInfo, AppUpdateOptions appUpdateOptions)
    {
        Debug.Log("[InAppUpdate] Starting flexible update.");

        AppUpdateRequest startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfo, appUpdateOptions);

        while (!startUpdateRequest.IsDone)
        {
            if (startUpdateRequest.Status == AppUpdateStatus.Downloaded)
            {
                Debug.Log("[InAppUpdate] Flexible update downloaded. Completing update.");
                yield return CompleteFlexibleUpdate();
                yield break;
            }

            if (startUpdateRequest.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogWarning("[InAppUpdate] Flexible update error: " + startUpdateRequest.Error);
                yield break;
            }

            yield return null;
        }

        if (startUpdateRequest.Status == AppUpdateStatus.Downloaded)
        {
            Debug.Log("[InAppUpdate] Flexible update downloaded after request finished. Completing update.");
            yield return CompleteFlexibleUpdate();
        }
        else if (startUpdateRequest.Error != AppUpdateErrorCode.NoError)
        {
            Debug.LogWarning("[InAppUpdate] Flexible update finished with error: " + startUpdateRequest.Error);
        }
        else
        {
            Debug.Log("[InAppUpdate] Flexible update flow finished. Status: " + startUpdateRequest.Status);
        }
    }

    private IEnumerator CompleteFlexibleUpdate()
    {
        PlayAsyncOperation<VoidResult, AppUpdateErrorCode> completeUpdateOperation =
            appUpdateManager.CompleteUpdate();

        yield return completeUpdateOperation;

        if (!completeUpdateOperation.IsSuccessful)
        {
            Debug.LogWarning("[InAppUpdate] CompleteUpdate failed: " + completeUpdateOperation.Error);
        }
    }

    private IEnumerator StartImmediateUpdate(AppUpdateInfo appUpdateInfo, AppUpdateOptions appUpdateOptions)
    {
        Debug.Log("[InAppUpdate] Starting immediate update.");

        AppUpdateRequest startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfo, appUpdateOptions);

        yield return startUpdateRequest;

        if (startUpdateRequest.Error != AppUpdateErrorCode.NoError)
        {
            Debug.LogWarning("[InAppUpdate] Immediate update error: " + startUpdateRequest.Error);
        }
    }
#endif
}