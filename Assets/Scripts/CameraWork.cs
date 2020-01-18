using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラのターゲット追跡スクリプト
/// </summary>
public class CameraWork : MonoBehaviour
{
    #region Private Serialized Fields
    [SerializeField, Tooltip("ZX平面におけるターゲットまでの距離")]
    private float distance = 7.0f;
    [SerializeField, Tooltip("カメラを上に動かす高さ")]
    private float height = 3.0f;
    [SerializeField, Tooltip("カメラ上昇時のラグ")]
    private float heightSmoothLag = 0.3f;
    [SerializeField, Tooltip("垂直方向オフセット")]
    private Vector3 centerOffset = Vector3.zero;
    [SerializeField, Tooltip("PhotonNetworkでインスタンス化されている場合，falseに設定する")]
    private bool followOnStart = false;
    #endregion

    #region Private Fields
    // ターゲットの移動を保持
    Transform _cameraTransform;
    bool _isFollowing;
    private float _heightVelocity;
    private float targetHeight = 100000.0f;
    #endregion

    #region MonoBehaviour Callbacks
    // Start is called before the first frame update
    void Start()
    {
        if (followOnStart)
        {
            OnStartFollowing();
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_cameraTransform == null && _isFollowing)
        {
            OnStartFollowing();
        }

        if (_isFollowing)
        {
            Apply();
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 
    /// </summary>
    public void OnStartFollowing()
    {
        _cameraTransform = Camera.main.transform;
        _isFollowing = true;

        Cut();
    }
    #endregion

    #region Private Methods
    void Apply()
    {
        Vector3 targetCenter = transform.position + centerOffset;
        // 回転角の取得
        float originalTargetAngle = transform.eulerAngles.y;
        float currentAngle = _cameraTransform.eulerAngles.y;
        // カメラがロックされている時，ターゲットの角度を調整
        float targetAngle = originalTargetAngle;
        currentAngle = targetAngle;
        targetHeight = targetCenter.y + height;

        // 高さを抑える
        float currentHeight = _cameraTransform.position.y;
        currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightSmoothLag, heightSmoothLag);
        // 角度をrotationに変換
        Quaternion currentRotation = Quaternion.Euler(0, currentAngle, 0);

        _cameraTransform.position = targetCenter;
        _cameraTransform.position += currentRotation * Vector3.back * distance;

        _cameraTransform.position = new Vector3(_cameraTransform.position.x, currentHeight, _cameraTransform.position.z);

        SetUpRotation(targetCenter);
    }

    void Cut()
    {
        float oldHeightSmooth = heightSmoothLag;
        heightSmoothLag = 0.001f;
        Apply();
        heightSmoothLag = oldHeightSmooth;
    }

    void SetUpRotation(Vector3 centerPos)
    {
        Vector3 cameraPos = _cameraTransform.position;
        Vector3 offsetToCenter = centerPos - cameraPos;

        Quaternion yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));
        Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
        _cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);

    }
    #endregion
}
