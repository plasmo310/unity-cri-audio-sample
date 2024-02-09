## unity-cri-audio-sample
* CRI ADX2関連の処理をまとめたサンプルプロジェクトになります。
* 下記のようなサンプルを用意しています。
  * 基本機能
  * オーディオスペクトラムの表示
  * インタラクティブミュージック
  * ビートに合わせたオブジェクトの伸縮

### バージョン
* Unity
  * 2022.3.16f1
* CRI Atom Craft LE
  * 3.50.06
* CRIWARE Unity Plug-in
  * 3.09.01

### シーン構成

* 各シーンは <a href="/UnityProject/Assets/CriSample/Scenes">UnityProject/Assets/CriSample/Scenes</a> 配下に格納しています。

| Scene名 | 概要                                                                                                                                                                                                                                                                                                                                                         |
----|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ 
| SoundTest.unity | 基本機能のサンプル集<br>・BGMクロスフェード<br>・SEの3D再生<br>・DSPエフェクト<br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/9c005afe-a9cf-4297-964a-5565e8d48b97"><br>・カテゴリによる音量設定画面<br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/77bae22f-8656-4887-9273-afe342a17cd1"> |
| Spectrum.unity | オーディオスペクトラムの実装サンプル<br>・LineRendererによる描画<br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/0ea03e5e-7aa0-4e7b-9d7f-e5a5ce457e98"><br>・3DCubeによる描画<br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/7f4abf40-1332-44fb-8370-fdcde1c8871b"> |
| Interactive.unity | インタラクティブミュージックのサンプル<br>・ブロック再生による切替<br>・シーケンスコールバックによるイベント処理<br>・AISACコントロールによるサウンド変化<br>・BeatSyncによるビート同期操作<br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/10a1d639-738d-4f33-a219-3ed06aac8250"><br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/2dca8f46-67db-4b63-853b-03ddce26cac2"> | |
