using Android.Graphics;
using Android.Views;
using Android.Webkit;

namespace Jivosite.Jivosite
{
    public class OnGlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        //public IntPtr Handle => throw new NotImplementedException();

        private readonly WebView _webView;
        private readonly float _density;
        private int _previousHeightDiff = 0;

        public OnGlobalLayoutListener(WebView webView, float density)
        {
            _webView = webView;
            _density = density;
        }
        
        public void OnGlobalLayout()
        {
            Rect r = new Rect();
            //r will be populated with the coordinates of your view that area still visible.
            _webView.GetWindowVisibleDisplayFrame(r);

            int heightDiff = _webView.RootView.Height - r.Bottom;
            int pixelHeightDiff = (int)(heightDiff / _density);
            if (pixelHeightDiff > 100 && pixelHeightDiff != _previousHeightDiff)
            { // if more than 100 pixels, its probably a keyboard...
                //String msg = "S" + Integer.toString(pixelHeightDiff);
                ExecJS("window.onKeyBoard({visible:false, height:0})");
            }
            else if (pixelHeightDiff != _previousHeightDiff && (_previousHeightDiff - pixelHeightDiff) > 100)
            {
                //String msg = "H";
                ExecJS("window.onKeyBoard({visible:false, height:0})");
            }
            _previousHeightDiff = pixelHeightDiff;
        }

        public void ExecJS(string script)
        {
            _webView.LoadUrl("javascript:" + script);
        }
    }
}