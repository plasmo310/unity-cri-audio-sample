using Common;
using CriSample.Services;
using CriSample.Settings;
using CriWare;
using UnityEngine;

namespace CriSample.Objects
{
    /// <summary>
    /// AIロボット
    /// ※3D再生確認用
    /// </summary>
    public class RobotBehaviour : MonoBehaviour
    {
        /// <summary>
        /// ステート関連定義
        /// </summary>
        public enum RobotState
        {
            None,
            Wait,
            Move,
            Action,
        }
        private StateMachine<RobotBehaviour> _stateMachine;
        public StateMachine<RobotBehaviour> StateMachine => _stateMachine;
        public void StartMove()
        {
            _stateMachine.ChangeState((int) RobotState.Wait);
        }
        public void StopMove()
        {
            _stateMachine.ChangeState((int) RobotState.None);
        }

        /// <summary>
        /// アニメーション関連定義
        /// </summary>
        private Animator _animator;
        private static readonly string PutTrigger = "PutTrigger";
        public void PlayPutAnimation()
        {
            _animator.SetTrigger(PutTrigger);
        }
        public void PutHitCallback()
        {
            // アニメーション内のイベントで呼び出される
            PlayOneShot(GameAudioSettings.CueName.SeAttack);
        }

        /// <summary>
        /// オーディオ関連定義
        /// </summary>
        [SerializeField] private float _sePitch = 0f;
        private ICriAtomAudioService AudioService => ServiceLocator.Resolve<ICriAtomAudioService>();
        private CriAtomSource _seAudioSource;
        public void PlayOneShot(string audioName)
        {
            var option = new ICriAtomAudioService.AudioPlayOption()
            {
                Volume = 1f,
                Pitch = _sePitch
            };
            AudioService.PlaySource(_seAudioSource, audioName, option);
        }

        /// <summary>
        /// 一時停止・解除
        /// </summary>
        private bool _isPause = false;
        private float _pauseAnimationTime = 0f;
        private int _pauseAnimationStateHash = 0;
        public void Pause()
        {
            _isPause = true;
            _seAudioSource.Pause(true);

            _animator.enabled = false;
            _pauseAnimationTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            _pauseAnimationStateHash = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        }
        public void Resume()
        {
            _isPause = false;
            _seAudioSource.Pause(false);

            _animator.enabled = true;
            _animator.Play(_pauseAnimationStateHash, 0, _pauseAnimationTime);
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();

            // AudioSourceを設定
            _seAudioSource = AudioService.CreateCriAtomSource(gameObject, GameAudioSettings.CueSheetInfo.Se);
            _seAudioSource.use3dPositioning = true; // 3D再生を有効にする

            // ステート定義
            _stateMachine = new StateMachine<RobotBehaviour>(this);
            _stateMachine.Add<StateNone>((int) RobotState.None);
            _stateMachine.Add<StateWait>((int) RobotState.Wait);
            _stateMachine.Add<StateMove>((int) RobotState.Move);
            _stateMachine.Add<StateAction>((int) RobotState.Action);
            _stateMachine.OnStart((int) RobotState.None);
        }

        private void Update()
        {
            if (_isPause)
            {
                return;
            }

            _stateMachine.OnUpdate();
        }

        // ----- 何もしない -----
        private class StateNone : StateMachine<RobotBehaviour>.StateBase
        {
            private static readonly float Speed = 3.5f;
            public override void OnStart()
            {
            }

            public override void OnUpdate()
            {
                var targetPosition = Vector3.zero;
                Owner.transform.localPosition = Vector3.MoveTowards(Owner.transform.localPosition, targetPosition, Speed * Time.deltaTime);
            }

            public override void OnEnd()
            {
            }
        }

        // ----- 待機 -----
        private class StateWait : StateMachine<RobotBehaviour>.StateBase
        {
            private static readonly float StateMaxTime = 1.5f;
            private static readonly float StateMinTime = 0.5f;
            private float _stateTime = 0f;
            private float _elapsedTime = 0f;
            public override void OnStart()
            {
                _stateTime = UnityEngine.Random.Range(StateMinTime, StateMaxTime);
                _elapsedTime = 0f;
            }

            public override void OnUpdate()
            {
                // 一定時間経過で移動する
                _elapsedTime += Time.deltaTime;
                if (_elapsedTime >= _stateTime)
                {
                    Owner.StateMachine.ChangeState((int) RobotState.Move);
                }
            }

            public override void OnEnd()
            {
            }
        }

        // ----- 移動 -----
        private class StateMove : StateMachine<RobotBehaviour>.StateBase
        {
            private static readonly float Speed = 3.5f;
            private Vector3 _targetPosition;

            public override void OnStart()
            {
                // 範囲内でランダムに目的地を決める
                var randomPosX = UnityEngine.Random.Range(-3f, 3f);
                var randomPosY = UnityEngine.Random.Range(-2f, 2f);
                var randomPosZ = UnityEngine.Random.Range(-3f, 3f);
                _targetPosition = new Vector3(randomPosX, randomPosY, randomPosZ);

                // 動き始める時にSEを再生する
                Owner.PlayOneShot(GameAudioSettings.CueName.SeMove);
            }

            public override void OnUpdate()
            {
                // 一定時間経過で移動する
                Owner.transform.localPosition = Vector3.MoveTowards(Owner.transform.localPosition, _targetPosition, Speed * Time.deltaTime);
                if (Mathf.Approximately(0.0f, Vector3.Distance(Owner.transform.localPosition, _targetPosition)))
                {
                    Owner.StateMachine.ChangeState((int) RobotState.Action);
                }
            }

            public override void OnEnd()
            {
            }
        }

        // ----- アクション -----
        private class StateAction : StateMachine<RobotBehaviour>.StateBase
        {
            private static readonly float StateTime = 0.5f;
            private float _elapsedTime;

            public override void OnStart()
            {
                _elapsedTime = 0.0f;
                Owner.PlayPutAnimation();
            }

            public override void OnUpdate()
            {
                // 一定時間経過で移動する
                _elapsedTime += Time.deltaTime;
                if (_elapsedTime >= StateTime)
                {
                    Owner.StateMachine.ChangeState((int) RobotState.Wait);
                }
            }

            public override void OnEnd()
            {
            }
        }
    }
}