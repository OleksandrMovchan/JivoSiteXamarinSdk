using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Webkit;
using Jivosite.Jivosite;

namespace Jivosite
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IJivoDelegate
    {
        private JivoSdk _jivoSdk;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var webView = FindViewById<WebView>(Resource.Id.jivoWebView);
            _jivoSdk = new JivoSdk(webView, this);
            _jivoSdk.Prepare();
        }

        public void OnEvent(string name, string data)
        {
            Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(data));
            if (name.Equals("url.click"))
            {
                StartActivity(browserIntent);
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

