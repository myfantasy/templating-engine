using System;
using System.Collections.Generic;
using System.Text;

namespace MyFantasy.TemplatingEngine
{
    public class TemplateItem
    {
        public TemplateItemType tit;
        public string name;
        public List<TemplateItem> lti;
        public List<List<TemplateItem>> operand;

        public override string ToString()
        {
            string res = tit.ToString() + "\t" + " " + name;
            if (lti != null)
            {
                for (int i = 0; i < lti.Count; i++)
                {
                    res += "\r\n\t\t" + lti[i].ToString();
                }
            }
            return res;
        }
    }
}
