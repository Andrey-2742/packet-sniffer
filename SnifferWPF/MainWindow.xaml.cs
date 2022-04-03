using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace SnifferWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Filters filters = new Filters();
        public static PacketInfoUpdater PIU { get; } = new PacketInfoUpdater();
        public ObservableCollection<IPHeader> FilteredPackets { get; private set; } = new ObservableCollection<IPHeader>();
        public ObservableCollection<IPHeader> AllPackets { get; private set; } = new ObservableCollection<IPHeader>();
        public IPAddress[] Addresses { get; } = Dns.GetHostAddresses(Dns.GetHostName())
                                                .Where(x => x.AddressFamily == AddressFamily.InterNetwork).ToArray();
        public IPAddress CurrentAddress { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);

            if (Addresses.Length > 0)
            {
                CurrentAddress = Addresses[0];
                Sniffer.Init(this);
            }
            else
            {
                MessageBox.Show("Не найден ни один IP-адрес хоста.", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void AddPacket(IPHeader packet)
        {
            if (filters.MatchPacket(packet))
            {
                AllPackets.Add(packet);
                FilteredPackets.Add(packet);
            }
        }

        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement(
                (DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;

            if (row == null) return;

            PIU.CurrentlySelected = (IPHeader)row.Item;
            dashedLine.Visibility = Visibility.Visible;
            IPGrid.Visibility = Visibility.Visible;

            foreach (TransportProtocol tp in Enum.GetValues(typeof(TransportProtocol)))
            {
                PIU[(int)tp] = PIU.CurrentlySelected.Protocol == tp.ToString();
            }
            //MessageBox.Show(PIU.CurrentlySelected.SequenceNumber);
        }

        private void btnControl_Click(object sender, RoutedEventArgs e)
        {
            if (btnControl.Content.ToString() == "Начать перехват" || btnControl.Content.ToString() == "Продолжить перехват")
            {
                Sniffer.Start(CurrentAddress);
                btnControl.Content = "Остановить перехват";
            }
            else
            {
                Sniffer.Stop();
                btnControl.Content = "Продолжить перехват";
            }
        }

        private void cmbInterface_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            CurrentAddress = (IPAddress)comboBox.SelectedItem;
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.FocusedElement is TextBox textBox)
            {
                Keyboard.ClearFocus();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            AllPackets.Clear();
            FilteredPackets.Clear();
            PIU.CurrentlySelected = null;
            dashedLine.Visibility = Visibility.Collapsed;
            IPGrid.Visibility = Visibility.Collapsed;
        }

        private void btnFilters_Click(object sender, RoutedEventArgs e)
        {
            PIU.FiltersVisibility = !PIU.FiltersVisibility;
        }

        private void btnAddFilterValue_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            StackPanel stackPanel = (StackPanel)button.Parent;

            TextBox textBox = XamlReader.Parse(XamlWriter.Save(stackPanel.Children[0])) as TextBox;
            textBox.MaxLength = 5;
            textBox.Width = 43;
            textBox.Margin = new Thickness(0, 0, 3, 0);
            textBox.Visibility = Visibility.Visible;
            stackPanel.Children.Insert(stackPanel.Children.Count - 2, textBox);
        }

        private void btnRemoveFilterValue_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            StackPanel stackPanel = (StackPanel)button.Parent;
            for (int i = stackPanel.Children.Count - 3; i > 0; i--)
            {
                stackPanel.Children.RemoveAt(i);
            }
        }
        private DispatcherTimer dispatcherTimer;
        private void btnUpdateFilters_Click(object sender, RoutedEventArgs e)
        {
            filters.SourceIP = sourceIPFilter.Text;
            filters.DestinationIP = destinationIPFilter.Text;
            filters.Data = dataFilter.Text;
            filters.Protocol = new List<string>(protocolFilter.Children.OfType<TextBox>().Select(x => x.Text).ToList());
            filters.SourcePort = new List<string>(sourcePortFilter.Children.OfType<TextBox>().Select(x => x.Text).ToList());
            filters.DestinationPort = new List<string>(destinationPortFilter.Children.OfType<TextBox>().Select(x => x.Text).ToList());
            //MessageBox.Show($"{sourceIPFilter.Text}\n{filters.DestinationIP}\n{dataFilter.Text}\n{string.Join(' ', filters.Protocol)}\n" +
            //    $"{string.Join(' ', filters.SourcePort)}\n{string.Join(' ', filters.DestinationPort)}");

            FilteredPackets.Clear();
            foreach (var packet in AllPackets.Where(p => filters.MatchPacket(p)))
            {
                FilteredPackets.Add(packet);
            }

            test.Visibility = Visibility.Visible;
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            test.Visibility = Visibility.Hidden;
            dispatcherTimer.IsEnabled = false;
        }
    }
}
