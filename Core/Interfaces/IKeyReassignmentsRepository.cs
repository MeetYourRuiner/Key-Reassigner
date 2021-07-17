using KeyReassigner.Core.Entity;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyReassigner.Interfaces
{
    interface IKeyReassignmentsRepository
    {
        void Add(Reassignment reassignment);
        List<Reassignment> GetAll();
        Reassignment Get(Keys key);

        bool ReassignmentExists(Keys key);
        void Remove(Reassignment reassignment);
        void Remove(Keys key);
        void Update(Reassignment original, Reassignment modified);
    }
}