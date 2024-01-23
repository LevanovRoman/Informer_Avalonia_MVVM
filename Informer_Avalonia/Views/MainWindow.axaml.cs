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
                    
                    StatusBar1.Text = $"Всего сообщений: {inbox.Count}\nНепрочитанных: {inbox.Recent}";
                    
                    var message1 = inbox.GetMessage (inbox.Count-1);
                    StatusBar2.Text = $"Отправитель: {message1.From}\nТема: {message1.Subject}\nДата: {message1.Date}";
                    
                    /*var message2 = inbox.GetMessage (inbox.Count-2);
                    StatusBar4.Text = $"Отправитель: {message2.From}\nТема: {message2.Subject}\nДата: {message2.Date}";
                    
                    var message3 = inbox.GetMessage (inbox.Count-3);
                    StatusBar5.Text = $"Отправитель: {message3.From}\nТема: {message3.Subject}\nДата: {message3.Date}";*/
                    
                    /*var message3 = inbox.GetMessage (inbox.Count-3);
                    StatusBar9.Text = $"Отправитель: {message3.From}";
                    StatusBar10.Text = $"Тема: {message3.Subject}";
                    StatusBar11.Text = $"Дата: {message3.Date}";*/

                    string result = "";
                    for (int i = inbox.Count-1; i > inbox.Count - 4 ; i--) {
                        var message = inbox.GetMessage (i);
                        result += $"Отправитель: {message.From}\nТема: {message.Subject}\nДата: {message.Date}\n\n";
                    }

                    StatusBar3.Text = result;
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