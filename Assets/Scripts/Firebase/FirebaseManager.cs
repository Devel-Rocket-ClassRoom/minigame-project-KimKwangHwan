using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Unity.VisualScripting;
using UnityEngine;

public class FirebaseManager : Singleton<FirebaseManager>
{
    public enum InitState
    {
        Pending,
        Ready,
        Failed,
    }

    public InitState State { get; private set; } = InitState.Pending;
    public bool IsReady => State == InitState.Ready;
    public string LastError { get; private set; }

    public FirebaseApp App { get; private set; }
    public FirebaseDatabase Database { get; private set; }
    public FirebaseAuth Auth { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitializeFirebaseAsync().Forget();
    }

    private async UniTaskVoid InitializeFirebaseAsync()
    {
        Debug.Log("[Firebase] 초기화 시작...");

        try
        {
            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();

            if (status != DependencyStatus.Available)
            {
                Fail($"의존성 오류: {status}");
                return;
            }

            App = FirebaseApp.DefaultInstance;

            Database = GetDatabase(App);
            Auth = FirebaseAuth.GetAuth(App);

            State = InitState.Ready;
            Debug.Log($"[Firebase] 초기화 성공 (app={App.Name})");
        }
        catch (System.Exception ex)
        {
            Fail(ex.Message);
        }
    }
    private FirebaseDatabase GetDatabase(FirebaseApp app)
    {
        FirebaseConfig config = Resources.Load<FirebaseConfig>("FirebaseConfig");
        if (config != null && !string.IsNullOrEmpty(config.databaseUrl))
        {
            return FirebaseDatabase.GetInstance(app, config.databaseUrl);
        }
        return FirebaseDatabase.GetInstance(app);
    }
    private void Fail(string error)
    {
        LastError = error;
        State = InitState.Failed;
        Debug.LogError($"[Firebase] 초기화 실패: {error}");
    }
    public async UniTask<bool> WaitForInitializationAsync()
    {
        await UniTask.WaitUntil(() => State != InitState.Pending);
        return State == InitState.Ready;
    }

    public async UniTask UploadSaveAsync(int slot, string json)
    {
        if (!IsReady || Auth == null) return;
        try
        {
            DatabaseReference saveRef = Database.GetReference($"users/{Auth.CurrentUser.UserId}/saves/slot{slot}");
            await saveRef.SetRawJsonValueAsync(json).AsUniTask();
            Debug.Log($"[FirebaseManager] 슬롯 {slot} 업로드 완료");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[FirebaseManager] 슬롯 {slot} 업로드 실패: {ex.Message}");
        }
    }
    public async UniTask<string> DownloadSaveAsync(int slot)
    {
        try
        {
            DatabaseReference loadRef = Database.GetReference($"users/{Auth.CurrentUser.UserId}/saves/slot{slot}");
            DataSnapshot snapshot = await loadRef.GetValueAsync().AsUniTask();
            return snapshot.Exists ? snapshot.GetRawJsonValue() : null;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[Firebase] 슬롯 {slot} 다운로드 실패: {ex.Message}");
            return null;
        }
    }

    public async UniTask<bool> HasAnySaveAsync()
    {
        try
        {
            DatabaseReference dataRef = Database.GetReference($"users/{Auth.CurrentUser.UserId}/saves");
            DataSnapshot snapshot = await dataRef.GetValueAsync().AsUniTask();
            return snapshot.Exists && snapshot.ChildrenCount > 0;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[Firebase] HasAnySave 확인 실패: {ex.Message}");
            return false;
        }
    }

    public async UniTask<bool> HasSaveAsync(int slot)
    {
        try
        {
            DatabaseReference dataRef = Database.GetReference($"users/{Auth.CurrentUser.UserId}/saves/slot{slot}");
            DataSnapshot snapshot = await dataRef.GetValueAsync().AsUniTask();
            return snapshot.Exists;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[Firebase] HasSave 확인 실패: {ex.Message}");
            return false;
        }
    }

    public async UniTask DeleteSaveAsync(int slot)
    {
        try
        {
            DatabaseReference dataRef = Database.GetReference($"users/{Auth.CurrentUser.UserId}/saves/slot{slot}");
            await dataRef.RemoveValueAsync().AsUniTask();
            Debug.Log($"[Firebase] 슬롯 {slot} 삭제 완료");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[Firebase] 슬롯 {slot} 삭제 실패: {ex.Message}");
        }
    }
}
