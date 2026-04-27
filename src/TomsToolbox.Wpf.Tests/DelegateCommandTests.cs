namespace TomsToolbox.Wpf.Tests;

using System;
using System.Windows.Input;

using Xunit;

public class DelegateCommandTests
{
    /// <summary>
    /// Verifies that subscribing multiple handlers to <see cref="ICommand.CanExecuteChanged"/>
    /// only attaches a single subscription to <see cref="CommandManager.RequerySuggested"/>.
    /// Each handler must be called exactly once per raise, not once per subscription.
    /// </summary>
    [Fact]
    public void DelegateCommandT_CanExecuteChanged_RequerySuggested_AttachedOnlyOnce()
    {
        var command = new DelegateCommand<object>(_ => { });

        var handler1CallCount = 0;
        var handler2CallCount = 0;
        var handler3CallCount = 0;

        EventHandler handler1 = (_, _) => handler1CallCount++;
        EventHandler handler2 = (_, _) => handler2CallCount++;
        EventHandler handler3 = (_, _) => handler3CallCount++;

        command.CanExecuteChanged += handler1;
        command.CanExecuteChanged += handler2;
        command.CanExecuteChanged += handler3;

        command.RaiseCanExecuteChanged();

        Assert.Equal(1, handler1CallCount);
        Assert.Equal(1, handler2CallCount);
        Assert.Equal(1, handler3CallCount);
    }

    /// <summary>
    /// Verifies that <see cref="CommandManager.RequerySuggested"/> is properly detached
    /// when the last subscriber unsubscribes from <see cref="ICommand.CanExecuteChanged"/>.
    /// After detaching, <see cref="DelegateCommand{T}.RaiseCanExecuteChanged"/> must not invoke removed handlers.
    /// </summary>
    [Fact]
    public void DelegateCommandT_CanExecuteChanged_RequerySuggested_ProperlyDetached()
    {
        var command = new DelegateCommand<object>(_ => { });

        var callCount = 0;
        EventHandler handler = (_, _) => callCount++;

        command.CanExecuteChanged += handler;
        command.RaiseCanExecuteChanged();
        Assert.Equal(1, callCount);

        command.CanExecuteChanged -= handler;
        command.RaiseCanExecuteChanged();

        // Handler must not be called again after unsubscription
        Assert.Equal(1, callCount);
    }

    /// <summary>
    /// Verifies that partially unsubscribing from <see cref="ICommand.CanExecuteChanged"/> keeps
    /// <see cref="CommandManager.RequerySuggested"/> attached and only remaining handlers are notified.
    /// </summary>
    [Fact]
    public void DelegateCommandT_CanExecuteChanged_PartialUnsubscribe_RemainingHandlerStillFires()
    {
        var command = new DelegateCommand<object>(_ => { });

        var handler1CallCount = 0;
        var handler2CallCount = 0;

        EventHandler handler1 = (_, _) => handler1CallCount++;
        EventHandler handler2 = (_, _) => handler2CallCount++;

        command.CanExecuteChanged += handler1;
        command.CanExecuteChanged += handler2;

        command.CanExecuteChanged -= handler1;
        command.RaiseCanExecuteChanged();

        Assert.Equal(0, handler1CallCount);
        Assert.Equal(1, handler2CallCount);
    }

    /// <summary>
    /// Verifies that <see cref="DelegateCommand{T}.RaiseCanExecuteChanged"/> only fires handlers
    /// subscribed to that specific command instance, not handlers of other command instances.
    /// </summary>
    [Fact]
    public void DelegateCommandT_OnRequerySuggested_OnlyFiresOwnCanExecuteChangedHandlers()
    {
        var command1 = new DelegateCommand<object>(_ => { });
        var command2 = new DelegateCommand<object>(_ => { });

        var command1CallCount = 0;
        var command2CallCount = 0;
        var commandManagerCallCount = 0;

        CommandManager.RequerySuggested += (_, _) => commandManagerCallCount++;

        command1.CanExecuteChanged += (_, _) => command1CallCount++;
        command2.CanExecuteChanged += (_, _) => command2CallCount++;

        command1.RaiseCanExecuteChanged();

        Assert.Equal(1, command1CallCount);
        Assert.Equal(0, command2CallCount);
        Assert.Equal(0, commandManagerCallCount);
    }

    /// <summary>
    /// Verifies that <see cref="CommandManager.RequerySuggested"/> is attached only once
    /// for <see cref="DelegateCommand"/> (non-generic), and each handler is called exactly once per raise.
    /// </summary>
    [Fact]
    public void DelegateCommand_CanExecuteChanged_RequerySuggested_AttachedOnlyOnce()
    {
        var command = new DelegateCommand(() => { });

        var handler1CallCount = 0;
        var handler2CallCount = 0;

        EventHandler handler1 = (_, _) => handler1CallCount++;
        EventHandler handler2 = (_, _) => handler2CallCount++;

        command.CanExecuteChanged += handler1;
        command.CanExecuteChanged += handler2;

        // DelegateCommand forwards CanExecuteChanged directly to CommandManager.RequerySuggested,
        // so InvalidateRequerySuggested is the trigger. Raise via the static CommandManager.
        CommandManager.InvalidateRequerySuggested();

        // Allow the WPF dispatcher to process the queued requery
        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Background,
            new Action(() => { }));

        Assert.Equal(1, handler1CallCount);
        Assert.Equal(1, handler2CallCount);
    }

    /// <summary>
    /// Verifies that <see cref="CommandManager.RequerySuggested"/> is properly detached
    /// for <see cref="DelegateCommand"/> (non-generic) after all handlers are removed.
    /// </summary>
    [Fact]
    public void DelegateCommand_CanExecuteChanged_RequerySuggested_ProperlyDetached()
    {
        var command = new DelegateCommand(() => { });

        var callCount = 0;
        EventHandler handler = (_, _) => callCount++;

        command.CanExecuteChanged += handler;
        CommandManager.InvalidateRequerySuggested();
        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Background,
            new Action(() => { }));
        Assert.Equal(1, callCount);

        command.CanExecuteChanged -= handler;
        CommandManager.InvalidateRequerySuggested();
        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Background,
            new Action(() => { }));

        // Handler must not be called again after unsubscription
        Assert.Equal(1, callCount);
    }
}
