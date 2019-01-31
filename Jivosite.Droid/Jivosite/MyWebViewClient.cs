using System;
using System.Text;
using Android.Webkit;

namespace Jivosite.Jivosite
{
    class MyWebViewClient : WebViewClient
    {
        private readonly IJivoDelegate _jivoDelegate;

        public MyWebViewClient(IJivoDelegate jivoDelegate)
        {
            this._jivoDelegate = jivoDelegate;
        }

        public override bool ShouldOverrideUrlLoading(WebView view, String url)
        {
            if (url.ToLower().IndexOf("jivoapi://") != 0)
            {
                return true;
            }

            string[] components = url.Replace("jivoapi://", "").Split('/');

            var apiKey = components[0];
            var data = "";
            if (components.Length > 1)
            {
                data = DecodeString(components[1]);
            }

            _jivoDelegate?.OnEvent(apiKey, data);

            return true;

            // Otherwise, the link is not for a page on my site, so launch another Activity that handles URLs
            //Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
            //StartActivity(intent);
        }
        
        private static string DecodeString(string encodedUri)
        {
            StringBuilder buffer = new StringBuilder();

            var sumb = 0;

            for (int i = 0, more = -1; i < encodedUri.Length; i++)
            {
                var actualChar = encodedUri[i];

                int bytePattern;
                switch (actualChar)
                {
                    case '%':
                        {
                            actualChar = encodedUri[++i];
                            int hb = (char.IsDigit(actualChar) ? actualChar - '0'
                                    : 10 + char.ToLower(actualChar) - 'a') & 0xF;
                            actualChar = encodedUri[++i];
                            int lb = (char.IsDigit(actualChar) ? actualChar - '0'
                                    : 10 + char.ToLower(actualChar) - 'a') & 0xF;
                            bytePattern = (hb << 4) | lb;
                            break;
                        }
                    case '+':
                        {
                            bytePattern = ' ';
                            break;
                        }
                    default:
                        {
                            bytePattern = actualChar;
                            break;
                        }
                }

                if ((bytePattern & 0xc0) == 0x80)
                { // 10xxxxxx
                    sumb = (sumb << 6) | (bytePattern & 0x3f);
                    if (--more == 0)
                        buffer.Append((char)sumb);
                }
                else if ((bytePattern & 0x80) == 0x00)
                { // 0xxxxxxx
                    buffer.Append((char)bytePattern);
                }
                else if ((bytePattern & 0xe0) == 0xc0)
                { // 110xxxxx
                    sumb = bytePattern & 0x1f;
                    more = 1;
                }
                else if ((bytePattern & 0xf0) == 0xe0)
                { // 1110xxxx
                    sumb = bytePattern & 0x0f;
                    more = 2;
                }
                else if ((bytePattern & 0xf8) == 0xf0)
                { // 11110xxx
                    sumb = bytePattern & 0x07;
                    more = 3;
                }
                else if ((bytePattern & 0xfc) == 0xf8)
                { // 111110xx
                    sumb = bytePattern & 0x03;
                    more = 4;
                }
                else
                { // 1111110x
                    sumb = bytePattern & 0x01;
                    more = 5;
                }
            }
            return buffer.ToString();
        }
    }

}