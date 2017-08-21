using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MyFantasy.TemplatingEngine
{
    public static class TemplateAddFunctions
    {
        /// <summary>
        /// Init Template example
        /// </summary>
        /// <param name="load_template">Function to load template by ext_obj and name</param>
        public static void Init(Func<object, string, string> load_template)
        {
            TemplateFactory.AddFunction("_url_encode", 1, _url_encode);
            TemplateFactory.AddFunction("_html_encode", 1, _html_encode);
            TemplateFactory.AddFunction("_replace", 1, _replace);
            TemplateFactory.AddFunction("_replace", 3, _replace);
            TemplateFactory.AddFunction("_replace", 5, _replace);
            TemplateFactory.AddFunction("_replace", 7, _replace);
            TemplateFactory.AddFunction("_replace", 9, _replace);

            TemplateFactory.AddParametrFunction("_to_json", _to_json);

            TemplateManager.LoadTemplate = load_template ?? TemplateManager.LoadTemplate;

            TemplateFactory._render_template = TemplateManager._render_template;
            TemplateFactory._render_template_name = "_render_template";
        }

        public static string _url_encode(object ext_obj, params string[] p)
        {
            if (p == null || p.Length < 1)
            { return ""; }
            return WebUtility.UrlEncode(p[0]);
        }
        public static string _html_encode(object ext_obj, params string[] p)
        {
            if (p == null || p.Length < 1)
            { return ""; }
            return WebUtility.HtmlEncode(p[0]);
        }
        public static string _replace(object ext_obj, params string[] p)
        {
            if (p != null && p.Length > 0)
            {
                string b = p[0];
                for (int i = 1; (1 + i * 2) <= p.Length; i++)
                {
                    b = b.Replace(p[i * 2 - 1], p[i * 2]);
                }
                return b;
            }
            else
            {
                return "";
            }
        }

        public static string _to_json(object ext_obj, object o)
        {
            return o.TryGetJson() ?? "";
        }
    }
}
