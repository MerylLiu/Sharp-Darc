namespace SharpStrc.Framework.Utilities
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlUtil
    {
        private static readonly Lazy<XmlUtil> Lazy = new Lazy<XmlUtil>(() => new XmlUtil());

        private XmlUtil()
        {
        }

        public static XmlUtil Instance
        {
            get { return Lazy.Value; }
        }

        private XmlDocument _xmldocument;
        private string _xmlFileName;

        #region Load and create xml

        public void LoadXml(string filePath)
        {
            _xmldocument = new XmlDocument();

            if (File.Exists(filePath))
                _xmldocument.Load(filePath);

            _xmlFileName = filePath;
        }

        public  bool CreateXmlDocument(string filePath, string rootNodeName,
                                             string version, string encoding,
                                             string standalone)
        {
            bool isSuccess = false;
            _xmlFileName = filePath;

            try
            {
                var xmlDoc = new XmlDocument();
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration(version,
                                                                            encoding, standalone);
                XmlNode root = xmlDoc.CreateElement(rootNodeName);
                xmlDoc.AppendChild(xmlDeclaration);
                xmlDoc.AppendChild(root);
                xmlDoc.Save(filePath);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isSuccess;
        }

        #endregion

        #region Read and write xml

        public XmlNode GetXmlNode(string xPath)
        {
            try
            {
                XmlNode xmlnode = _xmldocument.SelectSingleNode(xPath);
                return xmlnode;
            }
            catch
            {
                return null;
            }
        }

        public XmlNodeList GetXmlNodeList(string xPath)
        {
            try
            {
                XmlNodeList xmlnodelist = _xmldocument.SelectNodes(xPath);
                return xmlnodelist;
            }
            catch
            {
                return null;
            }
        }

        public XmlAttribute GetXmlAttribute(string xPath, string attributeName)
        {
            XmlAttribute xmlattribute = null;

            try
            {
                XmlNode xmlnode = _xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.Attributes.Count > 0)
                    {
                        xmlattribute = xmlnode.Attributes[attributeName];
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return xmlattribute;
        }

        public XmlAttributeCollection GetNodeAttributes(string xPath)
        {
            XmlAttributeCollection xmlattributes = null;

            try
            {
                XmlNode xmlnode = _xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.Attributes.Count > 0)
                    {
                        xmlattributes = xmlnode.Attributes;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return xmlattributes;
        }

        public bool UpdateAttribute(string xPath, string attributeName, string value)
        {
            bool isSuccess = false;

            try
            {
                XmlNode xmlnode = _xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    foreach (XmlAttribute attribute in xmlnode.Attributes)
                    {
                        if (attribute.Name.ToLower() == attributeName.ToLower())
                        {
                            isSuccess = true;
                            attribute.Value = value;
                            _xmldocument.Save(_xmlFileName);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccess;
        }

        public bool DeleteAttributes(string xPath)
        {
            bool isSuccess = false;

            try
            {
                XmlNode xmlnode = _xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.Attributes.Count > 0)
                    {
                        xmlnode.Attributes.RemoveAll();
                        _xmldocument.Save(_xmlFileName);
                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccess;
        }

        public bool DeleteOneAttribute(string xPath, string attributeName)
        {
            bool isSuccess = false;
            XmlAttribute xmlAttribute = null;

            try
            {
                XmlNode xmlnode = _xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.Attributes.Count > 0)
                    {
                        foreach (XmlAttribute attribute in xmlnode.Attributes)
                        {
                            if (attribute.Name.ToLower() == attributeName.ToLower())
                            {
                                xmlAttribute = attribute;
                                break;
                            }
                        }
                    }
                    if (xmlAttribute != null)
                    {
                        xmlnode.Attributes.Remove(xmlAttribute);
                        _xmldocument.Save(_xmlFileName);
                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccess;
        }

        public bool AddAttribute(string xPath, string attributeName, string value)
        {
            bool isSuccess = false;

            try
            {
                XmlNode xmlnode = _xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.Attributes.Count > 0)
                    {
                        foreach (XmlAttribute attribute in xmlnode.Attributes)
                        {
                            if (attribute.Name.ToLower() == attributeName.ToLower())
                            {
                                return true;
                            }
                        }
                    }
                    XmlAttribute xmlAttribute = _xmldocument.CreateAttribute(attributeName);
                    xmlAttribute.Value = value;
                    xmlnode.Attributes.Append(xmlAttribute);
                    _xmldocument.Save(_xmlFileName);
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccess;
        }

        public bool AddNode(string xPath, string nodeName, string innerText)
        {
            bool isSuccess = false;
            bool isExisitNode = false;

            try
            {
                XmlNode xmlnode = _xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    isExisitNode = true;
                }
                if (!isExisitNode)
                {
                    XmlElement subElement = _xmldocument.CreateElement(nodeName);
                    subElement.InnerText = innerText;
                    xmlnode.AppendChild(subElement);
                    isSuccess = true;
                    _xmldocument.Save(_xmlFileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccess;
        }

        public bool UpdateNode(string xPath, string nodeName, string innerText)
        {
            bool isSuccess = false;
            bool isExisitNode = false;
            XmlNode xmlnode = _xmldocument.SelectSingleNode(xPath);

            try
            {
                if (xmlnode != null)
                {
                    isExisitNode = true;
                }
                if (!isExisitNode)
                {
                    xmlnode.InnerText = innerText;
                    isSuccess = true;
                    _xmldocument.Save(_xmlFileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccess;
        }

        public bool DeleteNode(string xPath, string nodeName)
        {
            bool isSuccess = false;

            try
            {
                XmlNode xmlnode = _xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    if (xmlnode.HasChildNodes)
                    {
                        isSuccess = false;
                    }
                    else
                    {
                        xmlnode.ParentNode.RemoveChild(xmlnode);
                        isSuccess = true;
                        _xmldocument.Save(_xmlFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isSuccess;
        }

        public bool UpdateChildNode(string xPath, string nodeName,
            string childName, string innerText)
        {
            bool isSuccess = false;
            try
            {
                XmlNode xmlnode = _xmldocument.SelectSingleNode(xPath);
                if (xmlnode != null)
                {
                    foreach (XmlNode node in xmlnode.ChildNodes)
                    {
                        if (node.Name.ToLower() == childName.ToLower())
                        {
                            node.InnerText = innerText;
                            _xmldocument.Save(_xmlFileName);
                            isSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccess;
        }

        #endregion

        #region Serialize and Deserialize

        public static void Serialize<T>(string filename, T obj)
        {
            var xs = new XmlSerializer(typeof(T));
            using (var wr = new StreamWriter(filename))
            {
                xs.Serialize(wr, obj);
            }
        }

        public static T Deserialize<T>(string filename)
        {
            var xs = new XmlSerializer(typeof(T));
            using (var rd = new StreamReader(filename))
            {
                return (T)xs.Deserialize(rd);
            }
        }

        #endregion
    }
}