﻿/***************************************************************************
 *                                                                         *
 *                                                                         *
 * Copyright(c) 2021, REGATA Experiment at FLNP|JINR                       *
 * Author: [Boris Rumyantsev](mailto:bdrum@jinr.ru)                        *
 *                                                                         *
 * The REGATA Experiment team license this file to you under the           *
 * GNU GENERAL PUBLIC LICENSE                                              *
 *                                                                         *
 ***************************************************************************/

using Regata.Core.Hardware;
using Regata.Core.UI.WinForms;
using Regata.Core.UI.WinForms.Items;
using Regata.Core.UI.WinForms.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Regata.Desktop.WinForms.XHM
{
    enum Devices { D1, D2, D3, D4 }
    public partial class MainForm : RegataBaseForm
    {
        private static bool  _isInitialized;
       
        public readonly IReadOnlyDictionary<string, string> Device_SN = new Dictionary<string, string>()
        {
            { "D1", "107374" },
            { "D2", "107375" },
            { "D3", "107376" },
            { "D4", "114005" }
        };

        SampleChanger _chosenSC;

        public MainForm() : base()
        {
            base.StatusStrip.SizingGrip = false;
            Size = new Size(800, 700);
            base.Size = Size;
            base.MinimumSize = Size;
            MinimumSize = Size;
            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            _devices = new EnumItem<Devices>();
            base.MenuStrip.Items.Add(_devices.EnumMenuItem);
            base.StatusStrip.Items.Add(_devices.EnumStatusLabel);
            _devices.CheckedChanged += _devices_CheckedChanged;

            Name = "XemoHandleManagment";
            base.Name = "XemoHandleManagment";
            InitializeComponent();
            Labels.SetControlsLabels(this);

        }

        private void _devices_CheckedChanged()
        {

            if (!_isInitialized)
                InitializeComponents();

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