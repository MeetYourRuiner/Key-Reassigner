using KeyReassigner.Core.Entity;
using KeyReassigner.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KeyReassigner.UI
{
    partial class ConfigWindow : Form
    {
        private const string EDIT_TEXT = "Edit";
        private const string DELETE_TEXT = "Delete";
        private const string KEY_TEXT = "Key";
        private const string REASSIGNEDTO_TEXT = "Reassigned to";
        private const string IGNOREDWITHCTRL_TEXT = "Ignored with CTRL";

        private readonly IKeyReassignmentsRepository _keyReassignmentsRepository;

        public List<Reassignment> Reassignments { get; set; }

        public ConfigWindow(IKeyReassignmentsRepository keyReassignmentsRepository)
        {
            if (keyReassignmentsRepository == null)
                throw new ArgumentNullException(nameof(keyReassignmentsRepository));

            InitializeComponent();
            _keyReassignmentsRepository = keyReassignmentsRepository;
        }

        private void ConfigWindow_Load(object sender, System.EventArgs e)
        {
            SetUpDataGridView();
            UpdateForm();
        }

        private void UpdateForm()
        {
            Reassignments = _keyReassignmentsRepository.GetAll();
            bindingSource.DataSource = Reassignments;
            dataGridView.Update();
            dataGridView.Refresh();
        }

        private void SetUpDataGridView()
        {
            dataGridView.DefaultCellStyle.SelectionBackColor = SystemColors.Window;
            dataGridView.DefaultCellStyle.SelectionForeColor = SystemColors.WindowText;
            var rowHeight = dataGridView.RowTemplate.Height;

            DataGridViewColumn[] columns = new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn()
                {
                    Name = KEY_TEXT,
                    DataPropertyName = "Key",
                    MinimumWidth = 125,
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = REASSIGNEDTO_TEXT,
                    DataPropertyName = "ReassignedTo",
                    MinimumWidth = 125,
                },
                new DataGridViewCheckBoxColumn()
                {
                    Name = IGNOREDWITHCTRL_TEXT,
                    DataPropertyName = "IsIgnoredWithCtrl",
                    MinimumWidth = 100,
                },
                new DataGridViewButtonColumn()
                {
                    Name = EDIT_TEXT,
                    HeaderText = string.Empty,
                    Text = EDIT_TEXT,
                    MinimumWidth = rowHeight,
                    Width = rowHeight,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                },
                new DataGridViewButtonColumn()
                {
                    Name = DELETE_TEXT,
                    HeaderText = string.Empty,
                    Text = DELETE_TEXT,
                    MinimumWidth = rowHeight,
                    Width = rowHeight,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                }
            };
            dataGridView.Columns.AddRange(columns);

            dataGridView.CellClick += dataGridView_CellClick;
            dataGridView.CellPainting += dataGridView_CellPainting; ;
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            var columnIndex = e.ColumnIndex;
            var editColumnIndex = dataGridView.Columns[EDIT_TEXT].Index;
            var deleteColumnIndex = dataGridView.Columns[DELETE_TEXT].Index;
            if (columnIndex == editColumnIndex)
            {
                EditRow(e.RowIndex);
            }
            else if (columnIndex == deleteColumnIndex)
            {
                DeleteRow(e.RowIndex);
            }
        }

        private void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            var columnIndex = e.ColumnIndex;
            var editColumnIndex = dataGridView.Columns[EDIT_TEXT].Index;
            var deleteColumnIndex = dataGridView.Columns[DELETE_TEXT].Index;
            if (columnIndex == editColumnIndex)
            {
                DrawImage(Properties.Resources.Edit);
            }
            else if (columnIndex == deleteColumnIndex)
            {
                DrawImage(Properties.Resources.Delete);
            }

            void DrawImage(Bitmap resource)
            {
                using (Image img = resource)
                {
                    int height = (int)(e.CellBounds.Height / 1.5);
                    int width = (int)(e.CellBounds.Width / 1.5);

                    int centerX = e.CellBounds.X + ((e.CellBounds.Width / 2) - width / 2) - 1;
                    int centerY = e.CellBounds.Y + ((e.CellBounds.Height / 2) - height / 2) - 1;

                    e.PaintContent(e.ClipBounds);
                    e.Graphics.DrawImage(img, centerX, centerY, width, height);
                    e.Handled = true;
                }
            }
        }

        private void EditRow(int index)
        {
            var key = (Keys)dataGridView.Rows[index].Cells[KEY_TEXT].Value;
            var reassignment = _keyReassignmentsRepository.Get(key);
            var dialog = ReassignmentForm.CreateEditForm(reassignment);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var modifiedReassignment = dialog.Result;
                _keyReassignmentsRepository.Update(reassignment, modifiedReassignment);
                UpdateForm();
            }
        }

        private void DeleteRow(int index)
        {
            var key = (Keys)dataGridView.Rows[index].Cells[KEY_TEXT].Value;
            var message = "Are you sure you want to delete this reassignment?";
            var title = "Delete reassignment";
            if (MessageBox.Show(message, title, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                dataGridView.Rows.RemoveAt(index);
                _keyReassignmentsRepository.Remove(key);
                UpdateForm();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var dialog = ReassignmentForm.CreateAddForm();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var newReassignment = dialog.Result;
                _keyReassignmentsRepository.Add(newReassignment);
                UpdateForm();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
