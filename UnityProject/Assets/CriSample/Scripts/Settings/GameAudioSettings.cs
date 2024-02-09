using CriSample.Services;
using UnityEngine;

namespace CriSample.Settings
{
    /// <summary>
    /// ゲーム内サウンド設定
    /// </summary>
    public static class GameAudioSettings
    {
            /// <summary>
            /// ACFファイル名
            /// </summary>
            private const string AcfFileName = "UnityCriSample.acf";

            /// <summary>
            /// CueSheet名
            /// ※追加したら CueSheetInfo にも追加する
            /// </summary>
            public static class CueSheetName
            {
                public const string Bgm = "BGM";
                public const string BgmBlock = "BGM_Block";
                public const string Se = "SE";
            }

            /// <summary>
            /// CueSheet情報
            /// ※初期生成時に登録する場合は _criAtomInitializeSetting にも設定する
            /// </summary>
            public static class CueSheetInfo
            {
                public static readonly ICriAtomAudioService.CueSheetInfo Bgm = new (CueSheetName.Bgm, true, true);
                public static readonly ICriAtomAudioService.CueSheetInfo BgmBlock = new (CueSheetName.BgmBlock, true, false); // ブロック内でループさせるためループ設定はfalse
                public static readonly ICriAtomAudioService.CueSheetInfo Se = new (CueSheetName.Se, false, false);
            }

            /// <summary>
            /// CRI関連リソースの初期生成情報
            /// </summary>
            public static readonly ICriAtomAudioService.CriAtomInitializeSetting CriAtomInitializeSetting = new ()
            {
                AcfFileName = AcfFileName,
                CueSheetInfoArray = new[]
                {
                    CueSheetInfo.Bgm,
                    CueSheetInfo.BgmBlock,
                    CueSheetInfo.Se,
                }
            };

            /// <summary>
            /// Cue名
            /// ※追加したら GetCueSheetName にも追加する
            /// </summary>
            public static class CueName
            {
                public const string BgmSpaceWould = "BGM_SpaceWould";
                public const string BgmShotThunder = "BGM_ShotThunder";
                public const string BgmMogTheme = "BGM_MogTheme";
                public const string BgmAtomChain = "BGM_AtomChain";
                public const string SeMove = "SE_Move";
                public const string SeAttack = "SE_Attack";
                public const string SeRandom = "SE_Random";
            }

            /// <summary>
            /// Cue名から対象のCueSheet名を取得する
            /// </summary>
            /// <param name="cueName"></param>
            /// <returns></returns>
            public static string GetCueSheetName(string cueName)
            {
                switch (cueName)
                {
                    // BGM
                    case CueName.BgmSpaceWould:
                    case CueName.BgmShotThunder:
                    case CueName.BgmMogTheme:
                        return CueSheetName.Bgm;
                    case CueName.BgmAtomChain:
                        return CueSheetName.BgmBlock;
                    // SE
                    case CueName.SeMove:
                    case CueName.SeAttack:
                        return CueSheetName.Se;
                }
                Debug.LogError($"not found cueSheetName by cueName=> {cueName}");
                return CueSheetName.Bgm;
            }

            /// <summary>
            /// DSPバス名
            /// </summary>
            public static class BusName
            {
                public const string Master = "MasterOut";
                public const string BgmReverb = "BGMReverb";
                public const string BgmDistortion = "BGMDistortion";
                public const string SeReverb = "SEReverb";
                public const string SeDistortion = "SEDistortion";
            }

            /// <summary>
            /// DSPバス スナップショット名
            /// </summary>
            public static class BusSnapshotName
            {
                public const string Normal = "Normal";
                public const string BgmReverb = "BGM_Reverb";
                public const string BgmDistortion = "BGM_Distortion";
                public const string SeReverb = "SE_Reverb";
                public const string SeDistortion = "SE_Distortion";
            }

            /// <summary>
            /// カテゴリー名
            /// </summary>
            public static class CategoryName
            {
                public const string Bgm = "BGM";
                public const string Se = "SE";
            }

            /// <summary>
            /// AISACコントール名
            /// </summary>
            public static class AisacName
            {
                public const string Battle = "Battle";
            }

            /// <summary>
            /// イベント名
            /// </summary>
            public static class EventName
            {
                public const string StartAtomChainMainLoop = "StartAtomChainMainLoop";
            }

            /// <summary>
            /// BeatSyncラベル名
            /// </summary>
            public static class BeatSyncLabelName
            {
                public const string AtomChain = "AtomChainBeatSyncLabel";
            }
    }
}