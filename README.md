# Gridly Unity プラグイン <a href="https://github.com/gridly-spreadsheet-CMS/gridly-unity-plugin">本家（英語）</a>
<p align="center">
  <a href="README-en.md">English</a>
  / 
  <a href="README-ja.md">Japanese</a>
</p>

### 1. このプラグインでできること
Unity と Gridly 間のデータ同期を支援します。
Unity で生成されたスクリーンショットを Gridly にアップロードしたり、
ターゲット言語の文字列データを取得できます。

### 2. Gridly をセットアップする
このプラグインを使うためには、Gridly 上でのセットアップが必要です。
プラグインがカラムを正常に参照するため、**columnID** を変更します。
ソース言語のカラムは、必ず変更してください。
Gridly から Grid を開き、カラムのヘッダから、**Column properties** を選んでください。

<p align="center">
<img src="resources/column_properties.png"/>
</p>

columnID を Language に記載されている4文字の言語コードに変更してください。
jaJP のように大文字小文字を一致させてください。2文字の国コードは大文字です。

<p align="center">
<img src="resources/colId.png"/>
</p>

### 3. Unity と Gridly を接続
### 3.1 Unity プラグインのインストール
- GitHub からコードを <a href="https://github.com/nozomu-matsui/gridly-unity-plugin/archive/refs/heads/master.zip">ダウンロードZIP</a> します。
- パッケージを解凍します。
- パッケージ内の Gridly フォルダーを Unity プロジェクトのアセットフォルダーにドラッグ＆ドロップします。

### 3.2 Unity での Gridly セットアップ
最初に Grildy 設定 ウィンドウを
**Tools**->**Gridly**->**Setup Setting** から開いてください。
<p align="center">
<img src="resources/setup_setting.png"/>
</p>

API キーを入力してください。また、生成されたスクリーンショットを格納するフォルダーを指定してください。
<p align="center">
<img src="resources/setup_setting_mw.png"/>
</p>

API キーは、Gridly の Grid 右ペインの API クイックスタートメニューから取得できます。

<p align="center">
<img src="resources/api_quick_start.png"/>
</p>

このプラグインは、デフォルトで2つのビューを追加しますが、これはダミーです。削除しても改名しても構いません。
プラスアイコンから新しいビューを追加できます。
<p align="center">
<img src="resources/add_new_view.png"/>
</p>

### 3.3 プラグインを使って言語を管理する
* リストボックスで言語を選択し "追加" ボタンをクリックすると、言語の追加ができます。
Unity プロジェクト内で言語を追加すると、Gridly にもカラムが追加されます。
<p align="center">
<img src="resources/lang_selectLang.png"/>
</p>

* "X" ボタンをクリックすると、言語の削除ができます。言語を削除すると、2つのダイアログが表示されます。最初のダイアログでは、どの言語を Unity から削除するか、二つ目のダイアログでは、選択した言語と、その言語にひもづくスクリーンショットを Gridly から消すかを確認されます。
<p align="center">
<img src="resources/lang_deleteLang.png"/>
</p>

* どのフォントを使用するか、 "Font" もしくは "TmFont" 設定から選択できます。TmFont は Text Mesh Pro フォントです。
* "画像格納用カラムID" の横にあるドロップダウンメニューをクリックすることで、言語ごとの画像を格納するカラムを選択できます。
画像格納用カラムがない場合は、"カラムを作成" ボタンをクリックすることで、 Gridly に "files" タイプのカラムを作成できます。それから column ID を一覧から選択してください。


<p align="center">
<img src="resources/langScreenshotColId.png" alt="MarineGEO circle logo"/>
</p>

  

* プロジェクトのソース言語は、"ソース言語に選択" ボタンをクリックすると変更できます。
* 言語の column ID は、"カラムID" ラベルの横のドロップダウンリストを選択すると変更できます。
* 言語コードは、"選択言語" ドロップダウンリストから変更できます。

### 4. 文字列の編集
### 4.1 文字列エディターウィンドウ
文字列エディターウィンドウを開くには、 **Tool**->**Gridly**->**String Editor** を選択します。
<p align="center">
<img src="resources/str_editor.png" />
</p>
  
* **Path** : 文字列のあるシーンを選択できます。シーンのスクリーンショットをアップロードする場合に必要です。
* **Delete** : Gridly と Unity のレコードを削除します。
* **Rename** : Gridly と Unity のキーを改名します。  
* **Create Record** : string ID の指定をしてから"レポーどを作成" ボタンを叩くと、レコードを生成します。
* **Pull selected grid** : 選択した Grid のすべてのデータをダウンロードします。
* **Pull all grids** : 設定されたすべての Grid のすべてのデータをダウンロードします。
* **Push options** : Gridly にデータをプッシュする際、どのデータをアップロードするか選べます。

<p align="center">
<img src="resources/str_push_options.png" />
</p>    

* **Source text** : ソース言語文字列のみを Grildy にプッシュします。
* **Source text and screenshots**: ソース言語文字列と関連するソーススクリーンショットを Grildy にプッシュします。
* **Language screenshots** : なにか言語を選択すると、選択された言語のスクリーンショットをカラムにアップロードします。

* **Push only changed records** : ローカルの Unity で変更されたデータのみを Gridly にプッシュします。

### 4.2 実行時のデータ同期
このコンポーネントは、ゲームのリビルドを行わずに Gridly からデータを取得します。
<p align="center">
<img src="resources/str_sync.png" />
</p>    
**注意** : Gridly からのデータの取得は、development/staging 環境で行われるべきです。
live/public オブジェクトの更新は、推奨しません。

### 4.3 テキストの翻訳
+ ターゲット言語の設定には、次の **void** を使います。
<p align="center">
<img src="resources/str_set_target.png" />
</p>  

+ テキストの取得には、次の **void** を使います。
<p align="center">
<img src="resources/str_set_lang.png" />
</p>    

+ UI 翻訳テキストの設定には、次のコンポーネントを使います。
<p align="center">
<img src="resources/str_translate_text.png" />
</p>    

### 5. スクリーンショットジェネレーター
### 5.1 スクリプトのセットアップ
Gridly/ScreenshotSceneUtility 内の ScreenshotTaker シーンは、スクリーンショットの生成中に選択する言語や、スクリーンショット生成の長さ、シーン変更後のスクリーンショット撮影まで何秒待つかなどを定義できます。
<p align="center">
<img src="resources/st_script.png" />
</p>    

ドロップダウンリストから言語を選択し、"追加" ボタンをクリックすることで、言語を追加できます。
同様に "削除" から削除ができます。追加された言語は "追加された言語一覧" に表示されます。

スクリーンショットの生成中は、すべてのシーンは "Scenes in Build" 一覧に追加されます。
**File**->**Build settings** からシーンの追加ができます。
<p align="center">
<img src="resources/st_build_settings.png" />
</p>    

スクリーンショットは、Gridly 設定メニューで入力したフォルダパスに保存されます。
スクリプトによって、各言語のサブフォルダが作成されます。
スクリーンショットの生成手順は、再生ボタンからシーンを再生することで開始されます。
<p align="center">
<img src="resources/st_play.png" />
</p>    
