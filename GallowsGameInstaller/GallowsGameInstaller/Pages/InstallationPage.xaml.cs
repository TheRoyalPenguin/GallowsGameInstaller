using System.Diagnostics;
using System.IO.Compression;
using CommunityToolkit.Maui.Storage;
using IWshRuntimeLibrary;

namespace GallowsGameInstaller.Pages;

public partial class InstallationPage : ContentPage
{
    public InstallationPage(string path)
    {
        InitializeComponent();
        ProgressBarInit();
        if (!string.IsNullOrEmpty(path))
        {
            ExtractAndCopyFiles(@"D:\CSProjects\GallowsGameInstaller\GallowsGameInstaller\ProjectBuildFiles\GallowGame.zip", path);
        }
    }
    public InstallationPage()
    {
        InitializeComponent();
        ProgressBarInit();
        ExtractAndCopyFiles(@"D:\CSProjects\GallowsGameInstaller\GallowsGameInstaller\ProjectBuildFiles\GallowGame.zip", @"C:\Program Files\GallowGame");
    }
    private void ProgressBarInit()
    {
        // �������� ProgressBar
        ProgressBar progressBar = new ProgressBar
        {
            Progress = 0.5, // ���������� ��������� �������� ��������� (0.5 ��� 50%)
            ProgressColor = Colors.Blue // ���������� ���� ��������-���� (��������, �����)
        };
        Label label = new Label();
        label.Text = "0%";
        label.TextColor = Colors.Black;
        // �������� Grid ��� ���������� ProgressBar �� ������
        var grid = new Grid();
        grid.WidthRequest = 600;
        grid.HeightRequest = 50;
        grid.Children.Add(label);
        grid.Children.Add(progressBar);

        // ���������� ������������ �� ������
        grid.HorizontalOptions = LayoutOptions.Center;
        grid.VerticalOptions = LayoutOptions.Center;

        // �������� grid �� ���� �������� (��������, MainPage)
        Content = grid;
        Task.Run(() => progressBarTextUpdate(label, progressBar));
    }
    private void progressBarTextUpdate(Label label, ProgressBar progressBar)
    {
        while (true)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                label.Text = progressBar.Progress.ToString();
            });
        }
    }
    private void ExtractAndCopyFiles(string relativeZipFilePath, string relativeDestinationFolderPath)
    {
        // �������� ������� ���������� �s������
        string projectDirectory = Directory.GetCurrentDirectory();

        // ��������� ������ ���� � ��������� ����� � ����� ����������
        string zipFilePath = Path.Combine(projectDirectory, relativeZipFilePath);
        string destinationFolderPath = Path.Combine(projectDirectory, relativeDestinationFolderPath);

        // ���������, ���������� �� �������� ����
        if (!System.IO.File.Exists(zipFilePath))
        {
            throw new FileNotFoundException($"���� �� ������: {zipFilePath}");
        }

        // ������� ����� ����������, ���� ��� �� ����������
        if (!Directory.Exists(destinationFolderPath))
        {
            Directory.CreateDirectory(destinationFolderPath);
        }

        // ������� ��������� ����� ��� ���������� ������
        string tempFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempFolderPath);

        try
        {
            // ������������� ����� �� ��������� �����
            ZipFile.ExtractToDirectory(zipFilePath, tempFolderPath);

            // ���������� �� ���� ������ �� ��������� ����� � �������� �� � ����� ����������
            foreach (string filePath in Directory.GetFiles(tempFolderPath, "*", SearchOption.AllDirectories))
            {
                // ���������� ������������� ���� ����� �� ��������� �����
                string relativePath = filePath.Substring(tempFolderPath.Length + 1);
                // ��������� ������ ���� � ����� � ����� ����������
                string destinationPath = Path.Combine(destinationFolderPath, relativePath);

                // ��������, ��� ���������� ���������� ����������, � ������� �, ���� ���
                string destinationDir = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                }

                // �������� ���� �� ��������� ����� � ����� ����������
                System.IO.File.Copy(filePath, destinationPath, true);
                Debug.Write(destinationDir);
                Debug.Write(destinationPath);
                CreateShortcut(@"%USERPROFILE%\Desktop\GallowGame.lnk", destinationDir + @"\GallowGame.exe"); //���� � ������ ������ � name.lnk / ���� � ������������ �����
            }
        }
        finally
        {
            // ������� ��������� ����� � ��� � ����������
            Directory.Delete(tempFolderPath, true);
        }
    }

    public static void CreateShortcut(string shortcutPath, string targetPath)
    {
        shortcutPath = Environment.ExpandEnvironmentVariables(shortcutPath);
        WshShell wshShell = new WshShell();
        IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(shortcutPath);
        shortcut.TargetPath = targetPath;
        shortcut.Save();
    }
}