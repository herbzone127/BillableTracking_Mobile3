using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using MauiAuth;
using System;

namespace BillableTracking_Mobile3
{
    [Activity(Label = "CustomUrlSchemeInterceptorActivity",
              NoHistory = true,
              LaunchMode = LaunchMode.SingleTop,
              Exported = true)]
    [IntentFilter(
        actions: new[] { Intent.ActionView },
        Categories = new[]
        {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataSchemes = new[]
        {
            "com.googleusercontent.apps.499023355628-6cr2dtg7u8e9qmdjtu9mtmcuiusj5upg", // Google
            "msauth" // Microsoft
        },
       
        DataPaths = new[]
        {
            "/oauth2redirect", // Google path
            "/RDRZClQyYa5m1aE9OsIqGQsK32A=" // Microsoft path
        }
    )]
    public class CustomUrlSchemeInterceptorActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Android.Net.Uri uri_android = Intent?.Data;
            if (uri_android == null)
            {
                Finish();
                return;
            }

#if DEBUG
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("CustomUrlSchemeInterceptorActivity.OnCreate()");
            sb.Append("     uri_android = ").AppendLine(uri_android.ToString());
            System.Diagnostics.Debug.WriteLine(sb.ToString());
#endif

            Uri uri_netfx = new Uri(uri_android.ToString());

            if (OAuth2Authenticator.Instance != null)
            {
                OAuth2Authenticator.Instance.HandleRedirectUri(uri_netfx);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OAuth2Authenticator.Instance is null!");
            }

            var intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);

            Finish();
        }
    }
}