using System;
using CriSample.Services;
using CriSample.Settings;
using CriSample.Spectrum;
using UnityEngine;
using UnityEngine.UI;

namespace CriSample.Managers
{
    /// <summary>
    /// SpectrumScene管理クラス
    /// </summary>
    public class SpectrumSceneManager : MonoBehaviour
    {
        /// <summary>
        /// テスト用ボタン群
        /// </summary>
        [SerializeField] private Button _uiStopBgmButton;
        [SerializeField] private Button _uiPlayBgm01Button;
        [SerializeField] private Button _uiPlayBgm02Button;

        [SerializeField] private Button _uiSpectrumTypeNoneButton;
        [SerializeField] private Button _uiSpectrumTypeLineButton;
        [SerializeField] private Button _uiSpectrumTypeCubeButton;

        [SerializeField] private SpectrumVisualizer _lineSpectrumVisualizer;
        [SerializeField] private SpectrumVisualizer _cubeSpectrumVisualizer;

        private enum SpectrumType
        {
            None,
            Line,
            Cube,
        }

        private ICriAtomAudioService AudioService => ServiceLocator.Resolve<ICriAtomAudioService>();

        private Action<int> CreateSpectrumAnalyzer => (resolution) => AudioService.CreateSpectrumAnalyzer(GameAudioSettings.CueSheetName.Bgm, resolution);
        private Func<int, float[]> GetSpectrumData => (resolution) => AudioService.GetSpectrumData(resolution, true);

        private void Start()
        {
            _uiStopBgmButton.onClick.AddListener(StopBgm);
            _uiPlayBgm01Button.onClick.AddListener(PlayBgm01);
            _uiPlayBgm02Button.onClick.AddListener(PlayBgm02);

            _uiSpectrumTypeNoneButton.onClick.AddListener(() => ChangeSpectrumType(SpectrumType.None));
            _uiSpectrumTypeLineButton.onClick.AddListener(() => ChangeSpectrumType(SpectrumType.Line));
            _uiSpectrumTypeCubeButton.onClick.AddListener(() => ChangeSpectrumType(SpectrumType.Cube));

            _lineSpectrumVisualizer.Initialize(CreateSpectrumAnalyzer, GetSpectrumData);
            _cubeSpectrumVisualizer.Initialize(CreateSpectrumAnalyzer, GetSpectrumData);
            ChangeSpectrumType(SpectrumType.None);
        }

        private void ChangeSpectrumType(SpectrumType type)
        {
            _lineSpectrumVisualizer.gameObject.SetActive(type == SpectrumType.Line);
            _cubeSpectrumVisualizer.gameObject.SetActive(type == SpectrumType.Cube);
        }

        private void StopBgm()
        {
            AudioService.StopAll();
        }

        private void PlayBgm01()
        {
            var cueName = GameAudioSettings.CueName.BgmSpaceWould;
            AudioService.Play(GameAudioSettings.GetCueSheetName(cueName), cueName);
        }

        private void PlayBgm02()
        {
            var cueName = GameAudioSettings.CueName.BgmShotThunder;
            AudioService.Play(GameAudioSettings.GetCueSheetName(cueName), cueName);
        }
    }
}
