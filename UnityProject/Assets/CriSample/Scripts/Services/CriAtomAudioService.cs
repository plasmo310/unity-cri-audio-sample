using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CriWare;
using UnityEngine;

namespace CriSample.Services
{
    /// <summary>
    /// CriAtomサウンド管理クラス
    /// </summary>
    public class CriAtomAudioService : ICriAtomAudioService, IDisposable
    {
        /// <summary>
        /// 生成したCueAtomExPlayerのキャッシュ
        /// key: CueSheet名, value: CueAtomExPlayer
        /// </summary>
        private readonly Dictionary<string, CriAtomExPlayer> _criAtomExPlayerCache = new Dictionary<string, CriAtomExPlayer>();

        /// <summary>
        /// CueSheetに対応するCriAtomExPlayer
        /// </summary>
        /// <param name="cueSheetName"></param>
        /// <returns></returns>
        private CriAtomExPlayer GetCriAtomExPlayer(string cueSheetName) =>
            _criAtomExPlayerCache.ContainsKey(cueSheetName) ? _criAtomExPlayerCache[cueSheetName] : null;

        public CriAtomAudioService(ICriAtomAudioService.CriAtomInitializeSetting criAtomInitializeSetting)
        {
            // CriAtom関連リソースの設定
            InitializeCriAtomSettings(criAtomInitializeSetting);
        }

        /// <summary>
        /// オブジェクト破棄
        /// </summary>
        public void Dispose()
        {
            // 生成したオブジェクトを破棄
            if (_criAtomExPlayerCache != null && _criAtomExPlayerCache.Count > 0)
            {
                foreach (var criAtomPlayer in _criAtomExPlayerCache.Values)
                {
                    criAtomPlayer.Dispose();
                }
            }
            _spectrumAnalyzer?.Dispose();
        }

        /// <summary>
        /// CriAtom関連リソースの設定
        /// </summary>
        /// <param name="criAtomInitializeSetting"></param>
        private void InitializeCriAtomSettings(ICriAtomAudioService.CriAtomInitializeSetting criAtomInitializeSetting)
        {
            if (criAtomInitializeSetting == null || string.IsNullOrEmpty(criAtomInitializeSetting.AcfFileName))
            {
                Debug.LogError($"please register 'AcfFileName' value for CriAtomInitSetting.");
                return;
            }

            // ACFファイルの登録
            CriAtomEx.RegisterAcf(null, Path.Combine(CriWare.Common.streamingAssetsPath, criAtomInitializeSetting.AcfFileName));

            // CueSheet情報の登録
            var cueSheetFileInfoArray = criAtomInitializeSetting.CueSheetInfoArray;
            if (cueSheetFileInfoArray == null || cueSheetFileInfoArray.Length <= 0)
            {
                return;
            }
            foreach (var cueSheetInfo in cueSheetFileInfoArray)
            {
                RegisterCueSheet(cueSheetInfo);
            }
        }

        /// <summary>
        /// CueSheetの登録
        /// </summary>
        /// <param name="cueSheetInfo">CueSheet情報</param>
        public void RegisterCueSheet(ICriAtomAudioService.CueSheetInfo cueSheetInfo)
        {
            // CriAtomExPlayerの生成
            CreateCriAtomExPlayer(cueSheetInfo);

            // CueSheet情報を追加
            var cueSheetName = cueSheetInfo.Name;
            CriAtom.AddCueSheet(
                cueSheetName,
                $"{cueSheetName}.acb",
                cueSheetInfo.IsExistAwbFile ? $"{cueSheetName}.awb" : null);
        }

        /// <summary>
        /// CueSheetの削除
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        public void RemoveCueSheet(string cueSheetName)
        {
            if (!_criAtomExPlayerCache.ContainsKey(cueSheetName))
            {
                return;
            }

            // CriAtomExPlayerの削除
            var criAtomPlayer = _criAtomExPlayerCache[cueSheetName];
            criAtomPlayer.Dispose();
            _criAtomExPlayerCache.Remove(cueSheetName);

            // 設定したCueSheet情報も削除
            CriAtom.RemoveCueSheet(cueSheetName);
        }

        /// <summary>
        /// 登録済の全てのCueSheetを削除
        /// </summary>
        public void RemoveAllCueSheet()
        {
            if (_criAtomExPlayerCache == null || _criAtomExPlayerCache.Count <= 0)
            {
                return;
            }

            var cueSheetNameArray = _criAtomExPlayerCache.Select(cache => cache.Key).ToArray();
            foreach (var cueSheetName in cueSheetNameArray)
            {
                RemoveCueSheet(cueSheetName);
            }
        }

        /// <summary>
        /// CriAtomExPlayerの追加
        /// </summary>
        /// <param name="cueSheetInfo"></param>
        /// <returns></returns>
        private void CreateCriAtomExPlayer(ICriAtomAudioService.CueSheetInfo cueSheetInfo)
        {
            if (cueSheetInfo == null || string.IsNullOrEmpty(cueSheetInfo.Name))
            {
                Debug.LogError($"invalid CreateCriAtomExPlayer parameters.");
                return;
            }

            // 既に生成済か？
            var cueSheetName = cueSheetInfo.Name;
            if (_criAtomExPlayerCache != null
                &&_criAtomExPlayerCache.ContainsKey(cueSheetName))
            {
                Debug.LogWarning($"already create CriAtomExPlayer => {cueSheetName}");
                return;
            }

            // CriAtomExPlayerを生成してキャッシュ
            var criAtomExPlayer = new CriAtomExPlayer();

            criAtomExPlayer.Loop(cueSheetInfo.IsPlayLoop);
            _criAtomExPlayerCache.TryAdd(cueSheetName, criAtomExPlayer);
        }

        /// <summary>
        /// CriAtomSourceの作成
        /// </summary>
        /// <param name="parentObject">親オブジェクト</param>
        /// <param name="cueSheetInfo">CueSheet情報</param>
        /// <returns></returns>
        public CriAtomSource CreateCriAtomSource(GameObject parentObject, ICriAtomAudioService.CueSheetInfo cueSheetInfo)
        {
            var criAtomSource = parentObject.AddComponent<CriAtomSource>();
            criAtomSource.cueSheet = cueSheetInfo.Name;
            criAtomSource.loop = cueSheetInfo.IsPlayLoop;
            return criAtomSource;
        }

        /// <summary>
        /// 音声再生の一時停止
        /// </summary>
        public void Pause()
        {
            ChangeAllPauseState(true);
        }

        /// <summary>
        /// 音声再生の一時停止解除
        /// </summary>
        public void Resume()
        {
            ChangeAllPauseState(false);
        }

        /// <summary>
        /// 一時停止中か？
        /// </summary>
        private bool _isPause = false;

        /// <summary>
        /// CriAtomExPlayerの停止状態の切り替え
        /// </summary>
        /// <param name="isPause"></param>
        private void ChangeAllPauseState(bool isPause)
        {
            if (_isPause == isPause)
            {
                return;
            }
            _isPause = isPause;

            if (_criAtomExPlayerCache == null || _criAtomExPlayerCache.Count <= 0)
            {
                return;
            }

            foreach (var criAtomExPlayer in _criAtomExPlayerCache.Values)
            {
                criAtomExPlayer.Pause(isPause);
            }
        }

        /// <summary>
        /// 指定したCueを再生する
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="cueName">Cue名</param>
        /// <param name="option">再生オプション</param>
        public CriAtomExPlayback Play(string cueSheetName, string cueName, ICriAtomAudioService.AudioPlayOption option = null)
        {
            var criAtomExPlayer = GetCriAtomExPlayer(cueSheetName);
            if (criAtomExPlayer == null)
            {
                Debug.LogError($"not register CueSheet => {cueSheetName}");
                return new CriAtomExPlayback();
            }

            // フェード設定を切り替える
            var fadeTime = option?.FadeTimeMs ?? 0;
            ChangeCriAtomExPlayerFadeSettings(criAtomExPlayer, fadeTime);
            if (fadeTime <= 0)
            {
                Stop(cueSheetName);
            }

            // オプションに応じて再生
            var atomExAcb = CriAtom.GetAcb(cueSheetName);
            criAtomExPlayer.SetVolume(option?.Volume ?? 1f);
            criAtomExPlayer.SetPitch(option?.Pitch ?? 0f);
            criAtomExPlayer.SetCue(atomExAcb, cueName);
            return criAtomExPlayer.Start();
        }

        /// <summary>
        /// 渡したCriATomSourceにオプションを適用して再生する
        /// </summary>
        /// <param name="cueName">Cue名</param>
        /// <param name="criAtomSource">CriAtomSource</param>
        /// <param name="option">再生オプション</param>
        public void PlaySource(CriAtomSource criAtomSource, string cueName, ICriAtomAudioService.AudioPlayOption option = null)
        {
            // フェード設定を切り替える
            var criAtomExPlayer = criAtomSource.player;
            ChangeCriAtomExPlayerFadeSettings(criAtomExPlayer, option?.FadeTimeMs ?? 0);

            // オプションに応じて再生
            criAtomSource.cueName = cueName;
            criAtomSource.volume = option?.Volume ?? 1f;
            criAtomSource.pitch = option?.Pitch ?? 0f;
            criAtomSource.Play();
        }

        /// <summary>
        /// 指定したCueSheetの再生を停止する
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="option">停止オプション</param>
        public void Stop(string cueSheetName, ICriAtomAudioService.AudioStopOption option = null)
        {
            var criAtomExPlayer = GetCriAtomExPlayer(cueSheetName);
            if (criAtomExPlayer == null)
            {
                return;
            }

            // フェード設定を切り替える
            ChangeCriAtomExPlayerFadeSettings(criAtomExPlayer, option?.FadeTimeMs ?? 0);

            // CueSheetの再生を停止する
            criAtomExPlayer.Stop();
        }

        /// <summary>
        /// 再生中のサウンドを全て止める
        /// </summary>
        /// <param name="option">停止オプション</param>
        public void StopAll(ICriAtomAudioService.AudioStopOption option = null)
        {
            if (_criAtomExPlayerCache == null || _criAtomExPlayerCache.Count <= 0)
            {
                return;
            }

            foreach (var criAtomExPlayer in _criAtomExPlayerCache.Values)
            {
                // フェード設定を切り替える
                ChangeCriAtomExPlayerFadeSettings(criAtomExPlayer, option?.FadeTimeMs ?? 0);
                criAtomExPlayer.Stop();
            }
        }

        /// <summary>
        /// DSPバスのスナップショットを切り替える
        /// </summary>
        /// <param name="snapshotName">スナップショット名</param>
        /// <param name="fadeTimeMs">フェードさせる時間</param>
        public void ChangeBusSnapshot(string snapshotName, int fadeTimeMs)
        {
            CriAtomEx.ApplyDspBusSnapshot(snapshotName, fadeTimeMs);
        }

        /// <summary>
        /// CriAtomExPlayerのフェード設定 キャッシュ情報
        /// </summary>
        private readonly Dictionary<CriAtomExPlayer, int> _currentFadeTimeMsCache = new Dictionary<CriAtomExPlayer, int>();

        /// <summary>
        /// CriAtomExPlayerのフェード設定を切り替える
        /// </summary>
        /// <param name="criAtomExPlayer">CriAtomExPlayer</param>
        /// <param name="fadeTimeMs">フェードさせる時間</param>
        private void ChangeCriAtomExPlayerFadeSettings(CriAtomExPlayer criAtomExPlayer, int fadeTimeMs = 0)
        {
            if (criAtomExPlayer == null)
            {
                return;
            }

            // 設定の変更がない場合は何も行わない
            var isFirstSettings = !_currentFadeTimeMsCache.ContainsKey(criAtomExPlayer);
            var currentFadeTime = isFirstSettings ? 0 : _currentFadeTimeMsCache[criAtomExPlayer];
            if (fadeTimeMs == currentFadeTime)
            {
                return;
            }

            // フェード設定を変更
            if (isFirstSettings)
            {
                criAtomExPlayer.AttachFader();
            }
            criAtomExPlayer.SetFadeInTime(fadeTimeMs);
            criAtomExPlayer.SetFadeOutTime(fadeTimeMs);
            _currentFadeTimeMsCache[criAtomExPlayer] = fadeTimeMs;
        }

        /// <summary>
        /// 指定バスのボリューム取得
        /// </summary>
        /// <param name="busName">バス名</param>
        /// <returns></returns>
        public float GetBusVolume(string busName)
        {
            CriAtomExAsr.GetBusVolume(busName, out var volume);
            return volume;
        }

        /// <summary>
        /// 指定バスのボリューム設定
        /// </summary>
        /// <param name="busName">バス名</param>
        /// <param name="volume">ボリューム</param>
        public void SetBusVolume(string busName, float volume) => CriAtomExAsr.SetBusVolume(busName, volume);

        /// <summary>
        /// 指定カテゴリのボリューム取得
        /// </summary>
        /// <param name="categoryName">カテゴリ名</param>
        /// <returns></returns>
        public float GetCategoryVolume(string categoryName) => CriAtom.GetCategoryVolume(categoryName);

        /// <summary>
        /// 指定カテゴリのボリューム設定
        /// </summary>
        /// <param name="categoryName">カテゴリ名</param>
        /// <param name="volume">ボリューム</param>
        public void SetCategoryVolume(string categoryName, float volume) => CriAtom.SetCategoryVolume(categoryName, volume);

        /// <summary>
        /// AISACコントロール値の設定
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="controlName">AISACコントロール名</param>
        /// <param name="value">設定値</param>
        /// <param name="playback">再生中の音声</param>
        public void SetAisacControl(string cueSheetName, string controlName, float value, CriAtomExPlayback playback)
        {
            var criAtomExPlayer = GetCriAtomExPlayer(cueSheetName);
            criAtomExPlayer?.SetAisacControl(controlName, value);
            criAtomExPlayer?.Update(playback); // 再生中の音声にも変更を適用
        }

        /// <summary>
        /// AISACコントロール値の設定
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="controlName">AISACコントロール名</param>
        /// <param name="value">設定値</param>
        public void SetAisacControl(string cueSheetName, string controlName, float value)
        {
            var criAtomExPlayer = GetCriAtomExPlayer(cueSheetName);
            criAtomExPlayer?.SetAisacControl(controlName, value);
        }

        /// <summary>
        /// イベントコールバックの設定
        /// </summary>
        /// <param name="tagName">タグ名</param>
        /// <param name="callback">コールバック</param>
        public void SetSequenceCallback(string tagName, Action callback)
        {
            // 本開発時にはremoveも考慮する必要がある
            CriAtomExSequencer.OnCallback += (ref CriAtomExSequencer.CriAtomExSequenceEventInfo info) =>
            {
                if (info.tag == tagName)
                {
                    callback.Invoke();
                }
            };
        }

        /// <summary>
        /// ビート同期イベントコールバックの設定
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="labelName">ラベル名</param>
        /// <param name="callback">コールバック</param>
        public void SetBeatSyncCallback(string cueSheetName, string labelName, Action callback)
        {
            var criAtomExPlayer = GetCriAtomExPlayer(cueSheetName);
            if (criAtomExPlayer == null)
            {
                return;
            }

            // 本開発時にはremoveも考慮する必要がある
            criAtomExPlayer.OnBeatSyncCallback += (ref CriAtomExBeatSync.Info info) =>
            {
                if (info.label == labelName)
                {
                    callback?.Invoke();
                }
            };
        }

        /// <summary>
        /// スペクトラムアナライザ
        /// </summary>
        private CriAtomExOutputAnalyzer _spectrumAnalyzer;

        /// <summary>
        /// スペクトラムアナライザの作成
        /// CriAtomExPlayerを再生する前に紐づけておく必要がある
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="sampleCount">サンプル数</param>
        public void CreateSpectrumAnalyzer(string cueSheetName, int sampleCount)
        {
            if (!_criAtomExPlayerCache.ContainsKey(cueSheetName))
            {
                return;
            }
            var criAtomExPlayer = _criAtomExPlayerCache[cueSheetName];

            // 既に作成されていたらデタッチ、破棄する
            _spectrumAnalyzer?.DetachExPlayer();
            _spectrumAnalyzer?.Dispose();

            // スペクトラムアナライザ用の設定
            var config = new CriAtomExOutputAnalyzer.Config();
            config.enableSpectrumAnalyzer = true;
            config.numSpectrumAnalyzerBands = sampleCount;
            _spectrumAnalyzer = new CriAtomExOutputAnalyzer(config);
            _spectrumAnalyzer.AttachExPlayer(criAtomExPlayer);
        }

        /// <summary>
        /// スペクトラムアナライザから周波数データを取得する
        /// </summary>
        /// <param name="sampleCount">サンプル数</param>
        /// <param name="isConvertDecibel">デシベル値に変換するか？</param>
        /// <returns></returns>
        public float[] GetSpectrumData(int sampleCount, bool isConvertDecibel)
        {
            var result = new float[sampleCount];
            if (_spectrumAnalyzer == null)
            {
                Debug.LogError("not called 'CreateSpectrumAnalyzer' method.");
                return result;
            }

            // 周波数データを取得
            _spectrumAnalyzer.GetSpectrumLevels(ref result);

            // デシベルに変換
            // https://game.criware.jp/manual/unity_plugin/latest/contents/classCriWare_1_1CriAtomExOutputAnalyzer.html#a6b99d6b5310af38efe20ff834c59c4e0
            if (isConvertDecibel)
            {
                result = result.Select((value) =>
                {
                    if (value <= Mathf.Epsilon)
                    {
                        return 0f;
                    }
                    return 20.0f * Mathf.Log10(value);
                }).ToArray();
            }

            return result;
        }
    }
}
