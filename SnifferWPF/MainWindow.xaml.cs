using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SnifferWPF;
using System.Net;
using System.Net.Sockets;

namespace SnifferWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<IPHeader> CapturedPackets { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            CapturedPackets = new ObservableCollection<IPHeader>();
            Sniffer.Start(this);
        }

        //public void AddPacketToList(string text)
        //{
        //    TextBlock newitem = new TextBlock();
        //    newitem.Text = text;
        //    newitem.Background = Brushes.Blue;
        //    newitem.Foreground = Brushes.Yellow;
        //    newitem.Padding = new Thickness(3);
        //    newitem.Margin = new Thickness(0, 0, 0, 3);

        //    PacketList.Children.Add(newitem);
        //}
    }
}
