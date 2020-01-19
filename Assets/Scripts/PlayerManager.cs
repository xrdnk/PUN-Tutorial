using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region IPunObservable implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // データを他のプレイヤーに送る
            stream.SendNext(isFiring);
            stream.SendNext(Health);
        }
        else
        {
            // データを受信する
            this.isFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }
    }
    #endregion

    #region Private Serialized Fields
    [SerializeField, Tooltip("ビーム")]
    private GameObject beams;
    [SerializeField, Tooltip("Player UIプレハブ")]
    private GameObject playerUiPrefab;
    #endregion

    #region Private Fields
    // 発火しているか否か
    private bool isFiring;
    #endregion

    #region Public Fields
    [Tooltip("プレイヤーの現在HP")]
    public float Health = 1f;
    [Tooltip("ローカルプレイヤーインスタンス")]
    public static GameObject LocalPlayerInstance;
    #endregion

    #region MonoBehaviour Callbacks
    void Awake()
    {
        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }

        DontDestroyOnLoad(this.gameObject);

        if (beams == null)
        {
            Debug.LogError("<Color = Red><a>Missing</a></Color> Beams Reference.", this);
        }
        else
        {
            beams.SetActive(false);
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }

        if (playerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

#if UNITY_5_4_OR_NEWER
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (SceneManagerHelper, loadingMode) =>
            {
                this.CalledOnLevelWasLoaded(SceneManagerHelper.buildIndex);
            };
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }

        if (beams != null && isFiring != beams.activeInHierarchy)
        {
            beams.SetActive(isFiring);
            AudioManager.Instance.PlayOneShot("Beam");
        }

        if (Health <= 0f)
        {
            GameManager.Instance.LeaveRoom();
        }
    }

    /// <summary>
    /// ビームに当たった瞬間のダメージ
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (!other.name.Contains("Beam"))
        {
            return;
        }
        AudioManager.Instance.PlayOneShot("Damaged");
        Health -= 0.1f;
    }

    /// <summary>
    /// ビームに当たっている間のダメージ
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (!other.name.Contains("Beam"))
        {
            return;
        }

        // 積分してダメージを与える
        AudioManager.Instance.PlayOneShot("Damaged");
        Health -= 0.1f * Time.deltaTime;
    }

#if !UNITY_5_4_OR_NEWER
    void OnLevelWasLoaded(int level){
        this.CalledOnLevelWasLoaded(level);
    }
#endif

    void CalledOnLevelWasLoaded(int level)
    {
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }

        GameObject _uiGo = Instantiate(this.playerUiPrefab);
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    #endregion

    #region Custom
    void ProcessInputs()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isFiring)
            {
                isFiring = true;
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (isFiring)
            {
                isFiring = false;
            }
        }
    }
    #endregion
}
