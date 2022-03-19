using SnifferWPF.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SnifferWPF
{
    public class PacketInfoUpdater : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Dictionary<TransportProtocol, bool> visibility = new Dictionary<TransportProtocol, bool>();

        [IndexerName ("Item")]
        public bool this[int protocol]
        {
            get { return visibility[(TransportProtocol)protocol]; }
            set
            {
                visibility[(TransportProtocol)protocol] = value;
                OnPropertyChanged("Item[]");
            }
        }

        private bool dataVisibility;
        public bool DataVisibility
        {
            get { return dataVisibility; }
            set
            {
                dataVisibility = value;
                OnPropertyChanged("DataVisibility");
            }
        }

        private IPHeader currentlySelected;
        public IPHeader CurrentlySelected
        {
            get { return currentlySelected; }
            set
            {
                currentlySelected = value;
                DataVisibility = !(currentlySelected.UnderlyingPacket is IGMPHeader);
                OnPropertyChanged("CurrentlySelected");
            }
        }

        public PacketInfoUpdater()
        {
            foreach (TransportProtocol tp in Enum.GetValues(typeof(TransportProtocol)))
            {
                this[(int)tp] = false;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
