using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFantasy.TemplatingEngine
{
    public static class TemplateFactory
    {
        /// <summary>
        /// // <!--@ -- начало любого шаблона
        /// // <!--@@var_name-->
        /// // <!--@?code@var_name--><!--@?code--> if()
        /// // <!--@+code@var_name--><!--@+code--> if_exists(& not null)
        /// // <!--@!code@var_name--><!--@!code--> if(not)
        /// // <!--@-code@var_name--><!--@-code--> if_not_exists(|| null)
        /// // <!--@%code@var_name--><!--@%code--> for()
        /// // <!--@$code@func_name-->[<!--@|$code-->...]<!--@$code--> func()
        /// </summary>
        /// <param name="template">Template in text</param>
        /// <returns></returns>
        public static List<TemplateItem> SplitVars(string template)
        {
            List<TemplateItem> tt = new List<TemplateItem>();
            
            int si = 0;

            int nsi = template.IndexOf("<!--@", si);
            int nse = nsi >= 0 ? template.IndexOf("-->", nsi) : -1;

            while (nsi >= 0 && nse >= 0)
            {
                if (si < nsi)
                {
                    tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = template.Substring(si, nsi - si) });
                }
                if (nsi + 6 >= nse)
                {
                    tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = template.Substring(nsi, nse - nsi + 3) });
                }
                if (template.Substring(nsi + 5, 1) == "@")
                {
                    tt.Add(new TemplateItem() { tit = TemplateItemType.var, name = template.Substring(nsi + 6, nse - (nsi + 6)) });
                }
                if (template.Substring(nsi + 5, 1) == "?")
                {
                    int nsat = template.IndexOf("@", nsi + 5);
                    string code = nsat > nsi + 6 && nsat < nse ? template.Substring(nsi + 6, nsat - (nsi + 6)) : "";
                    int nsend = code.Length > 0 ? template.IndexOf("<!--@?" + code + "-->", nse) : -1;

                    if (nsat > nsi + 6 && nsat < nse && code.Length > 0 && code.Length > 0 && nsat + 1 < nse && nsend > 0)
                    {
                        List<TemplateItem> tti = SplitVars(template.Substring(nse + 3, nsend - (nse + 3)));
                        tt.Add(new TemplateItem() { tit = TemplateItemType.if_, name = template.Substring(nsat + 1, nse - (nsat + 1)), lti = tti });
                        nse = nsend + ("<!--@?" + code).Length;
                    }
                    else
                    {
                        tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = template.Substring(nsi, nse - nsi + 3) });
                    }
                }
                if (template.Substring(nsi + 5, 1) == "+")
                {
                    int nsat = template.IndexOf("@", nsi + 5);
                    string code = nsat > nsi + 6 && nsat < nse ? template.Substring(nsi + 6, nsat - (nsi + 6)) : "";
                    int nsend = code.Length > 0 ? template.IndexOf("<!--@+" + code + "-->", nse) : -1;

                    if (nsat > nsi + 6 && nsat < nse && code.Length > 0 && code.Length > 0 && nsat + 1 < nse && nsend > 0)
                    {
                        List<TemplateItem> tti = SplitVars(template.Substring(nse + 3, nsend - (nse + 3)));
                        tt.Add(new TemplateItem() { tit = TemplateItemType.if_exists_, name = template.Substring(nsat + 1, nse - (nsat + 1)), lti = tti });
                        nse = nsend + ("<!--@+" + code).Length;
                    }
                    else
                    {
                        tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = template.Substring(nsi, nse - nsi + 3) });
                    }
                }
                if (template.Substring(nsi + 5, 1) == "!")
                {
                    int nsat = template.IndexOf("@", nsi + 5);
                    string code = nsat > nsi + 6 && nsat < nse ? template.Substring(nsi + 6, nsat - (nsi + 6)) : "";
                    int nsend = code.Length > 0 ? template.IndexOf("<!--@!" + code + "-->", nse) : -1;

                    if (nsat > nsi + 6 && nsat < nse && code.Length > 0 && code.Length > 0 && nsat + 1 < nse && nsend > 0)
                    {
                        List<TemplateItem> tti = SplitVars(template.Substring(nse + 3, nsend - (nse + 3)));
                        tt.Add(new TemplateItem() { tit = TemplateItemType.not_if_, name = template.Substring(nsat + 1, nse - (nsat + 1)), lti = tti });
                        nse = nsend + ("<!--@!" + code).Length;
                    }
                    else
                    {
                        tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = template.Substring(nsi, nse - nsi + 3) });
                    }
                }
                if (template.Substring(nsi + 5, 1) == "-")
                {
                    int nsat = template.IndexOf("@", nsi + 5);
                    string code = nsat > nsi + 6 && nsat < nse ? template.Substring(nsi + 6, nsat - (nsi + 6)) : "";
                    int nsend = code.Length > 0 ? template.IndexOf("<!--@-" + code + "-->", nse) : -1;

                    if (nsat > nsi + 6 && nsat < nse && code.Length > 0 && code.Length > 0 && nsat + 1 < nse && nsend > 0)
                    {
                        List<TemplateItem> tti = SplitVars(template.Substring(nse + 3, nsend - (nse + 3)));
                        tt.Add(new TemplateItem() { tit = TemplateItemType.if_not_exists_, name = template.Substring(nsat + 1, nse - (nsat + 1)), lti = tti });
                        nse = nsend + ("<!--@-" + code).Length;
                    }
                    else
                    {
                        tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = template.Substring(nsi, nse - nsi + 3) });
                    }
                }
                if (template.Substring(nsi + 5, 1) == "%")
                {
                    int nsat = template.IndexOf("@", nsi + 5);
                    string code = nsat > nsi + 6 && nsat < nse ? template.Substring(nsi + 6, nsat - (nsi + 6)) : "";
                    int nsend = code.Length > 0 ? template.IndexOf("<!--@%" + code + "-->", nse) : -1;

                    if (nsat > nsi + 6 && nsat < nse && code.Length > 0 && code.Length > 0 && nsat + 1 < nse && nsend > 0)
                    {
                        List<TemplateItem> tti = SplitVars(template.Substring(nse + 3, nsend - (nse + 3)));
                        tt.Add(new TemplateItem() { tit = TemplateItemType.for_, name = template.Substring(nsat + 1, nse - (nsat + 1)), lti = tti });
                        nse = nsend + ("<!--@%" + code).Length;
                    }
                    else
                    {
                        tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = template.Substring(nsi, nse - nsi + 3) });
                    }
                }
                if (template.Substring(nsi + 5, 1) == "$")
                {
                    int nsat = template.IndexOf("@", nsi + 5);
                    string code = nsat > nsi + 6 && nsat < nse ? template.Substring(nsi + 6, nsat - (nsi + 6)) : "";
                    int nsend = code.Length > 0 ? template.IndexOf("<!--@$" + code + "-->", nse) : -1;

                    int pstart = nse + 3;
                    if (nsat > nsi + 6 && nsat < nse && code.Length > 0 && code.Length > 0 && nsat + 1 < nse && nsend > 0)
                    {
                        int sep_l = ("<!--@|$" + code + "-->").Length;
                        int nsep = code.Length > 0 ? template.IndexOf("<!--@|$" + code + "-->", pstart) : -1;

                        List<List<TemplateItem>> _operand = new List<List<TemplateItem>>();

                        while (nsep > 0 && nsep < nsend)
                        {
                            List<TemplateItem> tti = SplitVars(template.Substring(pstart, nsep - pstart));
                            _operand.Add(tti);

                            pstart = nsep + sep_l;
                            nsep = code.Length > 0 ? template.IndexOf("<!--@|$" + code + "-->", pstart) : -1;
                        }
                        {
                            List<TemplateItem> tti = SplitVars(template.Substring(pstart, nsend - pstart));
                            _operand.Add(tti);
                        }
                        tt.Add(new TemplateItem() { tit = TemplateItemType.func_, name = template.Substring(nsat + 1, nse - (nsat + 1)), operand = _operand });
                        nse = nsend + ("<!--@$" + code).Length;
                    }
                    else
                    {
                        tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = template.Substring(nsi, nse - nsi + 3) });
                    }
                }

                si = nse + 3;
                nsi = template.IndexOf("<!--@", si);
                nse = nsi >= 0 ? template.IndexOf("-->", nsi) : -1;
            }
            if (si < template.Length)
            {
                tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = template.Substring(si, template.Length - si) });
            }

            return tt;
        }


        public static bool RenderItemGet<T>(List<Tuple<string, Dictionary<string, object>>> values, string name, out T res)
        {
            int i = 0;
            string l_name = null;
            while (i < values.Count)
            {
                if (values[i].Item1.Length == 0)
                {
                    l_name = name;
                }
                else if (name.IndexOf(values[i].Item1) == 0)
                {
                    l_name = name.Substring(values[i].Item1.Length + 1);
                }
                if (l_name != null)
                {                    
                    string[] pars = l_name.Split(new string[] { "." }, StringSplitOptions.None);
                    if (values[i].Item2.ContainsElement(pars))
                    {
                        res = values[i].Item2.GetElement<T>(pars);
                        return true;
                    }
                }
                i++;
                l_name = null;
            }
            res = default(T);
            return false;
        }



        public static string RenderTemplate(List<TemplateItem> template, List<Tuple<string, Dictionary<string, object>>> values, object ext_obj)
        {
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < template.Count; i++)
            {
                if (template[i].tit == TemplateItemType.text)
                {
                    sb.Append(template[i].name);
                }
                if (template[i].tit == TemplateItemType.var)
                {
                    object res;
                    if (RenderItemGet(values, template[i].name, out res) && res != null)
                    {
                        sb.Append(res.ToString());
                    }
                }
                if (template[i].tit == TemplateItemType.if_)
                {
                    bool res;
                    if (RenderItemGet(values, template[i].name, out res) && res)
                    {
                        sb.Append(RenderTemplate(template[i].lti, values, ext_obj));
                    }
                }
                if (template[i].tit == TemplateItemType.if_exists_)
                {
                    object res;
                    if (RenderItemGet(values, template[i].name, out res) && res != null
                        && (!(res is List<object>) || ((List<object>)res).Any()))
                    {
                        sb.Append(RenderTemplate(template[i].lti, values, ext_obj));
                    }
                }
                if (template[i].tit == TemplateItemType.not_if_)
                {
                    bool res;
                    if (RenderItemGet(values, template[i].name, out res) && !res)
                    {
                        sb.Append(RenderTemplate(template[i].lti, values, ext_obj));
                    }
                }
                if (template[i].tit == TemplateItemType.if_not_exists_)
                {
                    object res;
                    if (!RenderItemGet(values, template[i].name, out res) || res == null
                        || ((res is List<object>) && !((List<object>)res).Any()))
                    {
                        sb.Append(RenderTemplate(template[i].lti, values, ext_obj));
                    }
                }
                if (template[i].tit == TemplateItemType.for_)
                {
                    List<object> lo;
                    if (RenderItemGet(values, template[i].name, out lo) && lo != null)
                    {
                        for (int j = 0; j < lo.Count; j++)
                        {
                            Dictionary<string, object> vls = lo[j] as Dictionary<string, object>;
                            if (vls != null)
                            {
                                List<Tuple<string, Dictionary<string, object>>> vls_l = new List<Tuple<string, Dictionary<string, object>>>();
                                vls_l.Add(new Tuple<string, Dictionary<string, object>>(template[i].name, vls));
                                vls_l.AddRange(values);

                                sb.Append(RenderTemplate(template[i].lti, vls_l, ext_obj));
                            }
                        }
                    }
                }

                if (template[i].tit == TemplateItemType.func_)
                {
                    Func<object, string[], string> func;
                    Func<object, object, string> func_object;
                    if (template[i].name == _render_template_name && _render_template_name != null && _render_template != null)
                    {
                        string[] ops = template[i].operand.Select(f => RenderTemplate(f, values, ext_obj)).ToArray();
                        sb.Append(_render_template(ops, values, ext_obj));
                    }
                    else
                    if (funcs_object.TryGetValue(template[i].name, out func_object) && template[i].operand.Count == 1 && template[i].operand[0].Count == 1 && template[i].operand[0][0].tit == TemplateItemType.var)
                    {
                        object res;
                        if (RenderItemGet(values, template[i].operand[0][0].name, out res))
                        {
                            sb.Append(func_object(ext_obj, res));
                        }
                    }
                    else
                    if (funcs.ContainsKey(template[i].name) && funcs[template[i].name].TryGetValue(template[i].operand?.Count ?? 0, out func))
                    {
                        string[] ops = template[i].operand.Select(f => RenderTemplate(f, values, ext_obj)).ToArray();
                        sb.Append(func(ext_obj, ops));
                    }
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Построить по шаблону результат
        /// </summary>
        /// <param name="template">разобранный шаблон</param>
        /// <param name="values">массив значений</param>
        /// <param name="ext_obj">внешний объект</param>
        /// <returns></returns>
        public static string RenderTemplate(List<TemplateItem> template, Dictionary<string, object> values, object ext_obj = null)
        {
            List<Tuple<string, Dictionary<string, object>>> vls_l = new List<Tuple<string, Dictionary<string, object>>>()
                    { new Tuple<string, Dictionary<string, object>>("", values) };
            return RenderTemplate(template, vls_l, ext_obj);
        }

        public static Dictionary<string, Dictionary<int, Func<object, string[], string>>> funcs = new Dictionary<string, Dictionary<int, Func<object, string[], string>>>();
        public static Dictionary<string, Func<object, object, string>> funcs_object = new Dictionary<string,Func<object, object, string>>();

        /// <summary>
        /// Добавить функцию для рендера шаблонов
        /// </summary>
        /// <param name="func_name">Имя Функции</param>
        /// <param name="params_count">Кол-во параметров</param>
        /// <param name="func">Функция (объект, переменные, результат)</param>
        public static void AddFunction(string func_name, int params_count, Func<object, string[], string> func)
        {
            funcs.AddIfNotExists(func_name, new Dictionary<int, Func<object, string[], string>>());
            funcs[func_name].AddIfNotExists(params_count, func);
        }

        /// <summary>
        /// Добавить функцию на переменную для рендера шаблонов
        /// </summary>
        /// <param name="func_name">Имя Функции</param>
        /// <param name="func">Функция (объект, переменная, результат)</param>
        public static void AddParametrFunction(string func_name, Func<object, object, string> func)
        {
            funcs_object.AddIfNotExists(func_name, func);
        }

        public static Func<string[], List<Tuple<string, Dictionary<string, object>>>, object, string> _render_template = null;
        
        public static string _render_template_name = null;
    }
}
