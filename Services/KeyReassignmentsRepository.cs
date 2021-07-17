using KeyReassigner.Core.Entity;
using KeyReassigner.Core.Interfaces;
using KeyReassigner.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyReassigner.Services
{
    class KeyReassignmentsRepository : IKeyReassignmentsRepository
    {
        private readonly IConfigurationStorage _configuration;

        public KeyReassignmentsRepository(IConfigurationStorage configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _configuration = configuration;
#if DEBUG
            Add(new Reassignment(Keys.VolumeDown, Keys.MediaPreviousTrack, true));
            Add(new Reassignment(Keys.VolumeMute, Keys.MediaPlayPause, true));
            Add(new Reassignment(Keys.VolumeUp, Keys.MediaNextTrack, true));
#endif

        }

        public void Add(Reassignment reassignment)
        {
            if (!ReassignmentExists(reassignment.Key))
            {
                _configuration.Add(reassignment);
            }
        }

        public void Update(Reassignment original, Reassignment modified)
        {
            _configuration.Update(original, modified);
        }

        public List<Reassignment> GetAll() => _configuration.GetAll();

        public Reassignment Get(Keys key)
        {
            return _configuration.Get(key);
        }

        public void Remove(Reassignment reassignment)
        {
            _configuration.Remove(reassignment);
        }

        public void Remove(Keys key)
        {
            var reassignment = _configuration.Get(key);
            if (reassignment != null)
            {
                _configuration.Remove(reassignment);
            }
        }

        public bool ReassignmentExists(Keys key)
        {
            return _configuration.Exists(key);
        }
    }
}