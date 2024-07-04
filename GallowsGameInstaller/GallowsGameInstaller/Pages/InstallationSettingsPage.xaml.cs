using System.Diagnostics;
using System.IO.Compression;
using CommunityToolkit.Maui.Storage;
using IWshRuntimeLibrary;

namespace GallowsGameInstaller.Pages;

public partial class InstallationSettingsPage : ContentPage
{
    public InstallationSettingsPage()
	{
		InitializeComponent();
    }
    private async void PathSelectClicked(object sender, EventArgs e)
    {
        await PickFolder(CancellationToken.None);
    }
    async Task PickFolder(CancellationToken cancellationToken)
    {
        var result = await FolderPicker.Default.PickAsync(cancellationToken);
        if (result.IsSuccessful)
        {
            // ���� � ��������� �����
            string folderPath = result.Folder.Path;
            PathEntry.Text = folderPath;
            // ��������� folderPath � ���� ����������
            // ...
        }
        else
        {
            // ��������� ������
            string errorMessage = result.Exception.Message;
            // ...
        }
    }
    private async void InstallClicked(object sender, EventArgs e)
    {
        string path = PathEntry.Text;
        await Navigation.PushAsync(new InstallationPage(path));
    }

}