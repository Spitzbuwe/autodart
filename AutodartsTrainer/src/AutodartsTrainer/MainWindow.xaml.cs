using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutodartsTrainer.ViewModels;

namespace AutodartsTrainer;

public partial class MainWindow : Window
{
    private bool _isFullscreen;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        if (e.Key == Key.F11)
        {
            ToggleFullScreen();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Back)
        {
            if (Keyboard.FocusedElement is TextBox)
            {
                return;
            }

            if (viewModel.UndoCommand.CanExecute(null))
            {
                viewModel.UndoCommand.Execute(null);
                e.Handled = true;
            }

            return;
        }

        if (IsDigitKey(e.Key))
        {
            if (Keyboard.FocusedElement is not TextBox)
            {
                var digit = ExtractDigit(e.Key);
                if (digit.HasValue)
                {
                    viewModel.ManualScoreInput += digit.Value;
                    e.Handled = true;
                }
            }

            return;
        }

        if (e.Key == Key.Enter)
        {
            if (viewModel.ConfirmTurnCommand.CanExecute(null))
            {
                viewModel.ConfirmTurnCommand.Execute(null);
                e.Handled = true;
            }
        }
    }

    private void ToggleFullScreen()
    {
        if (_isFullscreen)
        {
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.SingleBorderWindow;
        }
        else
        {
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
        }

        _isFullscreen = !_isFullscreen;
    }

    private static bool IsDigitKey(Key key) =>
        key is >= Key.D0 and <= Key.D9 or >= Key.NumPad0 and <= Key.NumPad9;

    private static char? ExtractDigit(Key key) => key switch
    {
        >= Key.D0 and <= Key.D9 => (char)('0' + (key - Key.D0)),
        >= Key.NumPad0 and <= Key.NumPad9 => (char)('0' + (key - Key.NumPad0)),
        _ => null
    };
}
