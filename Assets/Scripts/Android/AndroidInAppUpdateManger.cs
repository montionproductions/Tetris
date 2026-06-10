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

    [Tooltip("Si está activado, fuerza actualización inmediata cuando Google Play lo permita.")]
    [SerializeField] private bool useImmediateUpdate = false;

#if UNITY_ANDROID && !UNITY_EDITOR
    private AppUpdateManager appUpdateManager;
#endif

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (checkOnStart)
            CheckForUpdate();
    }

    public void CheckForUpdate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(CheckForUpdateRoutine());
#else
        Debug.Log("[InAppUpdate] Solo funciona en Android real instalado desde Google Play.");
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private IEnumerator CheckForUpdateRoutine()
    {
        appUpdateManager = new AppUpdateManager();

        var appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

        yield return appUpdateInfoOperation;

        if (!appUpdateInfoOperation.IsSuccessful)
        {
            Debug.LogWarning("[InAppUpdate] Error al obtener update info: " + appUpdateInfoOperation.Error);
            yield break;
        }

        AppUpdateInfo appUpdateInfo = appUpdateInfoOperation.GetResult();

        if (appUpdateInfo.UpdateAvailability != UpdateAvailability.UpdateAvailable)
        {
            Debug.Log("[InAppUpdate] No hay actualización disponible.");
            yield break;
        }

        Debug.Log("[InAppUpdate] Hay actualización disponible. VersionCode: " + appUpdateInfo.AvailableVersionCode);

        if (useImmediateUpdate && appUpdateInfo.IsUpdateTypeAllowed(AppUpdateType.Immediate))
        {
            yield return StartImmediateUpdate(appUpdateInfo);
        }
        else if (appUpdateInfo.IsUpdateTypeAllowed(AppUpdateType.Flexible))
        {
            yield return StartFlexibleUpdate(appUpdateInfo);
        }
        else if (appUpdateInfo.IsUpdateTypeAllowed(AppUpdateType.Immediate))
        {
            yield return StartImmediateUpdate(appUpdateInfo);
        }
        else
        {
            Debug.LogWarning("[InAppUpdate] Hay update, pero Google Play no permite iniciar ningún flujo.");
        }
    }

    private IEnumerator StartFlexibleUpdate(AppUpdateInfo appUpdateInfo)
    {
        Debug.Log("[InAppUpdate] Iniciando actualización flexible.");

        var options = AppUpdateOptions.FlexibleAppUpdateOptions();
        var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfo, options);

        yield return startUpdateRequest;

        if (!startUpdateRequest.IsSuccessful)
        {
            Debug.LogWarning("[InAppUpdate] Falló update flexible: " + startUpdateRequest.Error);
            yield break;
        }

        AppUpdateRequest updateRequest = startUpdateRequest.GetResult();

        while (!updateRequest.IsDone)
        {
            if (updateRequest.Status == AppUpdateStatus.Downloaded)
            {
                Debug.Log("[InAppUpdate] Update descargado. Completando instalación.");
                appUpdateManager.CompleteUpdate();
                yield break;
            }

            if (updateRequest.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogWarning("[InAppUpdate] Error durante descarga: " + updateRequest.Error);
                yield break;
            }

            yield return null;
        }

        if (updateRequest.Status == AppUpdateStatus.Downloaded)
        {
            Debug.Log("[InAppUpdate] Update descargado. Completando instalación.");
            appUpdateManager.CompleteUpdate();
        }
    }

    private IEnumerator StartImmediateUpdate(AppUpdateInfo appUpdateInfo)
    {
        Debug.Log("[InAppUpdate] Iniciando actualización inmediata.");

        var options = AppUpdateOptions.ImmediateAppUpdateOptions();
        var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfo, options);

        yield return startUpdateRequest;

        if (!startUpdateRequest.IsSuccessful)
        {
            Debug.LogWarning("[InAppUpdate] Falló update inmediato: " + startUpdateRequest.Error);
        }
    }
#endif
}