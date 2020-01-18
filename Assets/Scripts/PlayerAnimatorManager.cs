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
