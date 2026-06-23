using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField]
    private GameObject loginPanel;
    [SerializeField]
    private TMP_InputField emailInput;
    [SerializeField]
    private TMP_InputField passwordInput;
    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private Button signupButton;


    private async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(() => AuthManager.Instance.IsInitialized);
        loginButton.onClick.AddListener(() => OnLoginButtonClicked().Forget());
        signupButton.onClick.AddListener(() => OnSignupButtonClicked().Forget());

        UpdateUI().Forget();
    }

    public async UniTaskVoid UpdateUI()
    {
        if (!AuthManager.Instance.IsInitialized)
            return;

        bool isLoggedIn = AuthManager.Instance.IsLoggedIn;
        loginPanel.SetActive(!isLoggedIn);

        // if (isLoggedIn)
    }

    private async UniTaskVoid OnLoginButtonClicked()
    {
        string email = emailInput.text.Trim();
        string passwd = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(passwd))
        {
            return;
        }

        SetButtonsInteractable(false);

        var (success, error) = await AuthManager.Instance.SignInUserWithEmailAsync(email, passwd);

        if (success)
        {
            UpdateUI().Forget();
        }

        SetButtonsInteractable(true);
    }

    private async UniTaskVoid OnSignupButtonClicked()
    {
        string email = emailInput.text.Trim();
        string passwd = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(passwd))
        {
            return;
        }

        SetButtonsInteractable(false);

        var (success, error) = await AuthManager.Instance.CreateUserWithEmailAsync(email, passwd);

        if (success)
        {
            UpdateUI().Forget();
        }


        SetButtonsInteractable(true);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        loginButton.interactable = interactable;
        signupButton.interactable = interactable;
    }
}
