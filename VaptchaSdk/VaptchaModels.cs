using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaptchaSdk
{
    public class ConfigModel
    {
        public string api_server { get; set; }
        public string[] cdn_servers { get; set; }
        public string challenge { get; set; }
        public string code { get; set; }
        public bool guide { get; set; }
        public string logo { get; set; }
        public string msg { get; set; }
        public VaptchaMode type { get; set; }
        public int vip { get; set; }
    }

    public class GetModel : RefreshModel
    {
        public string code { get; set; }
        public int passline { get; set; }
    }

    public class ClickModel : VaptchaModelBase
    {
        public string msg { get; set; }
        public string token { get; set; }
        public int passline { get; set; }
    }
    public class ClickFailModel : GetModel
    {
        public string msg { get; set; }
        public int score { get; set; }
    }
    public class VerifyModel: VerifyFailModel
    {

        public int score { get; set; }

        public string token { get; set; }
    }
    public class VerifyFailModel : VaptchaModelBase
    {
        public int evs { get; set; }
        public double frequency { get; set; }
        public int passline { get; set; }
        public string similarity { get; set; }
    }
    public class RefreshModel
    {
        public string adlink { get; set; }
        public string linkword { get; set; }
        public string coverimg { get; set; }
        public double frequency { get; set; }
        public string img { get; set; }
    }
    public class VaptchaModelBase
    {
        public string code { get; set; }

    }

    public class VaptchaConst
    {
        public static readonly string Sample = "abcdefgh234lmntuwxyz";
        public static readonly string SuccessCode = "0103";
        public static readonly string FailCode = "0104";
        public static readonly char _ = '_';
    }

    public class VaptchaUrl
    {
        internal static readonly string config = "https://api.vaptcha.com/v2/config?id={0}&type={1}&scene={2}&callback=v";
        internal static readonly string get = "https://api.vaptcha.com/v2/get2?challenge={0}&id={1}&callback=v";
        internal static readonly string click = "https://api.vaptcha.com/v2/click2?challenge={0}&id={1}&callback=v";
        internal static readonly string verify = "https://api.vaptcha.com/v2/verify?v={0}&w=&id={1}&challenge={2}&drawtime={3}&callback=v";
        internal static readonly string refresh = "https://api.vaptcha.com/v2/refresh?challenge={0}&id={1}&callback=v";
        internal static readonly string Guide = @"https://www.vaptcha.com/document/faq#%E6%93%8D%E4%BD%9C%E6%BC%94%E7%A4%BA";
    }

}
