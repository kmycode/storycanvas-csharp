# storycanvas-csharp
Free story editor for Windows Desktop, Android and iOS

## 概要

StoryCanvasとは、Windows、Android、iOSで動作する、ストーリー編集ソフトです。互いの端末感でのデータやり取りも可能です。<br>
公式サイト：https://storycanvas.kmycode.net/

## 開発言語

C# （フレームワーク：WPF / Xamarin.Forms）

## 開発環境

Microsoft(R) Visual Studio(R) 2017 Community

## 公開の経緯

作者は、他のアプリの制作や、他の趣味にも打ち込みたく、そのためにはStoryCanvasの迅速な開発を犠牲にしなければいけません。
しかし、ユーザから多数の要望が来ている状況で、これ以上個人で開発することに限界を感じました。
よって、ソースコードを公開し、開発に協力頂ける人を募集します。

## コンパイル方法

StoryCanvasは、このリポジトリのソースそのままではコンパイルすることはできません。
OneDrive、DropboxのAPIキーを取得し、プログラムコード内に設定する必要があります。
なお、両者の機能を使用しない場合は、以下のソースをそのまま（`<Your --->`のまま）コピペして使用することもできます。

### OneDrive APIキー取得

OneDriveのClientId、ClientSecretを別途取得し、`Libraries/StoryCanvas.Native/Common/OneDriveStorageLicense.cs`を新たに作り、以下のソースを記述します。

~~~~
using System;
using System.Collections.Generic;
using System.Text;

namespace Libraries.StoryCanvas.Native.Common
{
    static class OneDriveStorageLicense
    {
        public const string ClientId = "<Your client id>";
        public const string ClientSecret = "<Your client secret>";
    }
}
~~~~

### Dropbox APIキー取得

DropboxのAPIキー、アプリシークレットを別途取得し、`StoryCanvas.Shared/Common/DropboxLicense.cs`を新たに作り、以下のソースを記述します。

~~~~
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Common
{
    static class DropboxLicense
    {
        public const string ApiKey = "<Your API key>";
        public const string AppSecret = "<Your app secret>";
    }
}
~~~~

なお、DropboxのPermission typeは「App folder」、App folder nameは「StoryCanvasCloud」に設定しています。

## 貢献方法

どんな小さいことでも構いません。このリポジトリをフォークして、ソースコードを修正してプルリクエストを送ってください。
StoryCanvas開発方針から逸脱したものでなければ、基本的にどのような機能でも受け入れます。

**皆様のご協力を待っています。**

## ライセンスについて

StoryCanvasは、ソースコードを公開していますが、**オープンソースではありません。**
「StoryCanvasライセンス」という、独自のライセンスを作成・適用しており、再頒布に制限が課せられています。
詳しくは、[LICENSE.md](https://github.com/kmycode/storycanvas-csharp/blob/master/LICENSE.md)を参照してください。
