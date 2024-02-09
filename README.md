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
| SoundTest.unity | 基本機能のサンプル集<br>・BGMクロスフェード<br>・SEの3D再生<br>・DSPエフェクト<br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/d376de23-3620-4614-85a2-dc8ac9218254"><br>・カテゴリによる音量設定画面<br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/446b6ea7-fec6-441d-9600-fc246f6bff06"> |
| Spectrum.unity | オーディオスペクトラムの実装サンプル<br>・LineRendererによる描画<br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/cb161183-c883-4ad6-998f-95750dbf03af"><br>・3DCubeによる描画<br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/2e4bd17d-6f7f-4c0e-aef6-933179ab9919"> |
| Interactive.unity | インタラクティブミュージックのサンプル<br>・ブロック再生による切替<br>・シーケンスコールバックによるイベント処理<br>・AISACコントロールによるサウンド変化<br>・BeatSyncによるビート同期操作<br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/77a60bf3-1a43-4cb6-a2c1-d8e6ed69d920"><br><img width=400 src="https://github.com/plasmo310/unity-cri-audio-sample/assets/77447256/e4250b7a-03b8-47a0-9418-1bd56e7fd14a"> | |
