using System.ComponentModel;
using System.Runtime.CompilerServices;
using AdjustSdk;
using Android.Webkit;
using pro1006002.Model;
using pro1006002.Utils;

namespace pro1006002;

public partial class WgViewPage : ContentPage, INotifyPropertyChanged
{
    private bool _isLoading = true;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
                _ = AnimateLoadingAsync(value);
            }
        }
    }

    public WgViewPage()
    {
        InitializeComponent();
        BindingContext = this;
        NavigationPage.SetHasNavigationBar(this, false);

        LoadingOverlay.Opacity = 1;
        LoadingCard.Opacity = 1;

        if (!string.IsNullOrEmpty(Storeage.Model.UserAgent))
        {
            webView.UserAgent = Storeage.Model.UserAgent;
        }

        webView.Source = Storeage.Model.Url;

        Adjust.GetAdid((adid) => { Storeage.AdId = adid; });
    }

    private async Task AnimateLoadingAsync(bool show)
    {
        try
        {
            if (show)
            {
                LoadingOverlay.IsVisible = true;
                LoadingOverlay.Opacity = 0;
                LoadingCard.Opacity = 0;
                LoadingCard.Scale = 0.9;

                await Task.WhenAll(
                    LoadingOverlay.FadeTo(1, 200, Easing.SinInOut),
                    LoadingCard.FadeTo(1, 250, Easing.SinInOut),
                    LoadingCard.ScaleTo(1, 250, Easing.SinOut)
                );
            }
            else
            {
                await Task.WhenAll(
                    LoadingOverlay.FadeTo(0, 200, Easing.SinInOut),
                    LoadingCard.FadeTo(0, 180, Easing.SinIn),
                    LoadingCard.ScaleTo(0.9, 180, Easing.SinIn)
                );
                LoadingOverlay.IsVisible = false;
            }
        }
        catch
        {
        }
    }

    protected override bool OnBackButtonPressed()
    {
        if (IsLoading)
        {
            IsLoading = false;
        }

        if (webView.CanGoBack)
        {
            webView.GoBack();
        }

        return true;
    }

    private void WebView_OnNavigating(object? sender, WebNavigatingEventArgs e)
    {
        IsLoading = true;
        try
        {
            var appid = AppInfo.Current.PackageName;
            var googleAdId = Storeage.GoogleAdId;
            var adid = Storeage.AdId;
            webView.EvaluateJavaScriptAsync(
                $"window.adid='{adid}';window.gps_adid='{googleAdId}';window.appid='{appid}';{Storeage.Model.Config} ");
            if (e.Url.Contains(".apk"))
            {
                Launcher.OpenAsync(e.Url);
                e.Cancel = true;
            }

            if (!e.Url.StartsWith("http"))
            {
                e.Cancel = true;
                var uri = new Uri(e.Url);
                Launcher.Default.TryOpenAsync(uri);
            }

            if (e.Url.Contains("myapp=1"))
            {
                e.Cancel = true;
                Launcher.OpenAsync(e.Url);
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void WebView_OnNavigated(object? sender, WebNavigatedEventArgs e)
    {
        IsLoading = false;
        try
        {
            var appid = AppInfo.Current.PackageName;
            var googleAdId = Storeage.GoogleAdId;
            var adid = Storeage.AdId;
            webView.EvaluateJavaScriptAsync(
                $"window.adid='{adid}';window.gps_adid='{googleAdId}';window.appid='{appid}';{Storeage.Model.Config} ");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}