using System;
using System.Threading;
using System.Windows;

namespace LibertyApp;

/* https://github.com/it3xl/WPF-app-Single-Instance-in-one-line-of-code/blob/master/WpfSingleInstanceByEventWaitHandle/WpfSingleInstance.cs */
public static class WpfSingleInstance
{
    private static bool _alreadyProcessedOnThisInstance;

    internal static void Make(string appName, bool uniquePerUser = true)
    {
        if (_alreadyProcessedOnThisInstance)
        {
            return;
        }
        _alreadyProcessedOnThisInstance = true;

        var app = Application.Current;

        var eventName = uniquePerUser
            ? $"{appName}-{Environment.MachineName}-{Environment.UserDomainName}-{Environment.UserName}"
            : $"{appName}-{Environment.MachineName}";

        var isSecondaryInstance = true;

        EventWaitHandle eventWaitHandle = null;
        try
        {
            eventWaitHandle = EventWaitHandle.OpenExisting(eventName);
        }
        catch
        {
            // This code only runs on the first instance.
            isSecondaryInstance = false;
        }

        if (isSecondaryInstance)
        {
            ActivateFirstInstanceWindow(eventWaitHandle);

            // Let's produce a non-interceptable exit (2009 year approach).
            Environment.Exit(0);
        }

        RegisterFirstInstanceWindowActivation(app, eventName);
    }

    private static void ActivateFirstInstanceWindow(EventWaitHandle eventWaitHandle)
    {
        // Let's notify the first instance to activate its main window.
        _ = eventWaitHandle.Set();
    }

    private static void RegisterFirstInstanceWindowActivation(Application app, string eventName)
    {
        var eventWaitHandle = new EventWaitHandle(
            false,
            EventResetMode.AutoReset,
            eventName);

        _ = ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, WaitOrTimerCallback, app, Timeout.Infinite, false);

        eventWaitHandle.Close();
    }

    private static void WaitOrTimerCallback(object state, bool timedOut)
    {
        var app = (Application)state;
        _ = app.Dispatcher.BeginInvoke(new Action(() =>
        {
            _ = Application.Current.MainWindow.Activate();
        }));
    }
}