using GeoSaveMob.ViewModels;

namespace GeoSaveMob;

public partial class MainPage : ContentPage
{
	
	public MainPage()
	{
		BindingContext = new MainVModel();
		InitializeComponent();
	}

	
}

