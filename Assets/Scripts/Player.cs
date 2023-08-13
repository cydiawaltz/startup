using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// �L�����N�^�[�R���g���[���[��K�{�Ƃ���
[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    #region �ړ��Ɋւ���ϐ�
    /// <summary>
    /// ���͂̏��
    /// </summary>
    private Vector2 _input;
    /// <summary>
    /// �v���C���[�ɃA�^�b�`����Ă���i�R�t������Ă���j�L�����N�^�[�R���g���[���[
    /// </summary>
    private CharacterController _characterController;
    /// <summary>
    /// �v���C���[���i�ޕ���
    /// </summary>
    private Vector3 _direction;
    /// <summary>
    /// �v���C���[�̃X�s�[�h
    /// </summary>
    [SerializeField] private float speed;
    #endregion

    #region ��]�Ɋւ���ϐ�
    /// <summary>
    /// �⊮���鎞��
    /// </summary>
    [SerializeField] private float smoothTime;
    /// <summary>
    /// �⊮�̂��߂̓����ϐ��A�[���l���Ȃ��Ă悢
    /// �i�⊮�̑��x�j
    /// </summary>
    private float _currentVelocity;
    #endregion

    #region �d�͂Ɋւ���ϐ�
    /// <summary>
    /// �d�͂̑傫��
    /// </summary>
    [SerializeField] private float _gravity;
    /// <summary>
    /// ���󂯂Ă���d�͂̑傫��
    /// </summary>
    private float _velocity;
    #endregion

    [SerializeField] private float jumpPower;

    /// <summary>
    /// �v���C���[�������������Ƃ��ɌĂяo�����֐�
    /// </summary>
    private void Awake()
    {
        // �v���C���[�ɃA�^�b�`����Ă���i�R�t������Ă���j�L�����N�^�[�R���g���[���[���擾����
        _characterController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// 1�t���[�����ƂɌĂяo�����֐��i�Q�[�����[�v�j
    /// </summary>
    private void Update()
    {
        // �J�����̌����Ă�������ɐi�ޕ��������킹��
        ApplyCameraDirection();
        // �d�͂�������
        ApplyyGravity();
        // �i�ޕ����։�]������
        ApplyRotation();
        // �i�ޕ����ֈړ�������
        ApplyMovement();

    }

    /// <summary>
    /// �J�����̌����Ă�������ɐi�ޕ��������킹��֐�
    /// </summary>
    private void ApplyCameraDirection()
    {
        // �J�����̌����Ă����������ɐi�ޕ��������߂�
        _direction = _input.y * Camera.main.transform.forward + _input.x * Camera.main.transform.right;
        // y���������Ȃ���
        _direction.y = 0;
    }



    /// <summary>
    /// �d�͂�������֐�
    /// </summary>
    private void ApplyyGravity ()
    {
        // �n�ʂƐڂ��Ă��Ȃ��Ȃ�
        if (!_characterController.isGrounded)
        {
            // �d�͂�������
            _velocity += _gravity * Time.deltaTime;
        }
        // �n�ʂƐڂ��Ă���Ȃ�
        else if (_velocity < 0.0f)
        {
            // �d�͂����Z�b�g����
            // �i0�Ƀ��Z�b�g����Ɛڒn���肪�s����ɂȂ�j
            _velocity = -1.0f;
        }
        // �d�͂�i�ޕ����ɉ�����
        _direction.y = _velocity;
    }


    /// <summary>
    /// �i�ޕ����։�]������֐�
    /// </summary>
    private void ApplyRotation ()
    {
        // �ړ����Ă��Ȃ����͏��������Ȃ�
        if (_input.magnitude == 0) return;

        // �v���C���[�̐i�ޕ����̊p�x�����߂�
        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        // �X���[�Y�ȓ����ɂȂ�悤�Ɋp�x��⊮����
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        // �⊮�����p�x��K��������
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }

    /// <summary>
    /// �i�ޕ����ֈړ�������֐�
    /// </summary>
    private void ApplyMovement ()
    {   
        // �v���C���[���ړ�������
        // FPS�ɂ��Y�����Ȃ������߂�Time.deltaTime�i�O�̃t���[������̌o�ߎ��ԁj���|����
        _characterController.Move(_direction * speed * Time.deltaTime);
    }


    /// <summary>
    /// �v���C���[�̓��́i�ړ��j�����ꂽ�Ƃ��ɌĂяo�����֐�
    /// </summary>
    /// <param name="context">���͂̏��</param>
    public void Move(InputAction.CallbackContext context)
    {
        // ���͂̏�񂩂�l��ǂݎ��
        _input = context.ReadValue<Vector2>();
    }


    public void Jump(InputAction.CallbackContext context)
    {
        // ���傤�ǃW�����v�̃L�[��������Ă��Ȃ��Ȃ珈�������Ȃ�
        if (!context.started) return;
        // �n�ʂɐڂ��Ă��Ȃ��Ȃ珈�������Ȃ�
        if (!_characterController.isGrounded) return;
        // �d�͂ɃW�����v�̗͂�������
        _velocity += jumpPower;
    }
}
