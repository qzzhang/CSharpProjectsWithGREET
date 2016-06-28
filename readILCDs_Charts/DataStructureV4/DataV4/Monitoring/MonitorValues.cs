using System;
using System.Collections.Generic;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.DataV4.Monitoring;
using Greet.LoggerLib;

namespace Greet.DataStructureV4
{
    /// <summary>
    /// A list of "monitor" value of objects.
    /// </summary>
    [Serializable]
    public class MonitorValues : Dictionary<string, AMonitor>
    {
        public MonitorValues(GData data, XmlNode node)
        {
            foreach (XmlNode mnode in node.SelectNodes("monitor"))
            {
                try
                {
                    Monitor mnt = new Monitor(data, mnode);
                    this.Add(mnt.UniqueId, mnt);
                }
                catch (Exception e)
                {
                    LogFile.Write("WTP Monitor value failed to be created: " + e.Message);
                }
            }
            foreach (XmlNode mnode in node.SelectNodes("vmonitor"))
            {
                try
                {
                    VMonitor mnt = new VMonitor(data, mnode);
                    this.Add(mnt.UniqueId, mnt);
                }
                catch (Exception e)
                {
                    LogFile.Write("Vehicle Monitor value failed to be created: " + e.Message);
                }
            }
        }

        public MonitorValues()
        {
            // TODO: Complete member initialization
        }

        public XmlNode ToXmlNode(XmlDocument doc, bool exportingResults = false)
        {
            XmlNode mnlist = doc.CreateNode("monitored");

            foreach (Monitor mnt in this.Values)
            {
                mnlist.AppendChild(mnt.ToXmlNode(doc, exportingResults));
            }
            return mnlist;
        }
    }

}
