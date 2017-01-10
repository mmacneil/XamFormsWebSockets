

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Websockets;
using Xamarin.Forms;

namespace XamFormsWebSockets
{
    public partial class MainPage
    {
        private bool _echo,_failed;
        private readonly IWebSocketConnection _connection;
        public MainPage()
        {
            InitializeComponent();

            // Get a websocket from your PCL library via the factory
            _connection = WebSocketFactory.Create();
            _connection.OnOpened += Connection_OnOpened;
            _connection.OnMessage += Connection_OnMessage;
            _connection.OnClosed += Connection_OnClosed;
            _connection.OnError += Connection_OnError;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _echo = _failed = false;
            _connection.Open("ws://169.254.80.80:53273/");

            while (!_connection.IsOpen && !_failed)
            {
                await Task.Delay(10);
            }

            Message.Focus();
        }

        private static void Connection_OnClosed()
        {
            Debug.WriteLine("Closed !");
        }
        private void Connection_OnError(string obj)
        {
            _failed = true;
            Debug.WriteLine("ERROR " + obj);
        }

        private void Connection_OnOpened()
        {
            Debug.WriteLine("Opened !");
        }

        private void Connection_OnMessage(string obj)
        {
            _echo = true;
            var item = new Label {Text = obj};
            Device.BeginInvokeOnMainThread(() =>
            {
                ReceivedData.Children.Add(item);
            });
        }

        private async void BtnSend_OnClicked(object sender, EventArgs e)
        {
            _echo = false;
            _connection.Send(Message.Text);
            Message.Text = "";
            
            while (!_echo && !_failed)
            {
                await Task.Delay(10);
            }

            Message.Focus();
        }
    }
}
