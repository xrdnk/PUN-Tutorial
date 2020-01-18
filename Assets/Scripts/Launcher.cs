using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields
    [SerializeField, Tooltip("各ルームの最大人数. 超過すると新しいルームが生成される．")]
    private byte maxPlayersPerRoom = 4;
    #endregion

    #region Private Fields
    [Tooltip("ゲームのバージョン")]
    string gameVersion = "1";
    #endregion

    #region MonoBehavior CallBacks
    void Awake()
    {
        // *重要* 
        // MasterClientがPhotonNetwork.LoadLevel()を利用できるようにする
        // 接続済プレーヤは全員自動的に同じレベルを読み込む
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        Connect();
    }

    void Update()
    {

    }
    #endregion

    #region MonoBehaviorPunCallbacks CallBacks
    /// <summary>
    /// Master Serverに接続した時(接続成功時)
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher : OnConnectedToMaster() was called by PUN");

        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// 接続失敗時
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basic Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}, cause");
    }

    /// <summary>
    /// ランダムなルームに参加が失敗したとき，ルームを生成する
    /// </summary>
    /// <param name="returnCode">エラーコード</param>
    /// <param name="message">エラーメッセージ</param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinRandomFailed() was called by PUN." +
                "No random available, so we create one. \nCalling: PhotonNetwork.CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    /// <summary>
    /// ルームに参加できた時
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN." +
                "Now this client is in a room");
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// すでに接続中の場合，ランダムでルームに参加する
    /// 未接続の場合，Photon Cloud Networkに接続する
    /// </summary>
    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            // ランダムなルームに接続を施行する
            // 失敗した場合，OnJoinRandomFailed()が呼び出される
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            // *重要* Photon Cloud接続の起点
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    #endregion

    #region Private Methods
    #endregion

}
