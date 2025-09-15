using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
public class UIManager : NetworkBehaviour
{
    public TMP_InputField inputField;
    public Button submitButton;
    public GameObject LoginPanel;
    private void Start()
    {
        submitButton.onClick.AddListener(OnSubmitName);
        LoginPanel.SetActive(false);

        GameManager2.Instance.OnConnection += () =>
        {
            LoginPanel.SetActive(true);
            inputField.text = "";
            submitButton.interactable = true;
            inputField.interactable = true;
        };
    }
    public void OnSubmitName()
    {
        string accountID = inputField.text;
        if (!string.IsNullOrEmpty(accountID))
        {
            GameManager2.Instance.RegisterPlayerServerRpc(accountID, NetworkManager.Singleton.LocalClientId);
            submitButton.interactable = false;
            inputField.interactable = false;
            LoginPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("El nombre del jugador está vacío.");
        }
    }
}