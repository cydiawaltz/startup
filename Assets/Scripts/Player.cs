using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// キャラクターコントローラーを必須とする
[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    #region 移動に関する変数
    /// <summary>
    /// 入力の情報
    /// </summary>
    private Vector2 _input;
    /// <summary>
    /// プレイヤーにアタッチされている（紐付けされている）キャラクターコントローラー
    /// </summary>
    private CharacterController _characterController;
    /// <summary>
    /// プレイヤーが進む方向
    /// </summary>
    private Vector3 _direction;
    /// <summary>
    /// プレイヤーのスピード
    /// </summary>
    [SerializeField] private float speed;
    #endregion

    #region 回転に関する変数
    /// <summary>
    /// 補完する時間
    /// </summary>
    [SerializeField] private float smoothTime;
    /// <summary>
    /// 補完のための内部変数、深く考えなくてよい
    /// （補完の速度）
    /// </summary>
    private float _currentVelocity;
    #endregion

    #region 重力に関する変数
    /// <summary>
    /// 重力の大きさ
    /// </summary>
    [SerializeField] private float _gravity;
    /// <summary>
    /// 今受けている重力の大きさ
    /// </summary>
    private float _velocity;
    #endregion

    [SerializeField] private float jumpPower;

    /// <summary>
    /// プレイヤーが初期化されるときに呼び出される関数
    /// </summary>
    private void Awake()
    {
        // プレイヤーにアタッチされている（紐付けされている）キャラクターコントローラーを取得する
        _characterController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// 1フレームごとに呼び出される関数（ゲームループ）
    /// </summary>
    private void Update()
    {
        // カメラの向いている方向に進む方向を合わせる
        ApplyCameraDirection();
        // 重力を加える
        ApplyyGravity();
        // 進む方向へ回転させる
        ApplyRotation();
        // 進む方向へ移動させる
        ApplyMovement();

    }

    /// <summary>
    /// カメラの向いている方向に進む方向を合わせる関数
    /// </summary>
    private void ApplyCameraDirection()
    {
        // カメラの向いている方向を基に進む方向を決める
        _direction = _input.y * Camera.main.transform.forward + _input.x * Camera.main.transform.right;
        // y軸方向をなくす
        _direction.y = 0;
    }



    /// <summary>
    /// 重力を加える関数
    /// </summary>
    private void ApplyyGravity ()
    {
        // 地面と接していないなら
        if (!_characterController.isGrounded)
        {
            // 重力を加える
            _velocity += _gravity * Time.deltaTime;
        }
        // 地面と接しているなら
        else if (_velocity < 0.0f)
        {
            // 重力をリセットする
            // （0にリセットすると接地判定が不安定になる）
            _velocity = -1.0f;
        }
        // 重力を進む方向に加える
        _direction.y = _velocity;
    }


    /// <summary>
    /// 進む方向へ回転させる関数
    /// </summary>
    private void ApplyRotation ()
    {
        // 移動していない時は処理をしない
        if (_input.magnitude == 0) return;

        // プレイヤーの進む方向の角度を求める
        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        // スムーズな動きになるように角度を補完する
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        // 補完した角度を適応させる
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }

    /// <summary>
    /// 進む方向へ移動させる関数
    /// </summary>
    private void ApplyMovement ()
    {   
        // プレイヤーを移動させる
        // FPSによるズレをなくすためにTime.deltaTime（前のフレームからの経過時間）を掛ける
        _characterController.Move(_direction * speed * Time.deltaTime);
    }


    /// <summary>
    /// プレイヤーの入力（移動）がされたときに呼び出される関数
    /// </summary>
    /// <param name="context">入力の情報</param>
    public void Move(InputAction.CallbackContext context)
    {
        // 入力の情報から値を読み取る
        _input = context.ReadValue<Vector2>();
    }


    public void Jump(InputAction.CallbackContext context)
    {
        // ちょうどジャンプのキーが押されていないなら処理をしない
        if (!context.started) return;
        // 地面に接していないなら処理をしない
        if (!_characterController.isGrounded) return;
        // 重力にジャンプの力を加える
        _velocity += jumpPower;
    }
}
