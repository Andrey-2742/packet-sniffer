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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<IPHeader> CapturedPackets { get; private set; }

        private IPHeader currentlySelected;
        public IPHeader CurrentlySelected
        {
            get { return currentlySelected; }
            set
            {
                if (currentlySelected != value)
                {
                    currentlySelected = value;
                    OnPropertyChanged("CurrentlySelected");
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            CapturedPackets = new ObservableCollection<IPHeader>();
            Sniffer.Start(this);
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
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

        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Ensure row was clicked and not empty space
            var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                                                e.OriginalSource as DependencyObject) as DataGridRow;
            if (row == null) return;

            CurrentlySelected = (IPHeader)row.Item;
            MessageBox.Show(CurrentlySelected.SourceIP);
        }
    }
}
