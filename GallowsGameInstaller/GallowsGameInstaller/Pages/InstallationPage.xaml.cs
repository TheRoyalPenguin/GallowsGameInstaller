using System.Diagnostics;
using System.IO.Compression;
using CommunityToolkit.Maui.Storage;
using IWshRuntimeLibrary;

namespace GallowsGameInstaller.Pages;

public partial class InstallationPage : ContentPage
{
    Label labelProgressBarProcent, labelProgressCompleteText;
    ProgressBar progressBar;
    private string shortcutPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\GallowsGame.lnk");    
    private string desktopPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop");
    public InstallationPage(string path)
    {
        InitializeComponent();
        ProgressBarInit();
        if (!string.IsNullOrEmpty(path))
        {
            //ExtractAndCopyFiles(@"D:\CSProjects\GallowsGameInstaller\GallowsGameInstaller\ProjectBuildFiles\GallowGame.zip", path);            
            ExtractAndCopyFiles(desktopPath + @"\GallowsGame.zip", path);
        }
    }
    public InstallationPage()
    {
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false); //убирает дефолтную кнопку возвращения на предыдущую страницу
        ProgressBarInit();
        ExtractAndCopyFiles(desktopPath + @"\GallowsGame.zip", desktopPath + @"\GallowsGame");
    }
    private void ProgressBarInit()
    {
        // Создайте ProgressBar
        progressBar = new ProgressBar
        {
            Progress = 0.01, // начальное значение прогресса (0.5 для 50%)
            WidthRequest = 608,
            ProgressColor = Colors.Navy
        };
        labelProgressBarProcent = new Label();
        labelProgressBarProcent.Text = "1%";
        labelProgressBarProcent.TextColor = Colors.Black;
        labelProgressBarProcent.Margin = new Thickness(0, -3, 0, 0);

        labelProgressCompleteText = new Label();
        labelProgressCompleteText.Text = "Установка...";
        labelProgressCompleteText.TextColor = Colors.Green;

        Grid progressGrid = new Grid();
        progressGrid.HeightRequest = 100;
        progressGrid.Add(labelProgressBarProcent, 0, 1);
        progressGrid.Add(progressBar, 0, 1);  
        progressGrid.Add(labelProgressCompleteText, 0, 2);
        
        Grid exitGrid = new Grid();
        Image exitImage = new Image();
        exitImage.Source = "thirdbttn.png";
        exitImage.WidthRequest = 130;

        Button exitButton = new Button();
        exitButton.Background = Colors.Transparent;
        exitButton.Text = "Выйти";
        exitButton.TextColor = Colors.Navy;
        exitButton.FontFamily = "Maki-Sans";
        exitButton.FontSize = 20;
        exitButton.WidthRequest = 130;
        exitButton.CornerRadius = 500;
        exitButton.HeightRequest = 60;
        exitButton.Clicked += OnExitButtonClicked;

        exitGrid.Add(exitImage);
        exitGrid.Add(exitButton);

        Grid startGrid = new Grid();
        Image startImage = new Image();
        startImage.Source = "firstbttn.png";
        startImage.WidthRequest = 200;

        Button startButton = new Button();
        startButton.Background = Colors.Transparent;
        startButton.Text = "Запустить";
        startButton.FontSize = 20;
        startButton.TextColor = Colors.Navy;
        startButton.FontFamily = "Maki-Sans";
        startButton.WidthRequest = 200;
        startButton.CornerRadius = 500;
        startButton.HeightRequest = 60;
        startButton.Clicked += OnStartButtonClicked;

        startGrid.Add(startImage);
        startGrid.Add(startButton);

        var buttonsGrid = new Grid();
        buttonsGrid.Margin = new Thickness(-70,0,0,0);
        buttonsGrid.Padding = new Thickness(200, 0, 100, 0);
        buttonsGrid.Add(startGrid, 0, 0);
        buttonsGrid.Add(exitGrid, 1, 0);

        // Grid для размещения ProgressBar по центру
        var grid = new Grid();
        grid.WidthRequest = 600;
        grid.HeightRequest = 400;
        grid.Margin = new Thickness(0, 10, 73, 0);
        grid.Add(progressGrid, 0, 0);
        grid.Add(buttonsGrid, 0, 1);

        // Выравнивание по центру
        grid.HorizontalOptions = LayoutOptions.Center;
        grid.VerticalOptions = LayoutOptions.Center;

        Content = grid;
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
            var allFilesTempFolder = Directory.GetFiles(tempFolderPath, "*", SearchOption.AllDirectories);
            int tempFolderFilesCount = allFilesTempFolder.Count();
            int counter = 0;
            foreach (string filePath in allFilesTempFolder)
            {
                counter += 1;
                double shareReadiness = (double)counter / tempFolderFilesCount;
                double shareReadinessRounded = Math.Round(shareReadiness, 2);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    progressBar.Progress = shareReadinessRounded;
                    labelProgressBarProcent.Text = (shareReadiness * 100).ToString() + "%";
                });

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
                CreateShortcut(shortcutPath, destinationDir + @"\GallowsGame.exe"); //Путь к новому ярлыку с name.lnk / путь к ИСПОЛНЯЕМОМУ файлу
                labelProgressCompleteText.Text = "Установка завершена!";
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

    private void OnExitButtonClicked(object sender, EventArgs e)
    {
        System.Environment.Exit(0);
    }
    private void OnStartButtonClicked(object sender, EventArgs e)
    {
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = shortcutPath;
            process.StartInfo.UseShellExecute = true; // Важно для ярлыков
            process.Start();

            // Ожидаем завершения процесса
            process.WaitForExit();
            System.Environment.Exit(0);
        }
        catch(Exception ex)
        {

        }
    }
}