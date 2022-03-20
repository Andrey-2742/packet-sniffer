﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SnifferWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static PacketInfoUpdater PIU { get; } = new PacketInfoUpdater();
        public ObservableCollection<IPHeader> CapturedPackets { get; } = new ObservableCollection<IPHeader>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Sniffer.Init(this);
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

        public void AddPacket(IPHeader packet)
        {
            CapturedPackets.Add(packet);
            if (CapturedPackets.Count > 300000)
            {
                CapturedPackets.RemoveAt(0);
            }
        }

        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Ensure row was clicked and not empty space
            var row = ItemsControl.ContainerFromElement(
                (DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;

            if (row == null) return;

            PIU.CurrentlySelected = (IPHeader)row.Item;

            foreach (TransportProtocol tp in Enum.GetValues(typeof(TransportProtocol)))
            {
                PIU[(int)tp] = PIU.CurrentlySelected.Protocol == tp;
            }
            //MessageBox.Show(PIU.CurrentlySelected.SequenceNumber);
        }

        private void btnControl_Click(object sender, RoutedEventArgs e)
        {
            if (btnControl.Content.ToString() == "Начать перехват" || btnControl.Content.ToString() == "Продолжить перехват")
            {
                Sniffer.Start();
                btnControl.Content = "Остановить перехват";
            }
            else
            {
                Sniffer.Stop();
                btnControl.Content = "Продолжить перехват";
            }
        }
    }
}
