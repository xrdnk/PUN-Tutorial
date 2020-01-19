using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    #region Private Fields
    private PlayerManager target;
    float characterControllerHeight = 0f;
    Transform targetTransform;
    Vector3 targetPosition;
    #endregion

    #region Private Serialized Field
    [SerializeField, Tooltip("プレイヤー名を表示するUI")]
    private Text playerNameText;
    [SerializeField, Tooltip("プレイヤーHPを表示するスライダー")]
    private Slider playerHealthSlider;
    [SerializeField, Tooltip("オフセット")]
    private Vector3 screenOffset = new Vector3(0f, 5f, 0f);
    #endregion

    #region MonoBehaviour CallBacks
    void Awake()
    {
        // TODO 動作が軽い奴に替えてみる
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = target.Health;
        }
    }

    void LateUpdate()
    {
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }
    #endregion

    #region Public Methods
    public void SetTarget(PlayerManager _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
        }

        target = _target;
        if (playerNameText != null)
        {
            playerNameText.text = target.photonView.Owner.NickName;
        }

        // ターゲットがなくなったときこのUIも破棄する
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        CharacterController _characterController = _target.GetComponent<CharacterController>();
        if (_characterController != null)
        {
            characterControllerHeight = _characterController.height;
        }
    }
    #endregion
}
