using System;
using System.Windows.Input;

namespace VillageNewbies.Helpers
{
    // Helper class for CommandManager functionality which is not natively in .NET MAUI
    internal static class CommandManager
    {
        public static event EventHandler? RequerySuggested;

        public static void InvalidateRequerySuggested()
        {
            RequerySuggested?.Invoke(null, EventArgs.Empty);
        }
    }

    // RelayCommand on aputoiminto ViewModel:ien komentojen toteuttamiseen
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object>? _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter!);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter!);
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
} 