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
    #endregion

    #region Private Fields
    // 発火しているか否か
    private bool isFiring;
    #endregion

    #region Public Fields
    [Tooltip("プレイヤーの現在HP")]
    public float Health = 1f;
    #endregion

    #region MonoBehaviour Callbacks
    void Awake()
    {
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
        Health -= 0.1f * Time.deltaTime;
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
