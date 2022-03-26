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

        private bool filtersVisibility = false;
        public bool FiltersVisibility
        {
            get { return filtersVisibility; }
            set
            {
                filtersVisibility = value;
                OnPropertyChanged("FiltersVisibility");
            }
        }

        private readonly Dictionary<TransportProtocol, bool> packetInfoVisibility = new Dictionary<TransportProtocol, bool>();

        [IndexerName ("Item")]
        public bool this[int protocol]
        {
            get { return packetInfoVisibility[(TransportProtocol)protocol]; }
            set
            {
                packetInfoVisibility[(TransportProtocol)protocol] = value;
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
                DataVisibility = currentlySelected != null && !(currentlySelected.UnderlyingPacket is IGMPHeader);
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
