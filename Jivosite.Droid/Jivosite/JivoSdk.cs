using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;

namespace Jivosite.Jivosite
{
    public class JivoSdk
    {
        private readonly WebView _webView;
        private readonly IJivoDelegate _jivoDelegate;
        
        public JivoSdk(WebView webView, IJivoDelegate jivoDelegate)
        {
            _webView = webView;
            _jivoDelegate = jivoDelegate;
        }

        public void Prepare()
        {
            DisplayMetrics dm = new DisplayMetrics();
            ((Activity)_jivoDelegate).GetSystemService(Context.WindowService).JavaCast<IWindowManager>().DefaultDisplay.GetMetrics(dm);
            float density = dm.Density;

            OnGlobalLayoutListener list = new OnGlobalLayoutListener(this._webView, density);

            _webView.ViewTreeObserver.AddOnGlobalLayoutListener(list);

            WebSettings webSettings = _webView.Settings;
            webSettings.JavaScriptEnabled = true;
            webSettings.DomStorageEnabled = true;
            webSettings.DatabaseEnabled = true;
            webSettings.JavaScriptCanOpenWindowsAutomatically = true;

            _webView.AddJavascriptInterface(new JivoInterface(_jivoDelegate), "JivoInterface");
            _webView.SetWebViewClient(new MyWebViewClient(_jivoDelegate));

            _webView.LoadUrl("file:///android_asset/html/index.html");
        }
        
        public void CallApiMethod(string methodName, string data)
        {
            _webView.LoadUrl("javascript:window.jivo_api." + methodName + "(" + data + ");");
        }
    }

}