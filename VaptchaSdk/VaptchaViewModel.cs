using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

namespace VaptchaSdk
{
    public class VaptchaViewModel : ViewModelBase
    {
        private BitmapImage vaptchaImageSource;
        private double sliderValue;
        public BitmapImage VaptchaImageSource
        {
            get => vaptchaImageSource;
            set
            {
                vaptchaImageSource = value;
                OnPropertyChanged();
            }
        }
        public double SliderValue
        {
            get => sliderValue;
            set
            {
                sliderValue = value;
                OnPropertyChanged();
            }
        }
        private Visibility notifyVisibility;
        public Visibility NotifyVisibility
        {
            get => notifyVisibility;
            set
            {
                notifyVisibility = value;
                OnPropertyChanged();
            }
        }
        private Visibility refreshVisibility;
        public Visibility RefreshVisibility
        {
            get => refreshVisibility;
            set
            {
                refreshVisibility = value;
                OnPropertyChanged();
            }
        }
        private Visibility guideVisible;
        public Visibility GuideVisible
        {
            get => guideVisible;
            set
            {
                guideVisible = value;
                OnPropertyChanged();
            }
        }
        private VaptchaMode vaptchaType;
        private string vid;
        public VaptchaMode VaptchaType
        {
            get => vaptchaType;
            set => vaptchaType = value;
        }
        public string Vid
        {
            get => vid;
            set => vid = value;
        }

        public delegate void VaptchaVerifyDelegate(VaptchaVerifyArgs e);
        public delegate void VaptchaFailDelegete(VaptchaFailArgs e);

        public event VaptchaVerifyDelegate VaptchaVerify;
        public event VaptchaFailDelegete VaptchaFail;

        public List<VaptchaPoint> Points { get; set; }

        private readonly HttpClient httpClient;
        private string challenge;

        private string[] cdns;
        private int dv;

        private int sampleLength;
        private int freshCount = 0;
        public VaptchaViewModel()
        {
            Debug.WriteLine("vid" + vid);

            dv = (int)Math.Pow(VaptchaConst.Sample.Length, 2);
            sampleLength = VaptchaConst.Sample.Length;

            httpClient = new HttpClient();

            Points = new List<VaptchaPoint>();

            Vid = ApplicationData.Current.LocalSettings.Values["vid"]?.ToString();
            RefreshVisibility = Visibility.Collapsed;
            NotifyVisibility = Visibility.Collapsed;
            //GetConfigAsync();
        }

        public void GetConfigAsync()
        {
            RefreshVisibility = Visibility.Collapsed;
            NotifyVisibility = Visibility.Collapsed;
            
            var config = GetStringAsync(String.Format(VaptchaUrl.config, Vid, VaptchaType.ToString(), string.Empty));
            var dto = JsonHelper.ToObj<ConfigModel>(config);
            if (dto.code == VaptchaConst.SuccessCode)
            {
                cdns = dto.cdn_servers;
                challenge = dto.challenge;
                GuideVisible = dto.guide ? Visibility.Visible : Visibility.Collapsed;
                switch (dto.type)
                {
                    case VaptchaMode.embed:
                        GetAsync();
                        break;
                    case VaptchaMode.click:
                        ClickAsync();
                        break;
                    case VaptchaMode.invisible:
                        ClickAsync();
                        break;
                }
            }
        }
        public void ClickAsync()
        {
            var click = GetStringAsync(string.Format(VaptchaUrl.click, challenge, Vid));
            var @base = JsonHelper.ToObj<VaptchaModelBase>(click);
            if (@base.code == VaptchaConst.SuccessCode)
            {
                var dto = JsonHelper.ToObj<ClickModel>(click);

            }
            else
            {
                var dto = JsonHelper.ToObj<ClickFailModel>(click);
                ShowVaptcha(dto.img);
                SliderValue = dto.frequency;
            }

        }

        public void GetAsync()
        {
            var get = GetStringAsync(string.Format(VaptchaUrl.get, challenge, Vid));
            var dto = JsonHelper.ToObj<GetModel>(get);
            if (dto.code == VaptchaConst.SuccessCode)
            {
                SliderValue = dto.frequency;
                ShowVaptcha(dto.img);
            }
        }

        public void RefreshAsync()
        {
            if (freshCount >= 5)
            {
                freshCount = 0;
                RefreshVisibility = Visibility.Visible;
            }
            var refresh = GetStringAsync(string.Format(VaptchaUrl.refresh, challenge, Vid));
            var dto = JsonHelper.ToObj<RefreshModel>(refresh);
            SliderValue = dto.frequency;
            ShowVaptcha(dto.img);
            freshCount += 1;
        }
        public void VerifyAsync()
        {
            Debug.WriteLine(GetVerifyData());
            var verify = GetStringAsync(string.Format(VaptchaUrl.verify, GetVerifyData(), Vid, challenge, Points.Last().T));
            var @base = JsonHelper.ToObj<VaptchaModelBase>(verify);
            if (@base.code == VaptchaConst.SuccessCode)
            {
                var dto = JsonHelper.ToObj<VerifyModel>(verify);
                SliderValue = dto.frequency;

                VaptchaVerify.Invoke(new VaptchaVerifyArgs { Code = dto.code, Token = dto.token });

            }
            else
            {
                var dto = JsonHelper.ToObj<VerifyFailModel>(verify);
                SliderValue = dto.frequency;
                VaptchaVerify.Invoke(new VaptchaVerifyArgs { Code = dto.code, Token = string.Empty });
            }
        }

        private void CallVaptchaFail(string msg)
        {
            VaptchaFail.Invoke(new VaptchaFailArgs
            {
                Message = msg
            });
        }
        private void ShowVaptcha(string imgPath)
        {
            if (cdns == null || !cdns.Any())
            {
                return;
            }
            var url = $"https://{cdns[0]}/{imgPath}";
            VaptchaImageSource = new BitmapImage(new Uri(url));
        }

        private string GetStringAsync(string url)
        {
            //Debug.WriteLine(url);
            var http = httpClient.GetStringAsync(new Uri(url));
            var result = http.AsTask().GetAwaiter().GetResult();
            result = result.Remove(result.Length - 1);
            result = result.Remove(0, 2);
            Debug.WriteLine(result);
            return result;


        }
        private string GetVerifyData()
        {
            var sbx = new StringBuilder();
            var sby = new StringBuilder();
            var sbt = new StringBuilder();

            foreach (var x in Points)
            {
                var xx = transform(x.X);
                var xy = transform(x.Y);
                var xt = transform(x.T);
                sbx.Append(xx);
                sby.Append(xy);
                sbt.Append(xt);
            }
            var sbr = new StringBuilder();
            sbr.Append(sbx.ToString());
            sbr.Append(sby.ToString());
            sbr.Append(sbt.ToString());
            return sbr.ToString();
        }
        private string transform(int v)
        {

            var muitiple = Math.Abs(v) / sampleLength;
            if (muitiple >= sampleLength)
            {
                var v1 = (v / dv);
                var r1 = v1 * dv;
                var v2 = ((v - r1) / sampleLength);
                var r2 = v2 * sampleLength;
                var v3 = v - r1 - r2;
                return new string(new[] { VaptchaConst.Sample[v1], VaptchaConst.Sample[v2], VaptchaConst.Sample[v3] });
            }
            else
            {
                var v2 = v / sampleLength;
                var r2 = v2 * sampleLength;
                var v3 = v - r2;
                return new string(new[] { VaptchaConst._, VaptchaConst.Sample[v2], VaptchaConst.Sample[v3] });
            }
        }
    }
}
