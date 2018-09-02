using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Vaptcha.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ApplicationData.Current.LocalSettings.Values["vid"] = "5b222656a485d43b546b5212";
        }

        private void vaptcha_VaptchaVerify(VaptchaSdk.VaptchaVerifyArgs e)
        {
            Debug.WriteLine("code : " + e.Code);
            Debug.WriteLine("token : " + e.Token);
        }

        private void vaptcha_VaptchaFail(VaptchaSdk.VaptchaFailArgs e)
        {

        }
    }
}
