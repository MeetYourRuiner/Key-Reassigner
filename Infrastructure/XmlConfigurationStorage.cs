using KeyReassigner.Core.Entity;
using KeyReassigner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace KeyReassigner.Infrastructure
{
    class XmlConfigurationStorage : IConfigurationStorage
    {
        private const string CONFIG_FILENAME = "config.xml";

        private const string CONFIGURATION_NODE = "configuration";
        private const string REASSIGNMENT_ARRAY_NODE = "reassignments";
        private const string REASSIGNMENT_NODE = "reassignment";
        private const string REASSIGNMENT_KEY_ATTRIBUTE = "key";
        private const string REASSIGNMENT_REASSIGNEDTO_ELEMENT = "reassignedTo";
        private const string REASSIGNMENT_IGNOREWITHCTRL_ELEMENT = "ignoreWithCtrl";

        private readonly string _appDataFolderPath;
        private readonly string _configFilePath;
        private readonly XDocument _cfg;

        public XmlConfigurationStorage()
        {
            _appDataFolderPath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
               System.Reflection.Assembly.GetEntryAssembly().GetName().Name
           );
            _configFilePath = Path.Combine(_appDataFolderPath, CONFIG_FILENAME);

            _cfg = LoadDocument();
        }

        public void Add(Reassignment reassignment)
        {
            var reassignmentsXmlArray = GetXmlArray();
            var newReassignmentElement = SerializeReassignment(reassignment);
            reassignmentsXmlArray.Add(newReassignmentElement);
            Save();
        }

        public Reassignment Get(Keys key)
        {
            var reassignmentsXmlArray = GetXmlArray();
            var reassignmentElement = FindReassignmentNode(key, reassignmentsXmlArray);
            if (reassignmentElement != null)
            {
                Reassignment reassignment = DeserializeReassignment(reassignmentElement);
                return reassignment;
            }
            return null;
        }

        public List<Reassignment> GetAll()
        {
            var reassignmentsXmlArray = GetXmlArray();
            var reassignments = reassignmentsXmlArray.Elements(REASSIGNMENT_NODE)
                .Select(xe => DeserializeReassignment(xe))
                .ToList();
            return reassignments;
        }

        public void Remove(Reassignment reassignment)
        {
            Remove(reassignment.Key);
        }

        public void Remove(Keys key)
        {
            var reassignmentsXmlArray = GetXmlArray();
            XElement reassignmentElement = FindReassignmentNode(key, reassignmentsXmlArray);
            if (reassignmentElement != null)
            {
                reassignmentElement.Remove();
                Save();
            }
        }

        public void Save()
        {
            _cfg.Save(_configFilePath);
        }

        public void Update(Reassignment original, Reassignment modified)
        {
            var reassignmentsXmlArray = GetXmlArray();
            XElement reassignmentElement = FindReassignmentNode(original.Key, reassignmentsXmlArray);
            reassignmentElement.ReplaceWith(SerializeReassignment(modified));
            Save();
        }

        public bool Exists(Reassignment reassignment)
        {
            return Exists(reassignment.Key);
        }

        public bool Exists(Keys key)
        {
            var reassignmentsXmlArray = GetXmlArray();
            XElement reassignmentElement = FindReassignmentNode(key, reassignmentsXmlArray);
            return reassignmentElement != null;
        }

        private Reassignment DeserializeReassignment(XElement xElement)
        {
            string key = xElement.Attribute(REASSIGNMENT_KEY_ATTRIBUTE).Value;
            string reassignedTo = xElement.Element(REASSIGNMENT_REASSIGNEDTO_ELEMENT).Value;
            string isIgnoredWithCtrl = xElement.Element(REASSIGNMENT_IGNOREWITHCTRL_ELEMENT).Value;
            var reassignment = new Reassignment(
                (Keys)Enum.Parse(typeof(Keys), key),
                (Keys)Enum.Parse(typeof(Keys), reassignedTo),
                bool.Parse(isIgnoredWithCtrl)
            );
            return reassignment;
        }

        private XElement SerializeReassignment(Reassignment reassignment)
        {
            XElement reassignmentElement = new XElement(
                REASSIGNMENT_NODE,
                new XAttribute(REASSIGNMENT_KEY_ATTRIBUTE, reassignment.Key),
                new XElement(REASSIGNMENT_REASSIGNEDTO_ELEMENT, reassignment.ReassignedTo),
                new XElement(REASSIGNMENT_IGNOREWITHCTRL_ELEMENT, reassignment.IsIgnoredWithCtrl)
            );
            return reassignmentElement;
        }

        private static XElement FindReassignmentNode(Keys key, XElement reassignmentArray)
        {
            return reassignmentArray.Elements(REASSIGNMENT_NODE)
                .Where(xe => xe.Attribute(REASSIGNMENT_KEY_ATTRIBUTE).Value == key.ToString())
                .FirstOrDefault();
        }

        private XElement GetXmlArray()
        {
            XElement reassignmentsXmlArray = _cfg.Element(CONFIGURATION_NODE).Element(REASSIGNMENT_ARRAY_NODE);
            return reassignmentsXmlArray;
        }

        private XDocument LoadDocument()
        {
            EnsureConfigCreated();

            XDocument cfg = XDocument.Load(_configFilePath);
            return cfg;
        }

        private void EnsureConfigCreated()
        {
            Directory.CreateDirectory(_appDataFolderPath);
            if (File.Exists(_configFilePath))
            {
                XDocument cfg = XDocument.Load(_configFilePath);
                XElement reassignmentsXmlArray = cfg.Element(CONFIGURATION_NODE).Element(REASSIGNMENT_ARRAY_NODE);
                if (reassignmentsXmlArray == null)
                {
                    CreateConfigurationFile();
                }
            }
            else
            {
                CreateConfigurationFile();
            }
        }

        private void CreateConfigurationFile()
        {
            XDocument cfg = new XDocument();
            XElement configurationRoot = new XElement(CONFIGURATION_NODE);
            XElement reassignmentsXmlArray = new XElement(REASSIGNMENT_ARRAY_NODE);
            cfg.Add(configurationRoot);
            configurationRoot.Add(reassignmentsXmlArray);
            cfg.Save(_configFilePath);
        }
    }
}
