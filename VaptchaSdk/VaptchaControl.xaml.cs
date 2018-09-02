using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace VaptchaSdk
{
    public sealed partial class VaptchaControl : UserControl
    {

        #region

        
        #endregion


        private VaptchaViewModel Vm { get; set; }

        public delegate void VaptchaVerifyDelegate(VaptchaVerifyArgs e);
        public delegate void VaptchaFailDelegete(VaptchaFailArgs e);

        public event VaptchaVerifyDelegate VaptchaVerify;
        public event VaptchaFailDelegete VaptchaFail;

        private bool OnDraw = false;

        public VaptchaControl()
        {
           
            this.InitializeComponent();
            Vm = new VaptchaViewModel();
            Vm.GetConfigAsync();
            Vm.VaptchaFail += Vm_VaptchaFail;
            Vm.VaptchaVerify += Vm_VaptchaVerify; 
        }

        private void init()
        {
            if (Width == 0)
            {
                root.Width = 400;
                ProgressBar.Width = 400;
            }
            else
            {
                root.Width = Width;
                ProgressBar.Width = Width;
            }
            if (Height == 0)
            {
                root.Height = 230;
            }
            else
            {
                root.Height = Height;
            }
        }

        private void Vm_VaptchaVerify(VaptchaVerifyArgs e)
        {
            this.VaptchaVerify.Invoke(e);
        }

        private void Vm_VaptchaFail(VaptchaFailArgs e)
        {
            this.VaptchaFail.Invoke(e);
        }

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var t = this.TransformToVisual(Window.Current.Content);
            var screenCoords = t.TransformPoint(new Point(0, 0));
            canvas.Width = Window.Current.Bounds.Width;
            canvas.Height = Window.Current.Bounds.Height;
            canvas.Margin = new Thickness(-screenCoords.X, -screenCoords.Y, 0, 0);
            OnDraw = true;
            Vm.Points = new List<VaptchaPoint>();
        }

        private void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (OnDraw)
                EndDraw();
        }

        private void canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (OnDraw)
                EndDraw();
        }

        private void EndDraw()
        {
            OnDraw = false;
            canvas.Width = 430;
            canvas.Height = 250;
            canvas.Margin = new Thickness(0,20,0,0);
            Debug.WriteLine("end");
            OnDraw = false;


            if (Vm.Points.Any())
            {
                var start = Vm.Points[0].T;
                Vm.Points[0].T = 0;
                //Vm.Points[Vm.Points.Count - 1].T = (Vm.Points[Vm.Points.Count - 1].T  - start);
                for (var i = 1; i < Vm.Points.Count; i++)
                {
                    var t = (Vm.Points[i].T - start) / 1000;
                    Vm.Points[i].T = t; 
                }
                Vm.VerifyAsync();
            }
            Debug.WriteLine(JsonHelper.ToJson(Vm.Points));
            line.Points.Clear();
            
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (OnDraw)
            {
                var point = e.GetCurrentPoint(sender as Canvas);

                var length = Vm.Points.Count;
                if (length == 0 || (int)point.Timestamp - Vm.Points[length - 1].T > 17000)
                {
                    var position = point.Position;

                    line.Points.Add(position);
                    Vm.Points.Add(new VaptchaPoint
                    {
                        X = (int)position.X,
                        Y = (int)position.Y,
                        T = (int)point.Timestamp
                    });
                }

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Vm.RefreshAsync();
        }

        private void Hyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            Vm.GetConfigAsync();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        { 
            Launcher.LaunchUriAsync(new Uri(VaptchaUrl.Guide)).AsTask();
        }
    }
}
