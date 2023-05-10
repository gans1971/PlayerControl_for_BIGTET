# PlayerControl for BIGTET
テトリス ザ・グランドマスタービッグモード（通称ビグテト）配信サポートアプリ

※ 対応テンプレート<[**SHIG : ウメブラSP向けテンプレート 数字版** (StreamControl-for-UMBR)](http://shigaming.com/2018/11/30/streamcontroltemplate2019/)> 

## 🕹️操作イメージ（Ver0.3.4）
![capture](https://user-images.githubusercontent.com/7456610/233826620-a6e47a2f-7f84-4cbc-bcd4-ca7977387932.gif)

***カーソルキー上下で選択・左右で1P/2Pを設定・スペースキーでスコア入力***

## 🕹️概要・特徴

- 対戦型ゲーム配信サポートツール <[**StreamControl**](http://streamcontroljapan.blog.jp/)> 互換
  - StreamControl との共存・同時使用が可能
- プレイヤーが頻繁に入れ替わるフリープレイイベントやローリングマッチを想定
  - プレイヤーを一括登録
  - カーソルキーで プレイヤー選択 と 1P/2Pセット を操作可能
  - スコアは4桁半角数字のみ入力可能
  - 保存ボタンのクリック不要で即反映
- アプリケーション or OS 強制終了した場合、次回起動時に入力情報を自動復元

## 🕹️ダウンロード

[**リリース版ダウンロード**](https://github.com/gans1971/PlayerControl_for_BIGTET/releases/)

## 🕹️使い方
![InstCard](Images/InstCard.png)
[**PDF (A5版) ダウンロード**](/Manual/PlayerControlForBIGTET_InstCard_v0.3.4.pdf)
- セットアップフォルダ内の **PlayerControl.exe** を直接実行
- よく使う場合はデスクトップ等にショートカットを作成
- 基本的な使用法は [**インストカード PDF (A5版)**](/Manual/PlayerControlForBIGTET_InstCard_v0.3.4.pdf) 
- その他細かな使い方は [**wiki 使い方補足**](https://github.com/gans1971/PlayerControl_for_BIGTET/wiki/%E4%BD%BF%E3%81%84%E6%96%B9-%E8%A3%9C%E8%B6%B3)を御覧ください

## 🕹️セットアップ方法
- [**セットアップ方法 PDF (A5版) ダウンロード**](/Manual/PlayerControlForBIGTET_Setup.pdf)
- ZIPファイル展開 ⇨ *PlayerControl_for_BIGTET* フォルダ
- テンプレートフォルダ直下に *PlayerControl_for_BIGTET* フォルダをそのままコピー
  - ※　StreamControl アプリフォルダと同じ階層
<img src="Images/Setup.png" width="400px">

## 🕹️削除方法

- StreamControl テンプレートフォルダから *PlayerControl_for_BIGTET* をフォルダごと削除
- アプリ内でレジストリは使用していない

## 🕹️動作環境

- Windows10 (1809以降)  /  11
  - 追加ランタイム不要（.NET6 自己完結型アプリ）
  - StreamControl 用 StreamControl-for-UMBR テンプレートがセットアップされている環境

## 🕹️リリースノート

### ■ Ver0.3.4.0(2023/04/23)
- スコアラベル機能追加（スコア上にラベル文字を表示・サイズ自動調整）
- プレイヤーのコンテキストメニューから、スコア入力・サブ情報設定を追加
- プレイヤーのコンテキストメニューから、プレイヤー削除を追加
- ステージ入力UIのレイアウトを調整
- プレイヤー一覧のスコア入力枠にスコアラベルを表示
- プレイヤー一覧のスコア入力枠で、IMEを有効化できないように変更
- プレイヤー一覧のスコア入力枠にサブ情報（twitter等）を更新可能に変更
- JSON保存時に保存ボタンがフラッシュアニメーションする表現を追加
- 各テキストボックスにクリアボタンを追加
- アプリケーション異常終了（OSリセットを含む）の次回起動時に設定を復元する機能を追加

### ■ Ver0.3.3.0(2023/02/23)
- ユーザー設定にサブ情報（pTwitter1/pTwitter2）プロパティを追加
- ユーザー名再編集画面で、名前を変更せず確定すると同名警告が表示される問題を修正
- レイアウトを調整

### ■ Ver0.3.2.0(2023/02/12)
- スコアの最大入力桁数を4桁に拡張
- ステージ名の改行対応(Alt+Enter で改行)
  - 2行: FontSize = -1  
  - 3行以上: FontSize = -2  

### ■ Ver0.3.1.0(2022/06/25)
- プレイヤーコントロール:スコア入力時にフォーカスが背面に移動してしまう問題を修正
- プレイヤーコントロール:ステージ入力コントロールに確定ボタンを追加
- プレイヤーコントロール:プレイヤーリスト設定画面ショートカット(Ctrl+P)
- プレイヤーリスト設定をモーダル化
- プレイヤーリスト設定:アイテムコンテナをWrapPanelに変更
- プレイヤーリスト設定:名前編集ダイアログを追加（直接編集廃止）
- プレイヤーリスト設定:クローズショートカット(CTRL+W or ESC)
- プレイヤーリスト設定:名前編集後同名プレイヤーが存在する場合は変更キャンセル

### ■ Ver0.3.0.0(2022/06/13)

- レイアウトを全面改修
- イベント名入力対応
- タイトルバー表示（バージョン番号付き）
- Ctrl + C で名前とスコアをクリップボードに登録
- DefaultCountryを設定
- ユーザーリストに直接スコア入力できるように修正
- 選択ユーザーの1P/2Pをマウスで設定できるように修正
- コンテキストメニューにクリップボード追加

### ■ Ver0.2.0.0(2022/06/11)

- 初回ロケテ版

## 🕹️開発環境

- Visual Studio Community 2022
- プラットフォーム: .NET6 + WPF

## 🕹️LICENSE

- MIT License

## 🕹️謝辞

- 動作検証用に [SHIG Website StreamControl テンプレート](http://shigaming.com/2018/11/30/streamcontroltemplate2019/) を使用いたしました。
  - githubリポジトリは [こちらです](https://github.com/Pon57/StreamControl-for-UMBR)
- *StreamControl* の挙動解析に [GitHubソースコード](https://github.com/farpenoodle/StreamControl) を参照しました。
