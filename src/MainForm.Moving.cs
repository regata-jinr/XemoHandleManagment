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
using System;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Regata.Desktop.WinForms.XHM
{
    public partial class MainForm : RegataBaseForm
    {
        private TrackBar         _speedBar;
        private TableLayoutPanel _controlTable;
        private TableLayoutPanel _movingTable;
        private TableLayoutPanel _positionTable;
        private ControlsGroupBox _controlGroupBox;
        private ControlsGroupBox _speedGroupBox;
        private ControlsGroupBox _positionGroupBox;
        private Button           _rightButton;
        private Button           _leftButton;
        private Button           _upButton;
        private Button           _downButton;
        private Button           _ccwButton;
        private Button           _cwButton;
        private NumericUpDown    _XPositionNumeric;
        private NumericUpDown    _YPositionNumeric;
        private NumericUpDown    _CPositionNumeric;
        private ToolTip          _speedBarToolTip;

        private void InitializeMovingComponents()
        {
           
            // _speedBar
            _speedBar = new TrackBar();
            _speedBar.Maximum = 100;
            _speedBar.Minimum = 1;
            _speedBar.SmallChange = 5;
            _speedBar.LargeChange = 20;
            _speedBar.TickFrequency = 5;
            _speedBar.Orientation = Orientation.Vertical;
            _speedBar.Dock = DockStyle.Fill;
            _speedBar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            _speedBarToolTip = new ToolTip();
            _speedBar.Scroll += _speedBar_Scroll;

            // _movingTable
            
            _movingTable = new TableLayoutPanel();
            _movingTable.ColumnCount = 3;
            _movingTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            _movingTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            _movingTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            _movingTable.RowCount = 4;
            _movingTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            _movingTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            _movingTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            _movingTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            _movingTable.Dock = DockStyle.Fill;

            var f = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);

            _rightButton = CreateButton("🞂", act: (s, e) => _chosenSC?.MoveRight(_speedBar.Value));
            _leftButton  = CreateButton("🞀", act: (s, e) => _chosenSC?.MoveLeft(_speedBar.Value));
            _upButton    = CreateButton("▲", anc_style: AnchorStyles.Top | AnchorStyles.Bottom, act: (s, e) => _chosenSC?.MoveUp(_speedBar.Value));
            _downButton  = CreateButton("▼", anc_style: AnchorStyles.Top | AnchorStyles.Bottom, act: (s, e) => _chosenSC?.MoveDown(_speedBar.Value));
            _ccwButton   = CreateButton("⟲", act: (s, e) => _chosenSC?.MoveСounterclockwise(_speedBar.Value));
            _cwButton    = CreateButton("⟳", act: (s, e) => _chosenSC?.MoveClockwise(_speedBar.Value));

            _movingTable.Controls.Add(_upButton,    1, 0);
            _movingTable.Controls.Add(_leftButton,  0, 1);
            _movingTable.Controls.Add(_rightButton, 2, 1);
            _movingTable.Controls.Add(_downButton,  1, 2);
            _movingTable.Controls.Add(_ccwButton,   0, 3);
            _movingTable.Controls.Add(_cwButton,    2, 3);

            // _positionTable
            _positionTable = new TableLayoutPanel();
            _positionTable.ColumnCount = 2;
            _positionTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            _positionTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            _positionTable.RowCount = 3;
            _positionTable.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            _positionTable.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            _positionTable.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            _positionTable.Dock = DockStyle.Fill;

            _positionTable.Controls.Add(CreateLabel("X", ContentAlignment.MiddleLeft), 0, 0);
            _positionTable.Controls.Add(CreateLabel("Y", ContentAlignment.MiddleLeft), 0, 1);
            _positionTable.Controls.Add(CreateLabel("C", ContentAlignment.MiddleLeft), 0, 2);

            _XPositionNumeric = CreateNumericsUpDown("_XPositionNumeric", font: f);
            _YPositionNumeric = CreateNumericsUpDown("_YPositionNumeric", font: f);
            _CPositionNumeric = CreateNumericsUpDown("_CPositionNumeric", font: f);
            
            _XPositionNumeric.Maximum = 79000;
            _YPositionNumeric.Maximum = 39000;
            _CPositionNumeric.Maximum = 100000;

            _XPositionNumeric.Minimum = 0;
            _YPositionNumeric.Minimum = 0;
            _CPositionNumeric.Minimum = -100000;

            _XPositionNumeric.Value   = 77400;
            _YPositionNumeric.Value   = 37300;
            _CPositionNumeric.Value   = 10000;
            
            _positionTable.Controls.Add(_XPositionNumeric, 1, 0);
            _positionTable.Controls.Add(_YPositionNumeric, 1, 1);
            _positionTable.Controls.Add(_CPositionNumeric, 1, 2);
            
            _speedGroupBox    = new ControlsGroupBox(new Control[] { _speedBar },      vertical: false) { Name = "speedGroupBox" };
            _positionGroupBox = new ControlsGroupBox(new Control[] { _positionTable }, vertical: false) { Name = "positionGroupBox" };

            // _controlTable
            _controlTable = new TableLayoutPanel();
            _controlTable.Dock = DockStyle.Fill;
            _controlTable.ColumnCount = 3;
            _controlTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            _controlTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            _controlTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            _controlTable.Controls.Add(_speedGroupBox, 0, 0);
            _controlTable.Controls.Add(_movingTable, 1, 0);
            _controlTable.Controls.Add(_positionGroupBox, 2, 0);

            // _controlGroupBox
            _controlGroupBox = new ControlsGroupBox(new Control[] { _controlTable }, vertical: false) { Name = "controlGroupBox", Dock = DockStyle.Fill };

        }

        private void _speedBar_Scroll(object sender, System.EventArgs e)
        {
            _speedBarToolTip.SetToolTip(_speedBar, _speedBar.Value.ToString());
            if (_chosenSC == null)
                return;
            _chosenSC.Settings.CVelocity = _speedBar.Value;
        }

        private NumericUpDown CreateNumericsUpDown(string name, AnchorStyles anc_style = AnchorStyles.Left | AnchorStyles.Right, Font font = null)
        {
            var n = new NumericUpDown();
            n.Name = name;
            n.Anchor = anc_style;
            n.AutoSize = true;
            n.ReadOnly = true;
            n.Increment = 0;
            n.Controls[0].Visible = false;
            n.VerticalScroll.Visible = false;
            n.TextAlign = HorizontalAlignment.Center;

            if (font != null)
                Font = font;

            return n;
        }

        private async Task CheckPositionAsync()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                if (_chosenSC == null)
                    continue;
                if (
                        _chosenSC.CurrentPosition.X >= _XPositionNumeric.Minimum &&
                        _chosenSC.CurrentPosition.X <= _XPositionNumeric.Maximum
                    )
                    _XPositionNumeric.Value = _chosenSC.CurrentPosition.X;

                if (
                        _chosenSC.CurrentPosition.Y >= _YPositionNumeric.Minimum &&
                        _chosenSC.CurrentPosition.Y <= _YPositionNumeric.Maximum
                   )
                    _YPositionNumeric.Value = _chosenSC.CurrentPosition.Y;

                if (!_chosenSC.CurrentPosition.C.HasValue) continue;

                if (
                        _chosenSC.CurrentPosition.C.Value >= _CPositionNumeric.Minimum &&
                        _chosenSC.CurrentPosition.C.Value <= _CPositionNumeric.Maximum
                    )
                    _CPositionNumeric.Value = _chosenSC.CurrentPosition.C.Value;

                Focus();
            }
        }


    } // public partial class MainForm : RegataBaseForm
}     // namespace Regata.Desktop.WinForms.XHM

