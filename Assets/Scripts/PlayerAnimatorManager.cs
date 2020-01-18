using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    #region Private Serialize Fields
    [SerializeField]
    private float directionDampTime = 0.25f;
    #endregion

    #region Private Fields
    private Animator _animator;
    #endregion

    #region MonoBehaviour Callbacks
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (!_animator)
        {
            Debug.Log("PlayerAnimatorManager is Missing Animator Component", this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_animator)
        {
            return;
        }

        // ジャンプ動作の前に，走っているか確認
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        // 走っていればジャンプを許可する
        if (stateInfo.IsName("Base Layer.Run"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _animator.SetTrigger("Jump");
            }
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (v < 0)
        {
            v = 0;
        }

        _animator.SetFloat("Speed", h * h + v * v);
        _animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
    }
    #endregion
}
