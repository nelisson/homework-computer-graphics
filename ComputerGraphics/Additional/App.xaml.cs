namespace Additional2
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    /// <summary>
    ///   Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void EventSetterGotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox) e.OriginalSource;
            tb.Dispatcher.BeginInvoke(new Action(tb.SelectAll), DispatcherPriority.Input);
        }
    }
}