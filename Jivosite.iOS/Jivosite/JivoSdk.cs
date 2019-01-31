using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Jivosite.iOS.Jivosite
{
    public class JivoSdk
    {
        // hackishFixClassName для открытия стандартной клавиатуры
        private static string hackishFixClassName;
        private static IntPtr hackishFixClass;
        private UIWebView _webView;
        private UIView _loadingView;
        private string _language;
        private IJivoDelegate _delegate;

        private UIView HackishlyFoundBrowserView()
        {
            UIScrollView scrollView = _webView.ScrollView;
            UIView browserView = null;
            foreach (UIView subview in scrollView.Subviews)
                if (NSString.FromHandle(subview.ClassHandle).Contains("UIWebBrowserView"))
                {
                    browserView = subview;
                    break;
                }
            return browserView;
        }

        private JivoSdk MethodReturningNil()
        {
            return null;
        }

        private void EnsureHackishSubclassExistsOfBrowserViewClass(IntPtr browserViewClass)
        {
            //if (!hackishFixClass)
            //{
                //IntPtr newClass = objc_allocateClassPair(browserViewClass, hackishFixClassName, 0); //TODO
                //IMP nilImp = this.methodForSelector(__selector(MethodReturningNil));
                //class_addMethod(newClass, __selector(inputAccessoryView), nilImp, "@@:");
                //objc_registerClassPair(newClass);
                //hackishFixClass = newClass;
            //}
        }

        private bool HackishlyHidesInputAccessoryView()
        {
            UIView browserView = this.HackishlyFoundBrowserView();
            return browserView.ClassHandle == hackishFixClass;
        }

        private void SetHackishlyHidesInputAccessoryView(bool val)
        {
            UIView browserView = HackishlyFoundBrowserView();
            if (browserView == null)
            {
                return;
            }
            this.EnsureHackishSubclassExistsOfBrowserViewClass(browserView.ClassHandle);
            if (val)
            {
                //object_setClass(browserView, hackishFixClass);
            }
            else
            {
                //Class normalClass = objc_getClass("UIWebBrowserView");
                //object_setClass(browserView, normalClass);
            }
            browserView.ReloadInputViews();
        }

        private void RemoveBar()
        {
            if (!HackishlyHidesInputAccessoryView())
            {
                SetHackishlyHidesInputAccessoryView(true);
            }
        }

        private void KeyboardWillShow(NSNotification notification)
        {
            //NSLog("keyboardWillShow");
            //this.performSelector(__selector(RemoveBar)) withObject(null) afterDelay(0);
            //CGRect keyboardFrame = notification.UserInfo["UIKeyboardFrameEndUserInfoKey"].CGRectValue();
            //keyboardFrame = _webView.ConvertRectFromView(keyboardFrame, null);
            //NSString script = NSString.stringWithFormat("window.onKeyBoard({visible:true, height:%@}); ", keyboardFrame.size.height.stringValue());
            //_webView.stringByEvaluatingJavaScriptFromString(script);
        }

        private void KeyboardDidShow(NSNotification notification)
        {
            //NSLog("keyboardDidShow");
            //CGRect keyboardFrame = notification.userInfo[UIKeyboardFrameEndUserInfoKey].CGRectValue();
            //keyboardFrame = _webView.convertRect(keyboardFrame) fromView(null);
            //this.execJs(NSString.stringWithFormat("window.scrollTo(0, %@);", keyboardFrame.size.height.stringValue()));
        }

        private void KeyboardWillHide(NSNotification notification)
        {
            //NSLog("keyboardWillHide");
            //CGRect frame = _webView.frame;
            //frame.origin.y = 0;
            //UIView.animateWithDuration(0.3) animations(() => {
            //    _webView.frame = frame;
            //});
            //_webView.stringByEvaluatingJavaScriptFromString("window.onKeyBoard({visible:false, height:0});");
        }

        private void KeyboardDidHide(NSNotification notification)
        {
            //NSLog("keyboardDidHide");
        }

        private void CreateLoader()
        {
            var kls = new Class("");
            _loadingView = new UIView(new CGRect(100, 400, 80, 80));
            //var loadingView = UIView.Alloc(kls).Init(); //CGRect.FromLTRB(100, 400, 80, 80)
            _loadingView.BackgroundColor = UIColor.White;
            _loadingView.Layer.CornerRadius = 5;
            UIActivityIndicatorView activityView = new UIActivityIndicatorView();
                                    //.alloc().initWithActivityIndicatorStyle(UIActivityIndicatorViewStyleWhiteLarge);
            activityView.Center = new CGPoint(_loadingView.Frame.Size.Width / 2.0, 35);
            activityView.StartAnimating();
            activityView.Tag = 100;
            _loadingView.AddSubview(activityView);
            UILabel lblLoading = new UILabel(new CGRect(0, 48, 80, 30));
            lblLoading.Text = "Loading";//NSBundle.MainBundle.LocalizedStringForKey("Loading") @value("") table(null);
            lblLoading.TextColor = UIColor.White;
            //lblLoading.Font = UIFont.FontWithName(lblLoading.font.fontName) size(15);
            lblLoading.TextAlignment = UITextAlignment.Center;
            _loadingView.AddSubview(lblLoading);
            _webView.AddSubview(_loadingView);
            _loadingView.Center = (UIApplication.SharedApplication.KeyWindow.Center);
        }

        private void DeregisterForKeyboardNotifications()
        {
            NSNotificationCenter center = NSNotificationCenter.DefaultCenter;
            //center.RemoveObserver(this) name(UIKeyboardWillShowNotification) @object(null);
            //center.RemoveObserver(this) name(UIKeyboardDidShowNotification) @object(null);
            //center.RemoveObserver(this) name(UIKeyboardWillHideNotification) @object(null);
            //center.RemoveObserver(this) name(UIKeyboardDidHideNotification) @object(null);
        }

        private void Dealloc()
        {
            this.DeregisterForKeyboardNotifications();
        }

        private string DecodeString(string encodedString)
        {
            //string decodedString = encodedString.stringByReplacingPercentEscapesUsingEncoding(NSUTF8StringEncoding);
            return encodedString;
        }

        private void WebViewDidStartLoad(UIWebView webView)
        {
            _loadingView.Hidden = false;
        }

        private void WebViewDidFinishLoad(UIWebView webView)
        {
            _loadingView.Hidden = true;
            this.RemoveBar();
        }

        private bool WebView(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            NSUrl url = request.Url;
            if (url.Scheme.ToLower() != "jivoapi") return true;

            string[] components = url.AbsoluteString.Replace("jivoapi://", "").Split("/"); //.ComponentsSeparatedByString("/");
            string apiKey = components[0];
            string data = "";
            if (components.Length > 1)
            {
                data = this.DecodeString(components[1]);
            }
            _delegate.OnEvent(apiKey, data);
            return true;
        }

        public JivoSdk(UIWebView web)
        {
            _webView = web;
            _language = "";
        }

        public JivoSdk(UIWebView web, NSString lang)
        {
            _webView = web;
            _language = lang;
        }

        private void Prepare()
        {
            //NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("keyboardWillShow"), ) selector(__selector(keyboardWillShow:)) name(UIKeyboardWillShowNotification) @object(null);
            //NSNotificationCenter.DefaultCenter.AddObserver(this) selector(__selector(keyboardWillHide:)) name(UIKeyboardWillHideNotification) @object(null);
            //NSNotificationCenter.DefaultCenter.AddObserver(this) selector(__selector(keyboardDidHide:)) name(UIKeyboardDidHideNotification) @object(null);
            //NSNotificationCenter.DefaultCenter.AddObserver(this) selector(__selector(keyboardDidShow:)) name(UIKeyboardDidShowNotification) @object(null);
            _webView.Delegate = new UIWebViewDelegate();//this;
        }

        private void Start()
        {
            this.CreateLoader();
            _webView.ScrollView.ScrollEnabled = false;
            _webView.ScrollView.Bounces = false;
            _webView.ScrollView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;
            var indexFile = _language.Length > 0 ? $"index_%{_language})" : "index";

            string htmlFile = NSBundle.MainBundle.PathForResource(indexFile, "html", "/html");
            string htmlContent = System.IO.File.ReadAllText(htmlFile);
            //NSString htmlString = NSString.stringWithContentsOfFile(htmlFile) encoding(NSUTF8StringEncoding) error(null);
            NSString htmlString = new NSString(htmlContent, NSStringEncoding.UTF8);
            _webView.LoadHtmlString(htmlString, NSUrl.CreateFileUrl(new []{"%@/html/", NSBundle.MainBundle.BundlePath}));
        }

        private void Stop()
        {
            this.DeregisterForKeyboardNotifications();
        }

        private void ExecJs(string code)
        {
            _webView.EvaluateJavascript("javascript:" + code);
            //_webView.EvaluateJavascript(code);
        }

        public void CallApiMethod(string methodName, string data)
        {
            _webView.EvaluateJavascript($"window.jivo_api.{methodName}({data});");
                //stringByEvaluatingJavaScriptFromString(NSString.stringWithFormat("window.jivo_api.%@(%@);", methodName, data));
        }
    }
}