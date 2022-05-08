using System.Collections.Generic;

namespace IFP.Modules.PiguIntegration.Models
{
    internal class xPiguProductProperty
    {
        public string IDstr { get; set; }
        public List<string> values = new List<string>();

        private string GetValuesXml() {
            string s = "";
            foreach (string val in values) {
                s += $"<value><![CDATA[{val}]]></value>\n";
            }

            return s;
        }

        public string GetXml() {
            string s = 
                $@"
                    <property>
                        <id><![CDATA[{IDstr}]]></id>
                         <values>{GetValuesXml()}</values>
                     </property>
                ";
            return s;
        }
    }
}
