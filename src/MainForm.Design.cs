/***************************************************************************
 *                                                                         *
 *                                                                         *
 * Copyright(c) 2021, REGATA Experiment at FLNP|JINR                       *
 * Author: [Boris Rumyantsev](mailto:bdrum@jinr.ru)                        *
 *                                                                         *
 * The REGATA Experiment team license this file to you under the           *
 * GNU GENERAL PUBLIC LICENSE                                              *
 *                                                                         *
 ***************************************************************************/

using Regata.Core.UI.WinForms.Forms;
using Regata.Core.UI.WinForms.Controls;
using Regata.Core.UI.WinForms.Items;
using Regata.Core.UI.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Regata.Desktop.WinForms.XHM
{
    public partial class MainForm : RegataBaseForm
    {
        private IndicatorControl _indRef;
        private IndicatorControl _indPos;
        private IndicatorControl _indNeg;
        private IndicatorControl _indState;

        private TableLayoutPanel _mainTable;
        
        private TableLayoutPanel _controlButtonsTable;

        private ControlsGroupBox _mainGroupBox;
        private ControlsGroupBox _buttonsGroupBox;

        private Button _haltButton;
        private Button _stopButton;
        private Button _resetButton;
        private Button _homeButton;

        private Button _saveButton;
        private ComboBox _saveAsComboBox;

        private EnumItem<Devices> _devices;

        private void InitializeComponents()
        {
            InitializeMovingComponents();
            // _controlButtonsTable
            _controlButtonsTable = new TableLayoutPanel();
            _controlButtonsTable.Dock = DockStyle.Fill;
            _controlButtonsTable.ColumnCount = 3;
            _controlButtonsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            _controlButtonsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            _controlButtonsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            _controlButtonsTable.RowCount = 2;
            _controlButtonsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _controlButtonsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            _haltButton  = CreateButton("haltButton", act: (s, e) => _chosenSC?.HaltSystem());
            _stopButton  = CreateButton("stopButton", act: (s, e) => _chosenSC?.Stop());
            _resetButton = CreateButton("resetButton", act: (s, e) => _chosenSC?.Reset());
            _homeButton  = CreateButton("homeButton", act: (s, e) => _chosenSC?.Home());
            _saveButton  = CreateButton("saveButton");

            // _saveAsComboBox

            _saveAsComboBox = new ComboBox();
            _saveAsComboBox.Items.AddRange(new string[] { "AboveDetectorX", "AboveDiskExternalX", "AboveDiskInternalX", "..." });
            _saveAsComboBox.SelectedItem = null;
            _saveAsComboBox.SelectedText = "Choose binded position";
            _saveAsComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _saveAsComboBox.AutoSize = true;

            _controlButtonsTable.Controls.Add(_haltButton,0,0  );
            _controlButtonsTable.Controls.Add(_resetButton, 0,1 );
            _controlButtonsTable.Controls.Add(_homeButton,1,0);
            _controlButtonsTable.Controls.Add(_stopButton,1,1  );
            _controlButtonsTable.Controls.Add(_saveAsComboBox, 2, 0);
            _controlButtonsTable.Controls.Add(_saveButton, 2, 1);

          
            _buttonsGroupBox = new ControlsGroupBox(new Control[] { _controlButtonsTable }, vertical: false) { Name = "buttonsGroupBox", Dock = DockStyle.Fill, Margin = new Padding(0,0,0,20) };

            // _mainTable
            _mainTable = new TableLayoutPanel();
            _mainTable.Dock = DockStyle.Fill;
            _mainTable.RowCount = 2;
            _mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            _mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            _mainTable.Controls.Add(_controlGroupBox, 0,0);
            _mainTable.Controls.Add(_buttonsGroupBox, 0,1);

            _mainGroupBox = new ControlsGroupBox(new Control[] { _mainTable }, vertical: false) { Name = "mainGroupBox", Dock = DockStyle.Fill };

            // indicators
            _indRef = new IndicatorControl() { Name = "IndRefSwitch" };
            _indPos = new IndicatorControl() { Name = "IndPosSwitch" };
            _indNeg = new IndicatorControl() { Name = "IndNegSwitch" };
            _indState = new IndicatorControl() { Name = "IndState" };

            base.StatusStrip.Items.Add(new ToolStripControlHost(_indState));
            base.StatusStrip.Items.Add(new ToolStripControlHost(_indRef));
            base.StatusStrip.Items.Add(new ToolStripControlHost(_indPos));
            base.StatusStrip.Items.Add(new ToolStripControlHost(_indNeg));

            Controls.Add(_mainGroupBox);

            Labels.SetControlsLabels(this);
            _isInitialized = true;  

        }

        private Label CreateLabel(string name, ContentAlignment ca)
        {
            var l = new Label();
            l.Name = name;
            l.Dock = DockStyle.Fill;
            l.TextAlign = ca;

            return l;
        }

        private Button CreateButton(string name, AnchorStyles anc_style = AnchorStyles.Left | AnchorStyles.Right, EventHandler act = null, Font font = null)
        {
            var b = new Button();
            b.Name = name;
            b.Anchor = anc_style;
            b.AutoSize = true;
            if (act != null)
                b.Click += act;
            if (font != null)
                Font = font;

            return b;
        }

    } // public partial class MainForm : RegataBaseForm
}     // namespace Regata.Desktop.WinForms.XHM

