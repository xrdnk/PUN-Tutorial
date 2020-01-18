using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// プレイヤー名を入力する
/// </summary>
[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    #region Private Constants
    // PlayerPref用のKey
    const string PLAYER_NAME_PREF_KEY = "PlayerName";
    #endregion

    #region MonoBehavior CallBacks
    void Start()
    {
        string defaultName = string.Empty;
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(PLAYER_NAME_PREF_KEY))
            {
                defaultName = PlayerPrefs.GetString(PLAYER_NAME_PREF_KEY);
                _inputField.text = defaultName;
            }
        }
        // ネットワーク上でプレイヤー名を設定
        PhotonNetwork.NickName = defaultName;
    }
    #endregion

    #region Public Methods
    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("プレイヤー名が入力されていません．");
            return;
        }
        PhotonNetwork.NickName = value;

        PlayerPrefs.SetString(PLAYER_NAME_PREF_KEY, value);
    }
    #endregion
}
