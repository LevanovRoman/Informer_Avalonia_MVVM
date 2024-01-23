using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Informer_Avalonia.ViewModels;

using System;
using System.Net;
using System.Threading;

using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MailKit.Security;
using MimeKit;

namespace Informer_Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
    
    private void Clear_OnClick(object? sender, RoutedEventArgs e)
    {
        Clear();
        StatusBar1.Text = "Cleared!";
    }

    private void Clear()
    {
        InputFirstName.Clear();
        InputLastName.Clear();
    }

    private void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        var first = InputFirstName.Text;
        var last = InputLastName.Text;

        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last))
        {
            StatusBar1.Text = "Warning! One of the fields is empty";
            return;
        }
       
        using var file = new StreamWriter("output.dat", append: true);
        file.WriteLine($"{last} {first}");
        Clear();
        Mail(first, last);
        // ;
    }

    private void Mail(string name, string password)
    { 
        using (var client = new ImapClient ())
        {
                try
                {
                    client.Connect ("cgp.nordsy.spb.ru", 143, SecureSocketOptions.StartTls);
                    client.Authenticate (name, password);
                    
                    var inbox = client.Inbox;
                    inbox.Open (FolderAccess.ReadOnly);

                    Console.WriteLine ("Total messages: {0}", inbox.Count);
                    Console.WriteLine ("Recent messages: {0}", inbox.Recent);

                    for (int i = inbox.Count-1; i > inbox.Count - 4 ; i--) {
                        var message = inbox.GetMessage (i);
                        StatusBar1.Text = $"Отправитель: {message.From}";
                        StatusBar2.Text = $"Тема: {message.Subject}";
                        StatusBar3.Text = $"Дата: {message.Date}";
                    }
                    client.Disconnect (true);
                }
                catch ( MailKit.Security.AuthenticationException e)
                {
                    StatusBar1.Text = $"Incorrect: name: {name};  password: {password}";
                }
                catch ( Exception e)
                {
                    StatusBar1.Text = $"Incorrect: name: {name};  password: {password}; {e}";
                }
        }
    }
    
}