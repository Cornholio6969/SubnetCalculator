using System.ComponentModel;
using System.Net;
using System.Windows;

namespace SubnetCalculator
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _ipAddress;
        private string _subnetMask;
        private string _cidrNotation;
        private string _network;
        private string _broadcast;
        private string _hostMin;
        private string _hostMax;
        private string _numberOfHosts;
        private string _wildcardMask;
        private string _binaryRepresentation;

        public string IpAddress
        {
            get => _ipAddress;
            set { _ipAddress = value; OnPropertyChanged("IpAddress"); }
        }

        public string SubnetMask
        {
            get => _subnetMask;
            set { _subnetMask = value; OnPropertyChanged("SubnetMask"); }
        }

        public string CidrNotation
        {
            get => _cidrNotation;
            set { _cidrNotation = value; OnPropertyChanged("CidrNotation"); }
        }

        public string Network
        {
            get => _network;
            set { _network = value; OnPropertyChanged("Network"); }
        }

        public string Broadcast
        {
            get => _broadcast;
            set { _broadcast = value; OnPropertyChanged("Broadcast"); }
        }

        public string HostMin
        {
            get => _hostMin;
            set { _hostMin = value; OnPropertyChanged("HostMin"); }
        }

        public string HostMax
        {
            get => _hostMax;
            set { _hostMax = value; OnPropertyChanged("HostMax"); }
        }

        public string NumberOfHosts
        {
            get => _numberOfHosts;
            set { _numberOfHosts = value; OnPropertyChanged("NumberOfHosts"); }
        }

        public string WildcardMask
        {
            get => _wildcardMask;
            set { _wildcardMask = value; OnPropertyChanged("WildcardMask"); }
        }

        public string BinaryRepresentation
        {
            get => _binaryRepresentation;
            set { _binaryRepresentation = value; OnPropertyChanged("BinaryRepresentation"); }
        }

        public RelayCommand CalculateCommand { get; }

        public MainWindowViewModel()
        {
            CalculateCommand = new RelayCommand(CalculateSubnet);
        }

        private void CalculateSubnet(object parameter)
        {
            try
            {
                if (!IPAddress.TryParse(IpAddress, out IPAddress ipAddress))
                {
                    MessageBox.Show("Invalid IP Address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int cidr = 0;
                IPAddress subnetMask = null;

                if (!string.IsNullOrWhiteSpace(CidrNotation))
                {
                    if (!int.TryParse(CidrNotation, out cidr) || cidr < 0 || cidr > 32)
                    {
                        MessageBox.Show("Invalid CIDR notation.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    subnetMask = IPAddressCalculator.GetSubnetMask(cidr);
                    SubnetMask = subnetMask.ToString();
                }
                else if (!string.IsNullOrWhiteSpace(SubnetMask))
                {
                    if (!IPAddress.TryParse(SubnetMask, out subnetMask))
                    {
                        MessageBox.Show("Invalid Subnet Mask.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    cidr = IPAddressCalculator.GetCIDRFromSubnetMask(subnetMask);
                    CidrNotation = cidr.ToString();
                }
                else
                {
                    MessageBox.Show("Please enter a Subnet Mask or CIDR notation.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var networkAddress = IPAddressCalculator.GetNetworkAddress(ipAddress, subnetMask);
                var broadcastAddress = IPAddressCalculator.GetBroadcastAddress(ipAddress, subnetMask);
                var firstHost = IPAddressCalculator.GetFirstHostAddress(networkAddress);
                var lastHost = IPAddressCalculator.GetLastHostAddress(broadcastAddress);
                var totalHosts = IPAddressCalculator.GetNumberOfHosts(cidr);
                var wildcardMask = IPAddressCalculator.GetWildcardMask(subnetMask);
                var binaryRep = IPAddressCalculator.GetBinaryRepresentation(ipAddress, subnetMask);

                Network = $"{networkAddress}/{cidr}";
                Broadcast = broadcastAddress.ToString();
                HostMin = firstHost.ToString();
                HostMax = lastHost.ToString();
                NumberOfHosts = totalHosts.ToString();
                WildcardMask = wildcardMask.ToString();
                BinaryRepresentation = binaryRep;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during calculation:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
