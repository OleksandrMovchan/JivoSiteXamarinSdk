using System;
using Foundation;
using UIKit;

namespace Jivosite.iOS.Jivosite
{
    public class JivoSdk
    {
        //private Type hackishFixClassType = null;
        private Type hackingFixType = null;

        private UIWebView _webView;
        private string _language;


        public JivoSdk(UIWebView webView, string language = null)
        {
            _webView = webView;
            _language = language;
        }

        public UIView HackishlyFoundBrowserView()
        {
            UIScrollView scrollView = _webView.ScrollView;
            UIView browserView = null;
            foreach (var subview in scrollView.Subviews)
            {
                if (subview.GetType() == hackingFixType) // UIWebBrowserView in obj-c, verify
                {
                    browserView = subview;
                    break;
                }
            }

            return browserView;
        }

        public void EnsureHackishSubclassExistsOfBrowserViewClass()
        {
            //todo add if
            var nullImp = 0;
        }

        public bool HackishlyHidesInputAccessoryView()
        {
            UIView browserView = HackishlyFoundBrowserView();
            return browserView is UIWebView; //hackishFixClass
        }



        public void Start()
        {
            CreateLoader();
            _webView.ScrollView.ScrollEnabled = false;
            _webView.ScrollView.Bounces = false;

            _webView.ScrollView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;

            string indexFile;
            indexFile = _language.Length > 0 ? $"index_{_language}" : "index";
            string htmlFile = NSBundle.MainBundle.PathForResource(indexFile, "html", "/html");
            _webView.LoadHtmlString(htmlFile, NSUrl.CreateFileUrl("/html/", NSBundle.MainBundle.BundleUrl)); //TODO Not sure
        }

        private void CreateLoader()
        {
            throw new NotImplementedException();
        }
    }
}