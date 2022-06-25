# PlayerControl for BIGTET

![capture](https://user-images.githubusercontent.com/87809679/175765945-74904b8b-928e-4ebb-9bda-f8c4e2a612a2.gif)
***カーソルキー上下で選択 左右で1P/2Pを設定しています***

![InstCard](Images/InstCard.png)

[**インストカード PDF (A5版) ダウンロード**](/Manual/PlayerControlForBIGTET_InstCard_v0.3.1.pdf)

## 概要・特徴

- 対戦型ゲーム配信サポートツール <[**StreamControl**](http://streamcontroljapan.blog.jp/)> 互換 Windows アプリ
  - StreamControl との共存・同時使用が可能
- TETRIS the Grandmaster ビッグモード（通称ビグテト）配信用に特化
- プレイヤーが頻繁に入れ替わるフリープレイイベントやローリングマッチを想定
  - 事前にプレイヤーリストを登録
  - カーソルキーだけで プレイヤー選択 と 1P/2Pセット が操作可能
  - スコアは3桁半角数字のみ入力可能

## ダウンロード

[**最新版ダウンロード**](https://github.com/gans1971/PlayerControl_for_BIGTET/releases/)

## 動作環境

- Windows10 / 11
  - 追加ランタイム不要（.NET6 自己完結型アプリ）
  - StreamControl テンプレートおよびアプリがセットアップされている環境

## セットアップ方法

- [**セットアップ方法PDF**](/Manual/PlayerControlForBIGTET_Setup.pdf)
  - ZIPファイル ⇛ *PlayerControl_for_BIGTET* フォルダ展開
  - テンプレートフォルダに展開したフォルダをコピー
  - (StreamControl フォルダと同じ階層)

## 削除方法

- テンプレートフォルダにコピーした *PlayerControl_for_BIGTET* をフォルダごと削除
- アプリ内でレジストリは使用していない

## 使い方

- セットアップフォルダ内の **PlayerControl.exe** を直接実行
- よく使う場合はデスクトップ等にショートカットを作成
- 基本的な使用法は [**インストカード PDF (A5版)**](/Manual/PlayerControlForBIGTET_InstCard_v0.3.1.pdf) を参照

## アプリの挙動解説

- アプリ内でプレイヤーやスコアなどが変更されると…
  - 実行ファイルのパスの親を辿り、*scoreboard.html* ファイルが存在するフォルダを探す
  - そのフォルダにアプリで設定した内容を記述した *streamcontrol.json* を保存
  - ファイル保存後、*scoreboard.js* が json の変更を検知してHTMLに反映
  - 保存する度に *timestamp* を変更する点がポイント
- 保存しているJSONの書式

```json
{
 "stage": "水曜フリプ",
 "pName1": "GANS",
 "pName2": "GAF",
 "pScore1": "792",
 "pScore2": "998",
 "pCountry1": "blk",
 "pCountry2": "blk",
 "timestamp": "1656151419511"
}
```

| プロパティ | サンプル値 | 解説 |
|:---|:---|:---|
|stage|"水曜フリプ" |大会・イベント名など中央に表示される文字|
|pName1|"GANS"|1P の名前 |
|pName1|"GAF"|2P の名前|
|pScore1|792 |1P のスコア（3桁の半角数字）|
|pScore2|998 |2P のスコア（3桁の半角数字）|
|pCountry1|"blk"| 国旗など（ビグテト配信専用 固定画像）|
|pCountry2|"blk"| 国旗など（ビグテト配信専用 固定画像）|
|timestamp|"1656151419511"| ファイル保存時刻(long)|

## リリースノート

### ■ Ver0.3.1.0(2022/06/25)

- スコア入力時にフォーカスが背面に移動してしまう問題を修正
- プレイヤーコントロール:プレイヤーリスト設定画面ショートカット(Ctrl+P)
- プレイヤーリスト設定をモーダル化
- プレイヤーリスト設定:アイテムコンテナをWrapPanelに変更
- プレイヤーリスト設定:名前編集ダイアログを追加（直接編集廃止）
- プレイヤーリスト設定:クローズショートカット(CTRL+W or ESC)
- プレイヤーリスト設定:名前編集後同名プレイヤーが存在する場合は変更キャンセル
- ステージ入力コントロールに確定ボタンを追加

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

## 開発環境

- Visual Studio Community 2022
- プラットフォーム: .NET6 + WPF

## LICENSE

- MIT License

## 備考

- 動作検証用に [SHIG Website StreamControl テンプレート](http://shigaming.com/2018/11/30/streamcontroltemplate2019/) をリポジトリ内に収録しております。
  - githubリポジトリは [こちらです](https://github.com/Pon57/StreamControl-for-UMBR)
- *StreamControl* の挙動解析に [GitHubソースコード](https://github.com/farpenoodle/StreamControl) を参照しました。
