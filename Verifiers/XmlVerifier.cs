using System;
using System.Xml;

namespace Verifiers
{
    public class XmlVerifier
    {
        /// <summary>
        /// Is xml a superset of verify?
        /// 
        /// Checks:
        ///     Nodes in verify have a corresponding node in xml (order matters).
        ///
        ///     To be a corresponding node:
        ///         * Node must have the same name
        ///         * Attributes present on the verify node must be present
        ///         * child nodes must be corresponding
        ///
        /// </summary>
        /// <param name="verifyXmlFilePath">file that contains the nodes to check that are present in xml</param>
        /// <param name="xmlFilePath">xml file to check</param>
        /// <returns>True if xml is a superset of verify</returns>
        public static bool Verify(string verifyXmlFilePath, string xmlFilePath)
        {
            // iterate through nodes in the verify xml file
            // make sure each verify node has a corresponding node in the xml
            // node order matters

            var verify = new XmlDocument();
            verify.Load(verifyXmlFilePath);

            var xml = new XmlDocument();
            xml.Load(xmlFilePath);

            return ChildrenMatch(verify, xml);
        }

        private static bool ChildrenMatch(XmlNode verify, XmlNode xml)
        {
            var verifyNodes = verify.ChildNodes;
            var xmlNodes = xml.ChildNodes;

            if (null == verifyNodes || verifyNodes.Count <= 0)
            {
                // no nodes to verify
                // additional children are ok
                return true;
            }

            if (null == xmlNodes || xmlNodes.Count <= 0)
            {
                // nodes to verify, but no nodes present
                return false;
            }

            // verify child nodes (order matters)
            int currentXmlNodeIndex = 0;

            bool allNodesMatch = true;

            foreach (XmlNode verifyNode in verifyNodes)
            {
                Console.WriteLine($"+ Verify Node: [{verifyNode.Name}]");

                // check node appears in the xml
                bool match = false;
                while (!match && currentXmlNodeIndex < xmlNodes.Count)
                {
                    var currentXmlNode = xmlNodes.Item(currentXmlNodeIndex);

                    match = NodeMatches(verifyNode, currentXmlNode);

                    // look at the next node no matter what (order matters)
                    currentXmlNodeIndex++;
                }

                allNodesMatch &= match;

                Console.WriteLine($"- Verify Node: [{verifyNode.Name}] {(match ? "Match" : "FAIL")}");
            }

            // all verify child nodes had matching nodes in xml
            return allNodesMatch;

        }

        private static bool AttributesMatch(XmlNode verify, XmlNode xml)
        {
            var verifyAttributes = verify.Attributes;
            var xmlAttributes = xml.Attributes;

            if (null == verifyAttributes)
            {
                // no attributes to verify
                return true;
            }

            if (null == xmlAttributes)
            {
                // attributes to verify, but none in the xml
                return false;
            }

            // check each verify attribute is present with the appropriate value
            foreach (XmlAttribute attribute in verifyAttributes)
            {

                var name = attribute.Name;
                var value = attribute.Value;

                var xmlNode = xmlAttributes.GetNamedItem(name);

                Console.Write($"\t Attribute: [{attribute.Name}] [{value}] ");

                if (null == xmlNode)
                {
                    // attribute not present
                    Console.WriteLine("missing");
                    return false;
                }

                var xmlValue = xmlNode.Value;
                if (value != xmlValue)
                {
                    // attribute value not equivalent
                    Console.WriteLine("wrong");
                    return false;
                }

                Console.WriteLine("Match");
            }

            // all verify attributes are present with an equivalent value
            return true;
        }

        private static bool NodeMatches(XmlNode verify, XmlNode xml)
        {
            if (verify.Name != xml.Name)
            {
                // node name did not match
                return false;
            }

            if (verify.Name == "#text")
            {
                if (verify.InnerText != xml.InnerText)
                {
                    // text node did not match
                    return false;
                }
            }

            if (!AttributesMatch(verify, xml))
            {
                // attribute did not match
                return false;
            }

            if (!ChildrenMatch(verify, xml))
            {
                // a child node could not be matched
                return false;
            }

            // nodes: name, text, attributes, and children match
            return true;
        }

    }
}
