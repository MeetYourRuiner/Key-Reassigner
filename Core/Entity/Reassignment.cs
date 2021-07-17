using System;
using System.Windows.Forms;

namespace KeyReassigner.Core.Entity
{
    [Serializable]
    class Reassignment
    {
        public Keys Key { get; set; }
        public Keys ReassignedTo { get; set; }
        public bool IsIgnoredWithCtrl { get; set; }

        public Reassignment() { }
        public Reassignment(Keys keyToReassign, Keys keyToInvoke, bool isIgnoredWithCtrl)
        {
            Key = keyToReassign;
            ReassignedTo = keyToInvoke;
            IsIgnoredWithCtrl = isIgnoredWithCtrl;
        }
    }
}
