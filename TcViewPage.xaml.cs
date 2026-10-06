using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdjustSdk;
using Android.Webkit;
using pro1006002.Model;

namespace pro1006002;

public partial class TcViewPage : ContentPage
{
    public TcViewPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        Adjust.GetAdid((adid) =>
        {
            Storeage.AdId = adid;
            webView.UserAgent = $"{WebSettings.GetDefaultUserAgent(Android.App.Application.Context)}/{DeviceInfo.Current.Manufacturer}/{Storeage.Model.UserAgent} UUID/{Storeage.AdId}";
            webView.Source = Storeage.Model.Url;
        });

    }

    protected override bool OnBackButtonPressed()
    {
        if (webView.CanGoBack)
        {
            webView.GoBack();
        }

        return true;
    }

    private void WebView_OnNavigating(object? sender, WebNavigatingEventArgs e)
    {
        if (e.Url.Contains(".apk"))
        {
            Launcher.OpenAsync(e.Url);
        }
    }
}