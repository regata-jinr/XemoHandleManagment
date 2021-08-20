using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Regata.Core.UI.WinForms.Forms;
using Regata.Core.UI.WinForms.Controls;
using Regata.Core.UI.WinForms.Items;
using Regata.Core.UI.WinForms;
using Regata.Core.Hardware;

namespace XemoHandleManagment
{
    enum Devices { D1, D2, D3, D4 }
    public partial class MainForm : RegataBaseForm
    {
        public readonly IReadOnlyDictionary<string, string> Device_SN = new Dictionary<string, string>()
        {
            { "D1", "107374" },
            { "D2", "107375" },
            { "D3", "107376" },
            { "D4", "114005" }
        };

        SampleChanger _chosenSC;

        EnumItem<Devices> _devices;

        public MainForm() : base()
        {
            InitializeComponent();
            _devices = new EnumItem<Devices>();
            MenuStrip.Items.Add(_devices.EnumMenuItem);
            StatusStrip.Items.Add(_devices.EnumStatusLabel);

            _devices.CheckedChanged += _devices_CheckedChanged;

            Labels.SetControlsLabels(this);
        }

        private void _devices_CheckedChanged()
        {

            //SerialNumberLabel.Text = Device_SN[_devices.ToString()];

            if (_chosenSC == null)
            {
                //_chosenSC = new SampleChanger(_devices.ToString());
            }
            else
            {
                //_chosenSC.Disconnect();
                //_chosenSC = new SampleChanger(_devices.ToString());
            }

            //_devices.EnumStatusLabel.Name = $"Device:{_devices} | ComPort:{_chosenSC.ComPort} | SN:{_chosenSC.SerialNumber}";
            //_devices.EnumStatusLabel.Name = $"Device:{_devices}";
            Labels.SetControlsLabels(this);

        }
    }
}
