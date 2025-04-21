using System.Diagnostics.CodeAnalysis;
using System.Windows;
using GraphicsEditor.Views;

namespace GraphicsEditor;

public class App(MainWindow mainWindow) : Application
{
    [Experimental("WPF0001")]
    protected override void OnStartup(StartupEventArgs e)
    {
        ThemeMode = ThemeMode.System;
        mainWindow.Show();
        base.OnStartup(e);
    }
}