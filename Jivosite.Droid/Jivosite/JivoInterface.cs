using Android.Webkit;
using Java.Interop;

namespace Jivosite.Jivosite
{
    public class JivoInterface : Java.Lang.Object
    {
        private readonly IJivoDelegate _jivoDelegate;
        
        public JivoInterface(IJivoDelegate jivoDelegate)
        {
            _jivoDelegate = jivoDelegate;
        }

        [Export("send")]
        [JavascriptInterface]
        public void Send(string name, string data)
        {
            _jivoDelegate?.OnEvent(name, data);
        }
    }

}