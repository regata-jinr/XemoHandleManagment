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

using Microsoft.EntityFrameworkCore;
using Regata.Core.DataBase;
using Regata.Core.DataBase.Models;
using Regata.Core.UI.WinForms;
using Regata.Core.UI.WinForms.Controls;
using Regata.Core.UI.WinForms.Items;
using Regata.Core.UI.WinForms.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private List<Position> _savedPositions;
        private Position       _selectedPositions;

        private Button _saveButton;
        private ComboBox _saveAsComboBox;
        
        private Button _putToDiskButton;
        private NumericUpDown _diskPositionNumeric;

        private ToolTip _stateToolTip;

        private Timer _refreshCoordinatesTimer;

        private EnumItem<Devices> _devices;

        private void InitializeComponents()
        {
            InitializeMovingComponents();
            using (var r = new RegataContext())
            {
                _savedPositions = r.Positions.AsNoTracking().ToList();
            }

            // _controlButtonsTable
            _controlButtonsTable = new TableLayoutPanel();
            _controlButtonsTable.Dock = DockStyle.Fill;
            _controlButtonsTable.ColumnCount = 4;
            _controlButtonsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            _controlButtonsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            _controlButtonsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            _controlButtonsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            _controlButtonsTable.RowCount = 2;
            _controlButtonsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _controlButtonsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            _haltButton  = CreateButton("haltButton",  act:  (s, e) => _chosenSC?.HaltSystem());
            _stopButton  = CreateButton("stopButton",  act:  (s, e) => _chosenSC?.Stop());
            this.AcceptButton = _homeButton;
            this.CancelButton = _haltButton;
            _resetButton = CreateButton("resetButton", act:  (s, e) => _chosenSC?.Reset());
            _homeButton  = CreateButton("homeButton",  act:  (s, e) => _chosenSC?.Home());

            _saveButton  = CreateButton("saveButton");
            _saveButton.Click += _saveButton_Click;

            var f = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);


            _putToDiskButton = CreateButton("putToDiskButton");
            _diskPositionNumeric = CreateNumericsUpDown("_diskPositionNumeric", font: f);
            _diskPositionNumeric.Value = 1;
            _diskPositionNumeric.Minimum = 1;
            _diskPositionNumeric.Maximum = 45;
            _diskPositionNumeric.Increment = 1;
            _diskPositionNumeric.ReadOnly = false;
            _diskPositionNumeric.Controls[0].Visible = true;
            _diskPositionNumeric.VerticalScroll.Visible = true;


            // _saveAsComboBox

            _saveAsComboBox = new ComboBox();
            _saveAsComboBox.Items.AddRange(_savedPositions.Select(p => p.Name).Distinct().ToArray());
            _saveAsComboBox.SelectedItem = null;
            _saveAsComboBox.SelectedText = "Choose binded position";
            _saveAsComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _saveAsComboBox.AutoSize = true;
            _saveAsComboBox.SelectedIndexChanged += _saveAsComboBox_SelectedIndexChanged;

            _controlButtonsTable.Controls.Add(_haltButton,          0, 0);
            _controlButtonsTable.Controls.Add(_resetButton,         0, 1);
            _controlButtonsTable.Controls.Add(_homeButton,          1, 0);
            _controlButtonsTable.Controls.Add(_stopButton,          1, 1);
            _controlButtonsTable.Controls.Add(_diskPositionNumeric, 2, 0);
            _controlButtonsTable.Controls.Add(_putToDiskButton,     2, 1);
            _controlButtonsTable.Controls.Add(_saveAsComboBox,      3, 0);
            _controlButtonsTable.Controls.Add(_saveButton,          3, 1);

          
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
            _indRef = new IndicatorControl()   { Name = "IndRefSwitch" };
            _indPos = new IndicatorControl()   { Name = "IndPosSwitch" };
            _indNeg = new IndicatorControl()   { Name = "IndNegSwitch" };
            _indState = new IndicatorControl() { Name = "IndState"     };


            base.StatusStrip.Items.Add(new ToolStripControlHost(_indState));
            base.StatusStrip.Items.Add(new ToolStripControlHost(_indRef));
            base.StatusStrip.Items.Add(new ToolStripControlHost(_indPos));
            base.StatusStrip.Items.Add(new ToolStripControlHost(_indNeg));
            
            _stateToolTip = new ToolTip();

            _refreshCoordinatesTimer = new Timer();
            _refreshCoordinatesTimer.Interval = 100;
            _refreshCoordinatesTimer.Tick += _refreshCoordinatesTimer_Tick;
            _refreshCoordinatesTimer.Start();

            Controls.Add(_mainGroupBox);

            Labels.SetControlsLabels(this);
            _isInitialized = true;  

        }

        private void _refreshCoordinatesTimer_Tick(object sender, EventArgs e)
        {
            var x = _chosenSC.CurrentPosition.X;
            var y = _chosenSC.CurrentPosition.Y;
            var c = _chosenSC.CurrentPosition.C;


            if (_chosenSC == null)
                return;
            if (
                    x >= _XPositionNumeric.Minimum &&
                    x <= _XPositionNumeric.Maximum
                )
                _XPositionNumeric.Value = x;

            if (
                    y >= _YPositionNumeric.Minimum &&
                    y <= _YPositionNumeric.Maximum
               )
                _YPositionNumeric.Value = y;

            if (!_chosenSC.CurrentPosition.C.HasValue) return;

            if (
                    c.Value >= _CPositionNumeric.Minimum &&
                    c.Value <= _CPositionNumeric.Maximum
                )
                _CPositionNumeric.Value = c.Value;
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            if (_selectedPositions == null)
                return;

            _selectedPositions.X = _chosenSC.CurrentPosition.X;
            _selectedPositions.Y = _chosenSC.CurrentPosition.Y;
            _selectedPositions.C = null;

            using (var r = new RegataContext())
            {
                r.Positions.Update(_selectedPositions);
                r.SaveChanges();
            }
        }

        private void _saveAsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedPositions = _savedPositions.Where(p => p.Name == _saveAsComboBox.SelectedItem.ToString() && p.SerialNumber == _chosenSC.SerialNumber && p.Detector == _chosenSC.PairedDetector).First();
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
                b.Click += (s,e) => 
                {
                    if (_chosenSC.DeviceIsMoving)
                        _chosenSC?.Stop();
                    else
                        act(s,e);
                };
            if (font != null)
                Font = font;

            return b;
        }

    } // public partial class MainForm : RegataBaseForm
}     // namespace Regata.Desktop.WinForms.XHM

