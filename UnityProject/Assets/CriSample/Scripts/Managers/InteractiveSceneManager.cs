using System;
using System.Collections.Generic;
using CriSample.Objects;
using CriSample.Objects.BeatSync;
using CriSample.Services;
using CriSample.Settings;
using CriSample.Spectrum;
using CriWare;
using UnityEngine;
using UnityEngine.UI;

namespace CriSample.Managers
{
    /// <summary>
    /// インタラクティブScene管理クラス
    /// </summary>
    public class InteractiveSceneManager : MonoBehaviour
    {
        /// <summary>
        /// プレイヤー
        /// </summary>
        [SerializeField] private HumanBehaviour _player;

        /// <summary>
        /// 到達目標
        /// </summary>
        [SerializeField] private Transform _targetTransform;

        /// <summary>
        /// バトルエリアだと判断する距離
        /// </summary>
        [SerializeField] private float _battleAreaDistance = 10f;

        /// <summary>
        /// 説明テキスト
        /// </summary>
        [SerializeField] private Text _uiStatusText;

        /// <summary>
        /// ステージマテリアル
        /// </summary>
        [SerializeField] private Material _stageMaterial;

        /// <summary>
        /// CriAtomAudioサービス
        /// </summary>
        private ICriAtomAudioService AudioService => ServiceLocator.Resolve<ICriAtomAudioService>();

        /// <summary>
        /// BGMのPlaybackクラス
        /// </summary>
        private CriAtomExPlayback _bgmPlayback;

        private void Start()
        {
            // オーディオスペクトラム初期化
            OnInitializeSpectrum();

            // BGMを再生
            var cueName = GameAudioSettings.CueName.BgmAtomChain;
            _bgmPlayback = AudioService.Play(GameAudioSettings.GetCueSheetName(cueName), cueName);

            // テキスト関連の初期化
            OnInitializeCharacterText();

            // インタラクティブミュージック関連の初期化
            OnInitializeInteractive();

            // ビート同期関連の初期化
            OnInitializeBeatSync();
        }

        private void Update()
        {
            OnUpdateCharacterText();
            OnUpdateInteractive();
        }

#region テキスト表示関連

        // 雑なコード...ごめんなさい...

        /// <summary>
        /// テキスト配列
        /// </summary>
        private readonly string[] _characterTextArray = new[]
        {
            "...",
            "ここはどこだろう？",
            "とにかく進まなければ...",
            "WASD: 移動",
            "ドーナツみたいなのがいる",
            "ボス「よくぞ来たな」",
            "目を背け逃げることにした",
            "END"
        };

        /// <summary>
        /// 表示中のテキストインデックス
        /// </summary>
        private int _displayCharacterTextIndex = 0;

        /// <summary>
        /// テキスト初期化
        /// </summary>
        private void OnInitializeCharacterText()
        {
            _uiStatusText.text = _characterTextArray[0];
        }

        /// <summary>
        /// テキスト更新
        /// </summary>
        private void OnUpdateCharacterText()
        {
            // テキストインデックス群
            const int readyIndex = 3;
            const int bossStartIndex = 4;
            const int bossEndIndex = 5;
            const int escapeIndex = 6;
            const int endIndex = 7;

            if (_displayCharacterTextIndex < readyIndex)
            {
                // 移動開始まで
                // マウスクリックで次のテキスト
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
                {
                    _displayCharacterTextIndex++;
                    _uiStatusText.text = _characterTextArray[_displayCharacterTextIndex];

                    if (_displayCharacterTextIndex == readyIndex)
                    {
                        _player.IsReadyMove = true;
                    }
                }
            }
            else if (_displayCharacterTextIndex < bossEndIndex)
            {
                // 移動開始〜ボスに近づくまで
                // ボスに近づいたらテキスト変更
                var targetDistance = Vector3.Distance(_player.transform.position, _targetTransform.position);
                if (targetDistance < 8f)
                {
                    _displayCharacterTextIndex = bossEndIndex;
                    _uiStatusText.text = _characterTextArray[_displayCharacterTextIndex];
                }
                else if (targetDistance < 50f)
                {
                    _displayCharacterTextIndex = bossStartIndex;
                    _uiStatusText.text = _characterTextArray[_displayCharacterTextIndex];
                }
            }
            else if (_displayCharacterTextIndex < endIndex)
            {
                // ボス到着〜離れるまで
                // ボスから遠かったら終了
                var targetDistance = Vector3.Distance(_player.transform.position, _targetTransform.position);
                if (targetDistance > _battleAreaDistance + 10f)
                {
                    _displayCharacterTextIndex = endIndex;
                    _uiStatusText.text = _characterTextArray[_displayCharacterTextIndex];
                }
                else if (targetDistance > _battleAreaDistance - 2f)
                {
                    _displayCharacterTextIndex = escapeIndex;
                    _uiStatusText.text = _characterTextArray[_displayCharacterTextIndex];
                }
            }
        }

#endregion

#region インタラクティブミュージック関連

        /// <summary>
        /// インタラクティブミュージック関連 初期化
        /// </summary>
        private void OnInitializeInteractive()
        {
            // AISAC値を初期化
            AudioService.SetAisacControl(GameAudioSettings.CueSheetName.BgmBlock, GameAudioSettings.AisacName.Battle, 0f);
        }

        /// <summary>
        /// インタラクティブミュージック関連 更新
        /// </summary>
        private void OnUpdateInteractive()
        {
            // メインブロック遷移チェック
            if (!_isChangedMainBlock)
            {
                CheckChangeMainBlock();
                return;
            }

            // AISAC値のチェック
            CheckBattleAisacControl();
        }

        /// <summary>
        /// メインブロックに遷移した？
        /// </summary>
        private bool _isChangedMainBlock = false;

        /// <summary>
        /// メインブロックに遷移するかのチェック
        /// </summary>
        private void CheckChangeMainBlock()
        {
            // プレイヤーの移動を検知したら遷移する
            if (_player.IsMoved)
            {
                _isChangedMainBlock = true;
                const int playBlock = 2;
                _bgmPlayback.SetNextBlockIndex(playBlock);
            }
        }

        /// <summary>
        /// バトルエリアAISAC値の更新
        /// </summary>
        private void CheckBattleAisacControl()
        {
            // バトルエリアに近づいたらAISAC値を上げる
            var battleAisacValue = 0f;
            var targetDistance = Vector3.Distance(_player.transform.position, _targetTransform.position);
            if (targetDistance <= _battleAreaDistance)
            {
                battleAisacValue = (_battleAreaDistance - targetDistance) / _battleAreaDistance;
            }
            AudioService.SetAisacControl(GameAudioSettings.CueSheetName.BgmBlock, GameAudioSettings.AisacName.Battle, battleAisacValue, _bgmPlayback);
        }

#endregion

#region ビート同期関連

        /// <summary>
        /// ビート同期関連の初期化
        /// </summary>
        private void OnInitializeBeatSync()
        {
            // ビート同期オブジェクトの生成
            CreateBeatSyncObjectList();

            // シーケンスコールバックを設定
            AudioService.SetSequenceCallback(GameAudioSettings.EventName.StartAtomChainMainLoop, OnStartBgmMainLoop);

            // ビート同期イベントを設定
            AudioService.SetBeatSyncCallback(GameAudioSettings.CueSheetName.BgmBlock, GameAudioSettings.BeatSyncLabelName.AtomChain, OnBeatSync);
        }

        /// <summary>
        /// BGMメインループが開始しているか？
        /// </summary>
        private bool _isStartedBgmMainLoop = false;

        /// <summary>
        /// BGMメインループの開始
        /// CRI側で設定したイベントから呼ばれる
        /// </summary>
        private void OnStartBgmMainLoop()
        {
            _isStartedBgmMainLoop = true;
        }

        /// <summary>
        /// 最大ビート数(分母)
        /// </summary>
        private const int MaxBeatCount = 4;

        /// <summary>
        /// 現在のビート数
        /// </summary>
        private int _currentBeatCount = 1;

        /// <summary>
        /// ビート同期オブジェクト群
        /// </summary>
        private List<IBeatSyncBehaviour> _beatSyncObjectList;

        /// <summary>
        /// ビート同期オブジェクトの生成
        /// </summary>
        private void CreateBeatSyncObjectList()
        {
            // 生成するZ位置範囲
            const float startPosZ = 5.5f;
            const float endPosZ = 94.5f;

            // ビート同期オブジェクト生成
            _beatSyncObjectList = new List<IBeatSyncBehaviour>();

            // Cube
            for (var i = startPosZ; Mathf.Floor(i) < Mathf.Floor(endPosZ) + 1; i += 1f)
            {
                // 両脇に添える
                _beatSyncObjectList.Add(CreateBeatSyncCubeObject(-2.75f, i));
                _beatSyncObjectList.Add(CreateBeatSyncCubeObject(2.75f, i));
            }

            // Sphere
            var sphereLoopCount = 0;
            for (var i = startPosZ; Mathf.Floor(i) < Mathf.Floor(endPosZ) + 1; i += 1f)
            {
                sphereLoopCount++;
                if (sphereLoopCount % 2 == 0)
                {
                    continue;
                }

                // 一定間隔でX、Y位置を変える
                var posX = (sphereLoopCount + 1) % 4 == 0 ? 6f : 5f;
                var posY = (sphereLoopCount + 1) % 4 == 0 ? 1.5f : 0.5f;

                // 両脇に添える
                _beatSyncObjectList.Add(CreateBeatSyncSphereObject(new Vector3(-posX, posY, i)));
                _beatSyncObjectList.Add(CreateBeatSyncSphereObject(new Vector3(posX, posY, i)));
            }
        }

        /// <summary>
        /// ビート同期Cube生成
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private BeatSyncSphereBehaviour CreateBeatSyncSphereObject(Vector3 pos)
        {
            var beatSyncObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            beatSyncObject.transform.position = pos;

            var beatSyncBehavior = beatSyncObject.AddComponent<BeatSyncSphereBehaviour>();
            beatSyncBehavior.OnInitialize(1f, 1.5f);
            var renderer = beatSyncBehavior.GetComponent<MeshRenderer>();
            renderer.material = _stageMaterial;
            return beatSyncBehavior;
        }

        /// <summary>
        /// ビート同期Sphere生成
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        private BeatSyncCubeBehaviour CreateBeatSyncCubeObject(float posX, float posZ)
        {
            const float extendCubeMaxHeight = 1.5f;

            var beatSyncObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beatSyncObject.transform.position = new Vector3(posX, 0, posZ);
            beatSyncObject.transform.localScale = new Vector3(2f, 1f, 1f);

            var beatSyncBehavior = beatSyncObject.AddComponent<BeatSyncCubeBehaviour>();
            beatSyncBehavior.OnInitialize(MaxBeatCount, extendCubeMaxHeight);
            var renderer = beatSyncBehavior.GetComponent<MeshRenderer>();
            renderer.material = _stageMaterial;
            return beatSyncBehavior;
        }

        /// <summary>
        /// ビート同期イベント処理
        /// CRI側で設定した拍のリズムに合わせて呼ばれる
        /// </summary>
        private void OnBeatSync()
        {
            if (_isStartedBgmMainLoop)
            {
                // 背景色を変えてみる
                Camera.main.backgroundColor = new Color(0f, 0f, 120f);

                // ビート同期
                foreach (var beatSyncObject in _beatSyncObjectList)
                {
                    beatSyncObject.OnBeatSync(_currentBeatCount);
                }
            }

            // ビート数を進める
            _currentBeatCount = _currentBeatCount == MaxBeatCount ? 1 : _currentBeatCount + 1;
        }

#endregion

#region オーディオスペクトラム関連

        /// <summary>
        /// LineRendererオーディオスペクトラム
        /// </summary>
        [SerializeField] private List<SpectrumVisualizer> _lineSpectrumVisualizerList;

        private Action<int> CreateSpectrumAnalyzer => (resolution) => AudioService.CreateSpectrumAnalyzer(GameAudioSettings.CueSheetName.BgmBlock, resolution);
        private Func<int, float[]> GetSpectrumData => (resolution) => AudioService.GetSpectrumData(resolution, true);

        /// <summary>
        /// オーディオスペクトラム初期化
        /// </summary>
        private void OnInitializeSpectrum()
        {
            foreach (var spectrumVisualizer in _lineSpectrumVisualizerList)
            {
                spectrumVisualizer.Initialize(CreateSpectrumAnalyzer, GetSpectrumData);
            }
        }

#endregion
    }
}
