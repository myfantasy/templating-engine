﻿using System;
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
            TemplateFactory.AddFunction("_json_str_encode", 1, _json_str_encode);
            TemplateFactory.AddFunction("_replace", 1, _replace);
            TemplateFactory.AddFunction("_replace", 3, _replace);
            TemplateFactory.AddFunction("_replace", 5, _replace);
            TemplateFactory.AddFunction("_replace", 7, _replace);
            TemplateFactory.AddFunction("_replace", 9, _replace);

            TemplateFactory.AddParametrFunction("_to_json", _to_json);


            TemplateFactory.AddDataGetFunction("_get_current_date", _get_current_date);
            TemplateFactory.AddFormatDataGetFunction("_format_date", _format_date);
            TemplateFactory.AddDataGetFunction("_set", _set);


            TemplateManager.LoadTemplate = load_template ?? TemplateManager.LoadTemplate;

            TemplateFactory._render_template = TemplateManager._render_template;
            TemplateFactory._render_template_name = "_render_template";
            TemplateFactory._show_all_params_name = "_show_all_params_name";
        }

        public static string _format_date(object ext_obj, object val, params string[] p)
        {
            if (!(val is DateTime))
            {
                return null;
            }
            DateTime dt = (DateTime)val;
            if (p.Length == 0 || p[0].IsNullOrWhiteSpace())
            {
                return dt.ToString("yyyy-MM-dd HH:mm");
            }
            else
            {
                return dt.ToString(p[0]);
            }
        }

        public static string _set(object ext_obj, params string[] p)
        {
            if (p == null || p.Length == 0 || p[0].IsNullOrWhiteSpace())
            {
                return "";
            }

            return p[0];
        }

        public static string _json_str_encode(object ext_obj, params string[] p)
        {
            if (p == null || p.Length < 1)
            { return ""; }
            return p[0].Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"");
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

        public static object _get_current_date(object ext_obj, params string[] p)
        {
            return DateTime.Now;
        }
    }
}
