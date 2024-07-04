using GallowsGameInstaller.Pages;
namespace GallowsGameInstaller;
public partial class MainPage : ContentPage
{

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnExitClicked(object sender, EventArgs e)
    {
        System.Environment.Exit(0);
    }
    private void OnInstallClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new EULAPage(false));
    }
    private void OnInstallAdvanceClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new EULAPage(true));
    }
}