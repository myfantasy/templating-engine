using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFantasy.TemplatingEngine
{
    public static class TemplateManager
    {
        /// <summary>
        /// Get hash store by ext_obj. Must be not null
        /// </summary>
        public static Func<object, string> _hash = (o) => "";

        /// <summary>
        /// GetTemplateByNameFunction
        /// </summary>
        public static Func<object, string, string> LoadTemplate = (o, name) => null;

        public static Dictionary<string, Dictionary<string, Tuple<DateTime, List<TemplateItem>>>> _templates
            = new Dictionary<string, Dictionary<string, Tuple<DateTime, List<TemplateItem>>>>();


        public static string RenderTemplate(object ext_obj, string name, Dictionary<string, object> values = null,
            string def_template = null, List<Tuple<string, Dictionary<string, object>>> values_list = null)
        {
            Tuple<DateTime, List<TemplateItem>> t = _templates.GetValueOrDefault(_hash(ext_obj))?.GetValueOrDefault(name);

            string c = template_from_file_get(name);

            if (c != null)
            {
                var tt = SplitVarsDefault(c);
                if (values_list != null)
                {
                    string res = TemplateFactory.RenderTemplate(tt, values_list, ext_obj);
                    return res;
                }
                else
                {
                    if (values != null)
                    {
                        foreach (var v in _template_add_params)
                        {
                            values.AddIfNotExists(v.Key, v.Value);
                        }
                    }
                    string res = TemplateFactory.RenderTemplate(tt, values, ext_obj);
                    return res;
                }
            }

            if (t == null || (DateTime.Now - t.Item1).TotalMinutes > 3)
            {
                string templ = LoadTemplate(ext_obj, name) ?? def_template;

                if (!templ.IsNullOrWhiteSpace())
                {
                    var tt = SplitVarsDefault(templ);
                    t = new Tuple<DateTime, List<TemplateItem>>(DateTime.Now, tt);
                    lock (_templates)
                    {
                        _templates.AddIfNotExists(_hash(ext_obj), new Dictionary<string, Tuple<DateTime, List<TemplateItem>>>());
                        _templates[_hash(ext_obj)].AddOrUpdate(name, t);
                    }
                }
            }

            if (t == null)
            {
                return "";
            }
            else if (values_list != null)
            {
                string res = TemplateFactory.RenderTemplate(t.Item2, values_list, ext_obj);
                return res;
            }
            else
            {
                if (values != null)
                {
                    foreach (var v in _template_add_params)
                    {
                        values.AddIfNotExists(v.Key, v.Value);
                    }
                }
                string res = TemplateFactory.RenderTemplate(t.Item2, values, ext_obj);
                return res;
            }
        }
        


        public static string _render_template(string[] templates, List<Tuple<string, Dictionary<string, object>>> values_list, object ext_obj)
        {
            StringBuilder sb = new StringBuilder("");

            for (int i = 0; i < templates.Length; i++)
            {
                string tmpl = "";
                tmpl = RenderTemplate(ext_obj, templates[i], values_list: values_list);

                sb.Append(tmpl);
            }

            return sb.ToString();
        }

        public static Dictionary<string, string> _template_from_files = new Dictionary<string, string>();

        static string template_from_file_get(string template)
        {
            string fn;
            if (_template_from_files.TryGetValue(template, out fn))
            {
                return System.IO.File.ReadAllText(fn);
            }
            else
            {
                return null;
            }
        }

        public static Dictionary<string, object> _template_add_params = new Dictionary<string, object>();

        public static Func<string, List<TemplateItem>> SplitVarsDefault = TemplateFactory.SplitVars_v2;
    }
}
