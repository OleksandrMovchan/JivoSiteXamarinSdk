using System;
using Jivosite.iOS.Jivosite;
using UIKit;

namespace Jivosite.iOS
{
    public partial class ViewController : UIViewController, IJivoDelegate
    {
        private JivoSdk _jivoSdk;

        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.
            var webView = new UIWebView();
            View.AddSubview(webView);

            _jivoSdk = new JivoSdk(webView);

        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }

        public void OnEvent(string name, string data)
        {
             
            //Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(data));
            if (name.Equals("url.click"))
            {
                //NavigationController.PushViewController();
                //StartActivity(browserIntent);
            }

            if (name.Equals("chat.ready"))
            {
                _jivoSdk.CallApiMethod("open", "");
            }

            if (name.Equals("agent.message"))
            {
                string str = data;
            }
        }
    }
}