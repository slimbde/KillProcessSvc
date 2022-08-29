using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;


namespace KillProcessSvc.Models.Config
{
    public class ProcessNamesSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            IList<ProcessNameConfig> names = new List<ProcessNameConfig>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                var attrs = childNode.Attributes;
                if (attrs != null)
                {
                    names.Add(new ProcessNameConfig()
                    {
                        ProcessName = attrs["name"].Value,
                    });
                }
            }

            return names;
        }
    }
}
