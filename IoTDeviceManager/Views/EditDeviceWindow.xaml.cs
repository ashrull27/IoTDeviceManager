using System.Windows;
using System.Linq;
using IoTDeviceManager.Models;

namespace IoTDeviceManager.Views
{
    /// <summary>
    /// Interaction logic for EditDeviceWindow.xaml
    /// Dialog for editing device properties
    /// </summary>
    public partial class EditDeviceWindow : Window
    {
        public Device Device { get; private set; }

        public EditDeviceWindow(Device device = null)
        {
            InitializeComponent();

            if (device == null)
            {
                // Creating new device
                Device = new Device
                {
                    Name = "",
                    Type = "Sensor",
                    IpAddress = "192.168.1.100",
                    FirmwareVersion = "v1.0.0",
                    Units = "N/A",
                    Location = "",
                    IsOnline = false
                };
                Title = "Add New Device";
            }
            else
            {
                // Editing existing device
                Device = device;
                Title = $"Edit Device - {device.Name}";
            }

            LoadDeviceData();
        }

        private void LoadDeviceData()
        {
            txtName.Text = Device.Name;
            txtIpAddress.Text = Device.IpAddress;
            txtLocation.Text = Device.Location;
            txtFirmware.Text = Device.FirmwareVersion;
            chkIsOnline.IsChecked = Device.IsOnline;

            // Set Type ComboBox selection
            var typeItem = cmbType.Items.Cast<System.Windows.Controls.ComboBoxItem>()
                              .FirstOrDefault(i => i.Content.ToString() == Device.Type);
            if (typeItem != null)
            {
                cmbType.SelectedItem = typeItem;
            }
            else
            {
                cmbType.SelectedIndex = 0;
            }

            // Set Units ComboBox selection or text
            if (!string.IsNullOrEmpty(Device.Units))
            {
                var unitsItem = cmbUnits.Items.Cast<System.Windows.Controls.ComboBoxItem>()
                                  .FirstOrDefault(i => i.Content.ToString() == Device.Units);
                if (unitsItem != null)
                {
                    cmbUnits.SelectedItem = unitsItem;
                }
                else
                {
                    cmbUnits.Text = Device.Units;
                }
            }
            else
            {
                cmbUnits.SelectedIndex = 10; // N/A
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Device name is required.", "Validation Error",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtIpAddress.Text))
            {
                MessageBox.Show("IP Address is required.", "Validation Error",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIpAddress.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Location is required.", "Validation Error",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLocation.Focus();
                return;
            }

            // Update device properties
            Device.Name = txtName.Text.Trim();
            Device.Type = (cmbType.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "Sensor";
            Device.IpAddress = txtIpAddress.Text.Trim();
            Device.Location = txtLocation.Text.Trim();
            Device.Units = string.IsNullOrWhiteSpace(cmbUnits.Text) ? "N/A" : cmbUnits.Text.Trim();
            Device.FirmwareVersion = txtFirmware.Text.Trim();
            Device.IsOnline = chkIsOnline.IsChecked ?? false;

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}