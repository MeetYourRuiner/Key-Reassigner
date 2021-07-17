using KeyReassigner.Core.Entity;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyReassigner.Core.Interfaces
{
    interface IConfigurationStorage
    {
        List<Reassignment> GetAll();
        Reassignment Get(Keys key);
        void Add(Reassignment reassignment);
        void Remove(Reassignment reassignment);
        void Remove(Keys key);
        void Update(Reassignment original, Reassignment modified);
        void Save();
        bool Exists(Reassignment reassignment);
        bool Exists(Keys key);
    }
}
