using CriSample.Services;
using CriSample.Settings;
using UnityEngine;

namespace CriSample
{
    /// <summary>
    /// プロジェクト初期化クラス
    /// </summary>
    public static class ProjectInitializer
    {
        /// <summary>
        /// シーンのロード前の初期化処理
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoad()
        {
        }

        /// <summary>
        /// シーンのロード後の初期化処理
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeAfterSceneLoad()
        {
            // サービス登録
            ServiceLocator.Register<ICriAtomAudioService>(new CriAtomAudioService(GameAudioSettings.CriAtomInitializeSetting));
        }
    }
}