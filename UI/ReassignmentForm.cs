using KeyReassigner.Core.Entity;
using System;
using System.Windows.Forms;

namespace KeyReassigner.UI
{
    partial class ReassignmentForm : Form
    {
        private Reassignment _editingReassignment;
        public Reassignment Result { get; set; }

        public static ReassignmentForm CreateAddForm()
        {
            return new ReassignmentForm("Add new reassignment", new Reassignment());
        }

        public static ReassignmentForm CreateEditForm(Reassignment reassignment)
        {
            return new ReassignmentForm("Edit reassignment", reassignment);
        }

        private ReassignmentForm(string title, Reassignment editingObject)
        {
            InitializeComponent();
            Text = title;
            _editingReassignment = editingObject;
        }

        private void ReassignmentForm_Load(object sender, EventArgs e)
        {
            FillComboboxes();
            SetInitialValues();
        }

        private void FillComboboxes()
        {
            var keysArray = Enum.GetValues(typeof(Keys));
            object[] keys = new object[keysArray.Length];
            keysArray.CopyTo(keys, 0);
            cbKey.Items.AddRange(keys);
            cbReassignTo.Items.AddRange(keys);
        }

        private void SetInitialValues()
        {
            cbKey.SelectedItem = _editingReassignment.Key;
            cbReassignTo.SelectedItem = _editingReassignment.ReassignedTo;
            chbIgnoreWithCtrl.Checked = _editingReassignment.IsIgnoredWithCtrl;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            Keys key = (Keys)cbKey.SelectedItem;
            Keys reassignedTo = (Keys)cbReassignTo.SelectedItem;
            bool ignoreWithCtrl = chbIgnoreWithCtrl.Checked;
            Result = new Reassignment(key, reassignedTo, ignoreWithCtrl);
            Close();
        }
    }
}
