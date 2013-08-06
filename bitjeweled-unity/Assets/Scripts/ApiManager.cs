using UnityEngine;
using System.Collections;

public class ApiManager : MonoBehaviour {


    private const string CREATE_COMMAND = "create";
    private const string BALANCE_COMMAND = "balance";
    private const string DEPOSIT_COMMAND = "deposit";
    private const string PLAY_COMMAND = "play";
    private const string SCORE_COMMAND = "score";
    private const string TOP_COMMAND = "top";

    private static string baseUrl = "https://bitjewled-backend.appspot.com/api/";

    private string addr = "";
    private string token = "";
    private double balance = -1.0;
    private bool waitingForPlay = false;
    private bool playing = false;
    private string game_token = "";

    void Start() {
        DontDestroyOnLoad(gameObject);
        addr = PlayerPrefs.GetString("addr", "");
        token = PlayerPrefs.GetString("token", "");
        if (HasAccount()) {
            RequestBalance();
        }
    }

    void Update() {

    }

    void OnGUI() {
        if (!playing) {
            GUILayout.BeginHorizontal();
            GUILayout.Space(100);
            GUILayout.BeginVertical();
            GUILayout.Space(100);
            if (!HasAccount()) {
                if (GUILayout.Button("Create address")) {
                    RequestCreateAddress();
                }
            } else {
                GUILayout.Label("Send funds to address: " + addr);
                GUILayout.Label("Your balance: " + (balance < 0 ? "..." : "" + balance));
                if (GUILayout.Button("Update balance")) {
                    RequestBalance();
                }

                // development
                GUILayout.Space(100);
                GUILayout.Label("Development/test options:");
                if (GUILayout.Button("Delete account")) {
                    addr = "";
                    token = "";
                    PlayerPrefs.DeleteAll();
                }
                if (GUILayout.Button("Add 0.25 funds")) {
                    RequestDeposit();
                }
            }
            GUILayout.Space(100);
            GUILayout.EndVertical();
            GUILayout.Space(100);


            GUILayout.Space(100);
            GUILayout.BeginVertical();
            GUILayout.Space(100);
            if (HasAccount()) {
                if (GUILayout.Button("Play now!")) {
                    RequestPlay();
                }
                if (waitingForPlay) {
                    GUILayout.Label("Waiting for server ...");
                }
            }
            GUILayout.Space(100);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
    }

    private bool HasAccount() {
        return token.Length != 0;
    }

    private void RequestCreateAddress() {
        StartCoroutine(MakeRequest(CREATE_COMMAND, ""));
    }
    private void RequestBalance() {
        balance = -1;
        StartCoroutine(MakeRequest(BALANCE_COMMAND, "t=" + token));
    }
    private void RequestDeposit() {
        balance = -1;
        StartCoroutine(MakeRequest(DEPOSIT_COMMAND, "t=" + token + "&d=0.25"));
    }
    private void RequestPlay() {
        waitingForPlay = true;
        StartCoroutine(MakeRequest(PLAY_COMMAND, "t=" + token));
    }
    private void RequestScore(int score) {
        StartCoroutine(MakeRequest(SCORE_COMMAND, "t=" + game_token+"&s="+score));
    }

    private IEnumerator MakeRequest(string command, string parameters) {
        WWW www = new WWW(baseUrl + command + "?" + parameters);
        yield return www;
        if (www.error != null) {
            Debug.Log(www.error);
            yield return null;
        }
        string text = www.text;
        JSONObject json = JSONObject.Parse(text);
        bool success = json.GetBoolean("success");
        if (success == true) {
            OnSuccess(command, json);
        } else {
            OnFail(command, json);
        }
    }

    private void OnSuccess(string command, JSONObject json) {

        switch (command) {
            case CREATE_COMMAND:
                addr = json.GetString("addr");
                token = json.GetString("token");
                PlayerPrefs.SetString("addr", addr);
                PlayerPrefs.SetString("token", token);
                balance = 0;
                break;
            case BALANCE_COMMAND:
                balance = json.GetNumber("balance");
                break;
            case DEPOSIT_COMMAND:
                balance = json.GetNumber("balance");
                break;
            case PLAY_COMMAND:
                game_token = json.GetString("token");
                playing = true;
                waitingForPlay = false;
                Application.LoadLevel(1);
                break;
            case SCORE_COMMAND:
                playing = false;
                Application.LoadLevel(0);
                break;
            default:
                break;
        }

    }

    internal void OnFinishedGame(int score) {
        RequestScore(score);
    }

    void OnLevelWasLoaded(int index) {
        RequestBalance();
    }

    private void OnFail(string command, JSONObject json) {
        Debug.Log(command + " failed: " + json);

    }

}
