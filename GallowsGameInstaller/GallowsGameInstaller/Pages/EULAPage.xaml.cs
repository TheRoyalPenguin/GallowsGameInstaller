namespace GallowsGameInstaller.Pages;

public partial class EULAPage : ContentPage
{
    bool isAdvancedInstallation;

    public EULAPage(bool isAdvancedInstallation)
	{
		InitializeComponent();
        this.isAdvancedInstallation = isAdvancedInstallation;

    }

    private async void AccesButton(object sender, EventArgs e)
    {
        await Navigation.PushAsync(isAdvancedInstallation ? new InstallationSettingsPage() : new InstallationPage());
    }
}