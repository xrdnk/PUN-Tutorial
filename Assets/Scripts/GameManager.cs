using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields
    // シングルトン化
    public static GameManager Instance;
    public GameObject playerPrefab;
    #endregion

    #region MonoBehavior Callbacks
    void Start()
    {
        Instance = this;

        if (playerPrefab == null)
        {
            Debug.LogError("<Color = Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating Local Player from {0}, Application.loaderLevelName");
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
    }
    #endregion

    #region Photon CallBacks
    /// <summary>
    /// ルームから退出した時
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// プレイヤーがルームに参加した時
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            LoadArena();
        }
    }

    /// <summary>
    /// 特定のプレイヤーがルームから退出した時
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            LoadArena();
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 退出する
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Private Methods
    void LoadArena()
    {
        // PhotonNetwork.LoadLevel()の呼び出しはMaster Clientのみにする
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork: Trying to Load a level but we are not the master client.");
        }

        // DENI NOTE:
        // PhotonNetwork.AutomaticallySyncScene = true に設定することで，
        // Levelの読込をPhotonに依存させる
        Debug.LogFormat("PhotonNetwork: Loading Level: {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    #endregion
}
