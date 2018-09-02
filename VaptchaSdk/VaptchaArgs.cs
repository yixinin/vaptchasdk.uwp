using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaptchaSdk
{
    public class VaptchaVerifyArgs : EventArgs
    {
        public string Token { get; set; }
        public string Code { get; set; }
    }
    public class VaptchaFailArgs : EventArgs
    {
        public string Message { get; set; }
    }
     
}
