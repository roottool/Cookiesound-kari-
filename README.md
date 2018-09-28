# Cookie☆Sound
このアプリはIRCチャットサーバに接続し、入室したチャンネル内で入力されたコマンドと同名の音声ファイル(oggファイルとmp3ファイルに限る)の再生を行います。
設定したフックキーを押すことによってコマンド入力待機状態となり、コマンド入力を行えます。これは、このアプリ以外(ブラウザ等)を最前面に開いている時でも可能です。


# 主な機能

 - 音量調整<br>
 config.iniファイルから0～100の整数で調節可能です。

 - 再生の停止<br>
 コマンド入力待機状態でstopと入力すると現在再生されている音声が停止します。

 - BGM再生機能(Cookie☆ Sound Records:CSR)<br>
 csrフォルダ内に格納されているcsr番号.mp3ファイルのBGMを通常のコマンドとは別に再生を行います。(ファイル名指定はCookie☆Soundの仕様と合わせるため)<br>
 再生中に別の番号のコマンドが入力された際には、現在再生しているBGMを停止して入力されたBGMを再生する。
 

# 使い方
soundフォルダにはoggファイルを、csrフォルダにはmp3ファイルを置いてください。<br>
ConfigEditor.exeにてconfig.iniファイルを作成の後、起動を行って頂くと正常動作可能になります。<br>
コマンドプロンプト画面上にて、"Join チャンネル名"と表示されますとフックキーの入力受付開始となります。<br>
ConfigEditor.exeにて設定したフックキーを入力すると、コマンド入力待機状態となります。<br>
コマンド入力待機状態にてsoundフォルダやcsrフォルダに格納したファイル名(英数字、-と^に限る)を入力後Enterキー押下で同名のファイルが再生されます。
