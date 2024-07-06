using System.Diagnostics;
using System.IO.Compression;
using CommunityToolkit.Maui.Storage;
using IWshRuntimeLibrary;

namespace GallowsGameInstaller.Pages;

public partial class InstallationPage : ContentPage
{
    Label labelProgressBarProcent;
    ProgressBar progressBar;
    public InstallationPage(string path)
    {
        InitializeComponent();
        ProgressBarInit();
        if (!string.IsNullOrEmpty(path))
        {
            //ExtractAndCopyFiles(@"D:\CSProjects\GallowsGameInstaller\GallowsGameInstaller\ProjectBuildFiles\GallowGame.zip", path);            
            ExtractAndCopyFiles(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\GallowGame.zip"), path);
        }
    }
    public InstallationPage()
    {
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false); //������� ��������� ������ ����������� �� ���������� ��������
        ProgressBarInit();
        ExtractAndCopyFiles(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\GallowGame.zip"), @"C:\Program Files\GallowGame");
    }
    private void ProgressBarInit()
    {
        // �������� ProgressBar
        progressBar = new ProgressBar
        {
            Progress = 0.5, // ���������� ��������� �������� ��������� (0.5 ��� 50%)
            ProgressColor = Colors.Blue // ���������� ���� ��������-���� (��������, �����)
        };
        labelProgressBarProcent = new Label();
        labelProgressBarProcent.Text = "0%";
        labelProgressBarProcent.TextColor = Colors.Black;

        Grid exitGrid = new Grid();
        Image exitImage = new Image();
        exitImage.Source = "thirdbttn.png";
        exitImage.WidthRequest = 130;

        Button exitButton = new Button();
        exitButton.Text = "�����";
        exitButton.WidthRequest = 130;

        exitGrid.Add(exitButton);
        exitGrid.Add(exitImage);
        // �������� Grid ��� ���������� ProgressBar �� ������
        var grid = new Grid();
        grid.WidthRequest = 600;
        grid.HeightRequest = 50;
        grid.Children.Add(labelProgressBarProcent);
        grid.Children.Add(progressBar);
        grid.Children.Add(exitGrid);

        // ���������� ������������ �� ������
        grid.HorizontalOptions = LayoutOptions.Center;
        grid.VerticalOptions = LayoutOptions.Center;

        // �������� grid �� ���� �������� (��������, MainPage)
        Content = grid;
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
            var allFilesTempFolder = Directory.GetFiles(tempFolderPath, "*", SearchOption.AllDirectories);
            int tempFolderFilesCount = allFilesTempFolder.Count();
            int counter = 0;
            foreach (string filePath in allFilesTempFolder)
            {
                counter += 1;
                double shareReadiness = counter / tempFolderFilesCount;
                double shareReadinessRounded = Math.Round(shareReadiness, 1);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    progressBar.Progress = shareReadinessRounded;
                    labelProgressBarProcent.Text = (shareReadiness * 100).ToString() + "%";
                });

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
                CreateShortcut(@"%USERPROFILE%\Desktop\GallowsGame.lnk", destinationDir + @"\GallowsGame.exe"); //���� � ������ ������ � name.lnk / ���� � ������������ �����
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