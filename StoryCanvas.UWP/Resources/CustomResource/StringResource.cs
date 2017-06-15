using StoryCanvas.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Resources;

namespace StoryCanvas.UWP.Resources.CustomResource
{
    /// <summary>
    /// 指定したキーに見合った文字列リソースを返す。
    /// Xamarin.Forms向けの文字列リソースファイルをそのままUWPで流用するには
    /// リソースのキー名に「.Text」などプロパティ名をつける必要が有るなど問題が起こるため、
    /// このようなクラスを作って利用している。
    /// この機能は、XAML内のCustomResourceから呼び出すことができる
    /// </summary>
    class StringResource : CustomXamlResourceLoader
    {
        protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType)
        {
            return StringResourceResolver.Resolve(resourceId);
        }
    }
}
