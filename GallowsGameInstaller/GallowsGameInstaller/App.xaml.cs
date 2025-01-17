﻿namespace GallowsGameInstaller
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();   
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            const int newWidth = 800;
            const int newHeight = 480;

            window.MinimumWidth = newWidth;
            window.MaximumWidth = newWidth;
            window.MinimumHeight = newHeight;
            window.MaximumHeight = newHeight;

            return window;
        }
    }
}
