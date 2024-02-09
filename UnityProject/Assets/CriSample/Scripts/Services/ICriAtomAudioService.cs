using System;
using CriWare;
using UnityEngine;

namespace CriSample.Services
{
    public interface ICriAtomAudioService
    {
        /// <summary>
        /// CriAtom関連リソースの初期設定情報
        /// </summary>
        public class CriAtomInitializeSetting
        {
            /// <summary>
            /// ACFファイル名
            /// </summary>
            public string AcfFileName;

            /// <summary>
            /// CueSheet情報
            /// </summary>
            public CueSheetInfo[] CueSheetInfoArray;
        }

        /// <summary>
        /// CueSheet情報
        /// </summary>
        public class CueSheetInfo
        {
            /// <summary>
            /// CueSheet名
            /// </summary>
            public string Name;

            /// <summary>
            /// AWBファイルが存在するか？
            /// </summary>
            public bool IsExistAwbFile;

            /// <summary>
            /// ループ再生するか？
            /// </summary>
            public bool IsPlayLoop = false;

            public CueSheetInfo(string name, bool isExistAwbFile, bool isPlayLoop)
            {
                Name = name;
                IsExistAwbFile = isExistAwbFile;
                IsPlayLoop = isPlayLoop;
            }
        }

        /// <summary>
        /// 再生オプション
        /// </summary>
        public class AudioPlayOption
        {
            /// <summary>
            /// ボリューム
            /// </summary>
            public float Volume = 1f;

            /// <summary>
            /// ピッチ
            /// ※デフォルトは0でセント単位での指定、100で半音分
            /// </summary>
            public float Pitch = 1f;

            /// <summary>
            /// フェードさせる時間
            /// </summary>
            public int FadeTimeMs = 0;
        }

        /// <summary>
        /// 停止オプション
        /// </summary>
        public class AudioStopOption
        {
            /// <summary>
            /// フェードさせる時間
            /// </summary>
            public int FadeTimeMs = 0;
        }

        /// <summary>
        /// CueSheetの登録
        /// </summary>
        /// <param name="cueSheetInfo">CueSheet情報</param>
        public void RegisterCueSheet(CueSheetInfo cueSheetInfo);

        /// <summary>
        /// CueSheetの削除
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        public void RemoveCueSheet(string cueSheetName);

        /// <summary>
        /// 登録済の全てのCueSheetを削除
        /// </summary>
        public void RemoveAllCueSheet();

        /// <summary>
        /// CriAtomSourceの作成
        /// </summary>
        /// <param name="parentObject">親オブジェクト</param>
        /// <param name="cueSheetInfo">CueSheet情報</param>
        /// <returns></returns>
        public CriAtomSource CreateCriAtomSource(GameObject parentObject, CueSheetInfo cueSheetInfo);

        /// <summary>
        /// 音声再生の一時停止
        /// </summary>
        public void Pause();

        /// <summary>
        /// 音声再生の一時停止解除
        /// </summary>
        public void Resume();

        /// <summary>
        /// 指定したCueを再生する
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="cueName">Cue名</param>
        /// <param name="option">再生オプション</param>
        public CriAtomExPlayback Play(string cueSheetName, string cueName, AudioPlayOption option = null);

        /// <summary>
        /// 渡したCriATomSourceにオプションを適用して再生する
        /// </summary>
        /// <param name="cueName">Cue名</param>
        /// <param name="criAtomSource">CriAtomSource</param>
        /// <param name="option">再生オプション</param>
        public void PlaySource(CriAtomSource criAtomSource, string cueName, AudioPlayOption option = null);

        /// <summary>
        /// 指定したCueSheetの再生を停止する
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="option">停止オプション</param>
        public void Stop(string cueSheetName, ICriAtomAudioService.AudioStopOption option = null);

        /// <summary>
        /// 再生中のサウンドを全て止める
        /// </summary>
        /// <param name="option">停止オプション</param>
        public void StopAll(ICriAtomAudioService.AudioStopOption option = null);

        /// <summary>
        /// DSPバスのスナップショットを切り替える
        /// </summary>
        /// <param name="snapshotName">スナップショット名</param>
        /// <param name="fadeTimeMs">フェードさせる時間</param>
        public void ChangeBusSnapshot(string snapshotName, int fadeTimeMs);

        /// <summary>
        /// 指定バスのボリューム取得
        /// </summary>
        /// <param name="busName">バス名</param>
        /// <returns></returns>
        public float GetBusVolume(string busName);

        /// <summary>
        /// 指定バスのボリューム設定
        /// </summary>
        /// <param name="busName">バス名</param>
        /// <param name="volume">ボリューム</param>
        public void SetBusVolume(string busName, float volume);

        /// <summary>
        /// 指定カテゴリのボリューム取得
        /// </summary>
        /// <param name="categoryName">カテゴリ名</param>
        /// <returns></returns>
        public float GetCategoryVolume(string categoryName);

        /// <summary>
        /// 指定カテゴリのボリューム設定
        /// </summary>
        /// <param name="categoryName">カテゴリ名</param>
        /// <param name="volume">ボリューム</param>
        public void SetCategoryVolume(string categoryName, float volume);

        /// <summary>
        /// AISACコントロール値の設定
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="controlName">AISACコントロール名</param>
        /// <param name="value">設定値</param>
        /// <param name="playback">再生中の音声</param>
        public void SetAisacControl(string cueSheetName, string controlName, float value, CriAtomExPlayback playback);

        /// <summary>
        /// イベントコールバックの設定
        /// </summary>
        /// <param name="tagName">タグ名</param>
        /// <param name="callback">コールバック</param>
        public void SetSequenceCallback(string tagName, Action callback);

        /// <summary>
        /// ビート同期イベントコールバックの設定
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="labelName">ラベル名</param>
        /// <param name="callback">コールバック</param>
        public void SetBeatSyncCallback(string cueSheetName, string labelName, Action callback);

        /// <summary>
        /// AISACコントロール値の設定
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="controlName">AISACコントロール名</param>
        /// <param name="value">設定値</param>
        public void SetAisacControl(string cueSheetName, string controlName, float value);

        /// <summary>
        /// スペクトラムアナライザの作成
        /// </summary>
        /// <param name="cueSheetName">CueSheet名</param>
        /// <param name="sampleCount">サンプル数</param>
        public void CreateSpectrumAnalyzer(string cueSheetName, int sampleCount);

        /// <summary>
        /// スペクトラムアナライザから周波数データを取得する
        /// </summary>
        /// <param name="sampleCount">サンプル数</param>
        /// <param name="isConvertDecibel">デシベル値に変換するか？</param>
        /// <returns></returns>
        public float[] GetSpectrumData(int sampleCount, bool isConvertDecibel);
    }
}
