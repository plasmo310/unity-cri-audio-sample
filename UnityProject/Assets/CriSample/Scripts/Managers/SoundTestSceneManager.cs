using System;
using System.Collections.Generic;
using CriSample.Objects;
using CriSample.Services;
using CriSample.Settings;
using CriSample.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CriSample.Managers
{
    /// <summary>
    /// TestScene管理クラス
    /// </summary>
    public class SoundTestSceneManager : MonoBehaviour
    {
        /// <summary>
        /// テスト用ボタン群
        /// </summary>
        [SerializeField] private Button _uiStopBgmButton;
        [SerializeField] private Button _uiStopFadeBgmButton;
        [SerializeField] private Button _uiPlayBgm01Button;
        [SerializeField] private Button _uiPlayBgm02Button;
        [SerializeField] private Button _uiPlayFadeBgm01Button;
        [SerializeField] private Button _uiPlayFadeBgm02Button;

        [SerializeField] private Button _uiStart3dSeButton;
        [SerializeField] private Button _uiStop3dSeButton;

        [SerializeField] private Button _uiEffectBgmNormalButton;
        [SerializeField] private Button _uiEffectBgmReverbButton;
        [SerializeField] private Button _uiEffectBgmDistortionButton;
        [SerializeField] private Button _uiEffectSeNormalButton;
        [SerializeField] private Button _uiEffectSeReverbButton;
        [SerializeField] private Button _uiEffectSeDistortionButton;

        [SerializeField] private Button _uiPauseButton;
        [SerializeField] private Button _uiResumeButton;
        [SerializeField] private Button _uiOpenConfigButton;
        [SerializeField] private Button _uiRemoveCueSheetButton;
        [SerializeField] private Button _uiAddBgmCueSheetButton;
        [SerializeField] private Button _uiAddSeCueSheetButton;

        /// <summary>
        /// オーディオ設定UI
        /// </summary>
        [SerializeField] private UIAudioConfig _uiAudioConfig;

        /// <summary>
        /// 3DSE確認用のロボット
        /// </summary>
        [SerializeField] private List<RobotBehaviour> _robots;

        /// <summary>
        /// CriAtomAudioサービス
        /// </summary>
        private ICriAtomAudioService AudioService => ServiceLocator.Resolve<ICriAtomAudioService>();

        // GetCategoryVolume等の処理がAwakeのタイミングだと動作しなかった.
        private void Start()
        {
            // UIイベント登録
            _uiStopBgmButton.onClick.AddListener(StopBgm);
            _uiStopFadeBgmButton.onClick.AddListener(StopFadeBgm);
            _uiPlayBgm01Button.onClick.AddListener(PlayBgm01);
            _uiPlayBgm02Button.onClick.AddListener(PlayBgm02);
            _uiPlayFadeBgm01Button.onClick.AddListener(PlayFadeBgm01);
            _uiPlayFadeBgm02Button.onClick.AddListener(PlayFadeBgm02);

            _uiStart3dSeButton.onClick.AddListener(Start3dSeSample);
            _uiStop3dSeButton.onClick.AddListener(Stop3dSeSample);

            _uiEffectBgmNormalButton.onClick.AddListener(ChangeSnapshotNormal);
            _uiEffectBgmReverbButton.onClick.AddListener(ChangeSnapshotBgmReverb);
            _uiEffectBgmDistortionButton.onClick.AddListener(ChangeSnapshotBgmDistortion);
            _uiEffectSeNormalButton.onClick.AddListener(ChangeSnapshotNormal);
            _uiEffectSeReverbButton.onClick.AddListener(ChangeSnapshotSeReverb);
            _uiEffectSeDistortionButton.onClick.AddListener(ChangeSnapshotSeDistortion);

            _uiPauseButton.onClick.AddListener(Pause);
            _uiResumeButton.onClick.AddListener(Resume);
            _uiOpenConfigButton.onClick.AddListener(() => _uiAudioConfig.gameObject.SetActive(true));
            _uiRemoveCueSheetButton.onClick.AddListener(RemoveAllCueSheet);
            _uiAddBgmCueSheetButton.onClick.AddListener(AddBgmCueSheet);
            _uiAddSeCueSheetButton.onClick.AddListener(AddSeCueSheet);

            _uiAudioConfig.gameObject.SetActive(false);
            _uiAudioConfig.SetListenerBgImage(() => _uiAudioConfig.gameObject.SetActive(false));
            _uiAudioConfig.SetValueMasterVolumeSlider(AudioService.GetBusVolume(GameAudioSettings.BusName.Master));
            _uiAudioConfig.SetValueBgmVolumeSlider(AudioService.GetCategoryVolume(GameAudioSettings.CategoryName.Bgm));
            _uiAudioConfig.SetValueSeVolumeSlider(AudioService.GetCategoryVolume(GameAudioSettings.CategoryName.Se));
            _uiAudioConfig.SetListenerMasterVolumeSlider(value => AudioService.SetBusVolume(GameAudioSettings.BusName.Master, value));
            _uiAudioConfig.SetListenerBgmVolumeSliderCallback(value => AudioService.SetCategoryVolume(GameAudioSettings.CategoryName.Bgm, value));
            _uiAudioConfig.SetListenerSeVolumeSliderCallback(value => AudioService.SetCategoryVolume(GameAudioSettings.CategoryName.Se, value));
        }

        private void StopBgm()
        {
            AudioService.Stop(GameAudioSettings.CueSheetName.Bgm);
        }

        private void StopFadeBgm()
        {
            AudioService.Stop(GameAudioSettings.CueSheetName.Bgm,
                new ICriAtomAudioService.AudioStopOption() { FadeTimeMs = 1000});
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

        private void PlayFadeBgm01()
        {
            var cueName = GameAudioSettings.CueName.BgmSpaceWould;
            AudioService.Play(GameAudioSettings.GetCueSheetName(cueName), cueName,
                new ICriAtomAudioService.AudioPlayOption() { FadeTimeMs = 1000});
        }

        private void PlayFadeBgm02()
        {
            var cueName = GameAudioSettings.CueName.BgmShotThunder;
            AudioService.Play(GameAudioSettings.GetCueSheetName(cueName), cueName,
                new ICriAtomAudioService.AudioPlayOption() { FadeTimeMs = 1000 });
        }

        private void Start3dSeSample()
        {
            foreach (var robot in _robots)
            {
                robot.StartMove();
            }
        }

        private void Stop3dSeSample()
        {
            foreach (var robot in _robots)
            {
                robot.StopMove();
            }
        }

        private void ChangeSnapshotNormal()
        {
            AudioService.ChangeBusSnapshot(GameAudioSettings.BusSnapshotName.Normal, 1000);
        }

        private void ChangeSnapshotBgmReverb()
        {
            AudioService.ChangeBusSnapshot(GameAudioSettings.BusSnapshotName.BgmReverb, 1000);
        }

        private void ChangeSnapshotBgmDistortion()
        {
            AudioService.ChangeBusSnapshot(GameAudioSettings.BusSnapshotName.BgmDistortion, 1000);
        }

        private void ChangeSnapshotSeReverb()
        {
            AudioService.ChangeBusSnapshot(GameAudioSettings.BusSnapshotName.SeReverb, 1000);
        }

        private void ChangeSnapshotSeDistortion()
        {
            AudioService.ChangeBusSnapshot(GameAudioSettings.BusSnapshotName.SeDistortion, 1000);
        }

        private void Pause()
        {
            AudioService.Pause();
            foreach (var robot in _robots)
            {
                robot.Pause();
            }
        }

        private void Resume()
        {
            AudioService.Resume();
            foreach (var robot in _robots)
            {
                robot.Resume();
            }
        }

        private void RemoveAllCueSheet()
        {
            AudioService.RemoveAllCueSheet();
        }

        private void AddBgmCueSheet()
        {
            AudioService.RegisterCueSheet(GameAudioSettings.CueSheetInfo.Bgm);
        }

        private void AddSeCueSheet()
        {
            AudioService.RegisterCueSheet(GameAudioSettings.CueSheetInfo.Se);
        }
    }
}
