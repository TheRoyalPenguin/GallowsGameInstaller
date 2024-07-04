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
        // Создайте ProgressBar
        ProgressBar progressBar = new ProgressBar
        {
            Progress = 0.5, // Установите начальное значение прогресса (0.5 для 50%)
            ProgressColor = Colors.Blue // Установите цвет прогресс-бара (например, синий)
        };
        Label label = new Label();
        label.Text = "0%";
        label.TextColor = Colors.Black;
        // Создайте Grid для размещения ProgressBar по центру
        var grid = new Grid();
        grid.WidthRequest = 600;
        grid.HeightRequest = 50;
        grid.Children.Add(label);
        grid.Children.Add(progressBar);

        // Установите выравнивание по центру
        grid.HorizontalOptions = LayoutOptions.Center;
        grid.VerticalOptions = LayoutOptions.Center;

        // Добавьте grid на вашу страницу (например, MainPage)
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
        // Получаем текущую директорию пsроекта
        string projectDirectory = Directory.GetCurrentDirectory();

        // Формируем полный путь к архивному файлу и папке назначения
        string zipFilePath = Path.Combine(projectDirectory, relativeZipFilePath);
        string destinationFolderPath = Path.Combine(projectDirectory, relativeDestinationFolderPath);

        // Проверяем, существует ли архивный файл
        if (!System.IO.File.Exists(zipFilePath))
        {
            throw new FileNotFoundException($"Файл не найден: {zipFilePath}");
        }

        // Создаем папку назначения, если она не существует
        if (!Directory.Exists(destinationFolderPath))
        {
            Directory.CreateDirectory(destinationFolderPath);
        }

        // Создаем временную папку для извлечения файлов
        string tempFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempFolderPath);

        try
        {
            // Разархивируем файлы во временную папку
            ZipFile.ExtractToDirectory(zipFilePath, tempFolderPath);

            // Проходимся по всем файлам во временной папке и копируем их в папку назначения
            foreach (string filePath in Directory.GetFiles(tempFolderPath, "*", SearchOption.AllDirectories))
            {
                // Определяем относительный путь файла от временной папки
                string relativePath = filePath.Substring(tempFolderPath.Length + 1);
                // Формируем полный путь к файлу в папке назначения
                string destinationPath = Path.Combine(destinationFolderPath, relativePath);

                // Убедимся, что директория назначения существует, и создаем её, если нет
                string destinationDir = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                }

                // Копируем файл во временной папке в папку назначения
                System.IO.File.Copy(filePath, destinationPath, true);
                Debug.Write(destinationDir);
                Debug.Write(destinationPath);
                CreateShortcut(@"%USERPROFILE%\Desktop\GallowGame.lnk", destinationDir + @"\GallowGame.exe"); //Путь к новому ярлыку с name.lnk / путь к ИСПОЛНЯЕМОМУ файлу
            }
        }
        finally
        {
            // Удаляем временную папку и все её содержимое
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