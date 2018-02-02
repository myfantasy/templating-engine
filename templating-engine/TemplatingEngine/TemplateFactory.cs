using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFantasy.TemplatingEngine
{
    public static class TemplateFactory
    {
        public static string t_var_at = "@@@";
        public static string t_mfignor = "<mf_ignor>";
        public static string t_mfignor_end = "</mf_ignor>";

        public static string t_mfvalue_attr = "mf_var";
        public static string t_mf_not_display_attr = @"mf_nd=""mf_nd""";

        public static string t_mfvalue = "<mf_value ";
        public static string t_mfvalue_end = "</mf_value>";

        #region teg
        public static string t_mfif = "<mf_if ";
        public static string t_mfif_end = "</mf_if>";
        public static string t_mfifnot = "<mf_if_not ";
        public static string t_mfifnot_end = "</mf_if_not>";
        public static string t_mfifexists = "<mf_if_exists ";
        public static string t_mfifexists_end = "</mf_if_exists>";
        public static string t_mfifnotexists = "<mf_if_not_exists ";
        public static string t_mfifnotexists_end = "</mf_if_not_exists>";
        public static string t_mffor = "<mf_for ";
        public static string t_mffor_end = "</mf_for>";

        public static string t_mffunc = "<mf_func ";
        public static string t_mffunc_end = "</mf_func>";
        public static string t_mfoperand = "<mf_operand>";
        public static string t_mfoperand_end = "</mf_operand>";
        #endregion

        #region comment
        public static string c_mfif = "<!--mf_if ";
        public static string c_mfif_end = "<!--/mf_if-->";
        public static string c_mfifnot = "<!--mf_if_not ";
        public static string c_mfifnot_end = "<!--/mf_if_not-->";
        public static string c_mfifexists = "!--<mf_if_exists ";
        public static string c_mfifexists_end = "<!--/mf_if_exists-->";
        public static string c_mfifnotexists = "<!--mf_if_not_exists ";
        public static string c_mfifnotexists_end = "<!--/mf_if_not_exists-->";
        public static string c_mffor = "<!--mf_for ";
        public static string c_mffor_end = "<!--/mf_for-->";

        public static string c_mffunc = "<!--mf_func ";
        public static string c_mffunc_end = "<!--/mf_func-->";
        public static string c_mfoperand = "<!--mf_operand-->";
        public static string c_mfoperand_end = "<!--/mf_operand-->";

        public static int ioet_l = 3;
        #endregion


        public static string[] stop_leters = new string[] { " ", ",", ";", ":", "|", "\r", "\n", "\t", "\"", "'", "<", ")", "(", ">", "-", "+", "|", "\\", "/", "*", "!", "^", "]", "[", "}", "{" };

        public static List<TemplateItem> SplitVars_v2(string template)
        {
            int stop_pos;
            return SplitVars_v2(template, 0, null, out stop_pos);
        }
        public static List<TemplateItem> SplitVars_v2(string template, int current_pos, string stop_word, out int stop_pos)
        {
            stop_pos = 0;
            List<TemplateItem> tt = new List<TemplateItem>();

            int current_pos_last_step = current_pos;

            string[] start_words = new string[] { t_var_at, t_mfignor, t_mfvalue,
                    t_mfif, t_mfifnot, t_mfifexists, t_mfifnotexists, t_mffor, t_mffunc,
                    stop_word };

            var si = template.IndexOfAnyItem(start_words, current_pos);
            bool found_stop_word = false;

            while (si.Item2 >= 0 && !found_stop_word)
            {
                if (si.Item2 - current_pos_last_step > 0)
                {
                    tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = template.Substring(current_pos_last_step, si.Item2 - current_pos_last_step) });
                }
                current_pos = si.Item2 + si.Item1.Length;

                #region stop_word
                if (si.Item1 == stop_word && stop_word != null)
                {
                    stop_pos = si.Item2 + stop_word.Length;
                    found_stop_word = true;
                }
                #endregion
                #region t_mfignor
                if (si.Item1 == t_mfignor)
                {
                    int ioe = template.IndexOf(t_mfignor_end, current_pos, StringComparison.OrdinalIgnoreCase);
                    if (ioe > 0)
                    {
                        current_pos = ioe + t_mfignor_end.Length;
                        current_pos_last_step = current_pos;
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion

                #region t_var_at @@@
                if (si.Item1 == t_var_at)
                {
                    var es = template.IndexOfAnyItem(stop_leters, current_pos);
                    if (es.Item2 > 0)
                    {
                        string var_name = template.Substring(current_pos, es.Item2 - (current_pos));

                        if (var_name != "")
                        {
                            tt.Add(new TemplateItem() { tit = TemplateItemType.var, name = var_name });
                        }

                        current_pos = es.Item2;
                        current_pos_last_step = current_pos;
                    }
                    else
                    {
                        string var_name = template.Substring(current_pos);
                        if (var_name != "")
                        {
                            tt.Add(new TemplateItem() { tit = TemplateItemType.var, name = var_name });
                        }

                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion
                #region t_mfvalue
                if (si.Item1 == t_mfvalue)
                {
                    int ioe = template.IndexOf(t_mfvalue_end, current_pos, StringComparison.OrdinalIgnoreCase);
                    int ioet = template.IndexOf(">", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);
                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                        }
                    }

                    if (iov < ioet && ioe > ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            tt.Add(new TemplateItem() { tit = TemplateItemType.var, name = var_name });
                        }

                        current_pos = ioe + t_mfvalue_end.Length;
                        current_pos_last_step = current_pos;
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion

                #region teg
                #region t_mfif
                if (si.Item1 == t_mfif)
                {
                    int ioet = template.IndexOf(">", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);

                    int iond = template.IndexOf(t_mf_not_display_attr, current_pos, StringComparison.OrdinalIgnoreCase);
                    string elements = null;

                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                            elements = template.Substring(ioq2 + 1, ioet - ioq2 - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<TemplateItem> tti = SplitVars_v2(template, ioet + 1, t_mfif_end, out stop_pos_o);

                            if (!(iond > 0 && iond < ioet))
                            {
                                tti = tti.Prepend(new TemplateItem() { tit = TemplateItemType.text, name = "<div " + elements + ">" }).ToList();
                                tti.Add(new TemplateItem() { tit = TemplateItemType.text, name = "</div>" });
                            }

                            tt.Add(new TemplateItem() { tit = TemplateItemType.if_, name = var_name, lti = tti });

                            current_pos = stop_pos_o;
                            current_pos_last_step = current_pos;
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion
                #region t_mfifnot
                if (si.Item1 == t_mfifnot)
                {
                    int ioet = template.IndexOf(">", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);

                    int iond = template.IndexOf(t_mf_not_display_attr, current_pos, StringComparison.OrdinalIgnoreCase);
                    string elements = null;

                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                            elements = template.Substring(ioq2 + 1, ioet - ioq2 - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<TemplateItem> tti = SplitVars_v2(template, ioet + 1, t_mfifnot_end, out stop_pos_o);

                            if (!(iond > 0 && iond < ioet))
                            {
                                tti = tti.Prepend(new TemplateItem() { tit = TemplateItemType.text, name = "<div " + elements + ">" }).ToList();
                                tti.Add(new TemplateItem() { tit = TemplateItemType.text, name = "</div>" });
                            }

                            tt.Add(new TemplateItem() { tit = TemplateItemType.not_if_, name = var_name, lti = tti });

                            current_pos = stop_pos_o;
                            current_pos_last_step = current_pos;
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion
                #region t_mfifexists
                if (si.Item1 == t_mfifexists)
                {
                    int ioet = template.IndexOf(">", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);

                    int iond = template.IndexOf(t_mf_not_display_attr, current_pos, StringComparison.OrdinalIgnoreCase);
                    string elements = null;

                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                            elements = template.Substring(ioq2 + 1, ioet - ioq2 - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<TemplateItem> tti = SplitVars_v2(template, ioet + 1, t_mfifexists_end, out stop_pos_o);

                            if (!(iond > 0 && iond < ioet))
                            {
                                tti = tti.Prepend(new TemplateItem() { tit = TemplateItemType.text, name = "<div " + elements + ">" }).ToList();
                                tti.Add(new TemplateItem() { tit = TemplateItemType.text, name = "</div>" });
                            }

                            tt.Add(new TemplateItem() { tit = TemplateItemType.if_exists_, name = var_name, lti = tti });

                            current_pos = stop_pos_o;
                            current_pos_last_step = current_pos;
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion
                #region t_mfifnotexists
                if (si.Item1 == t_mfifnotexists)
                {
                    int ioet = template.IndexOf(">", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);

                    int iond = template.IndexOf(t_mf_not_display_attr, current_pos, StringComparison.OrdinalIgnoreCase);
                    string elements = null;

                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                            elements = template.Substring(ioq2 + 1, ioet - ioq2 - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<TemplateItem> tti = SplitVars_v2(template, ioet + 1, t_mfifnotexists_end, out stop_pos_o);

                            if (!(iond > 0 && iond < ioet))
                            {
                                tti = tti.Prepend(new TemplateItem() { tit = TemplateItemType.text, name = "<div " + elements + ">" }).ToList();
                                tti.Add(new TemplateItem() { tit = TemplateItemType.text, name = "</div>" });
                            }

                            tt.Add(new TemplateItem() { tit = TemplateItemType.if_not_exists_, name = var_name, lti = tti });

                            current_pos = stop_pos_o;
                            current_pos_last_step = current_pos;
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion
                #region t_mffor
                if (si.Item1 == t_mffor)
                {
                    int ioet = template.IndexOf(">", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);

                    int iond = template.IndexOf(t_mf_not_display_attr, current_pos, StringComparison.OrdinalIgnoreCase);
                    string elements = null;

                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                            elements = template.Substring(ioq2 + 1, ioet - ioq2 - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<TemplateItem> tti = SplitVars_v2(template, ioet + 1, t_mffor_end, out stop_pos_o);

                            if (!(iond > 0 && iond < ioet))
                            {
                                tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = "<div " + elements + ">" });                                
                            }

                            tt.Add(new TemplateItem() { tit = TemplateItemType.for_, name = var_name, lti = tti });

                            if (!(iond > 0 && iond < ioet))
                            {
                                tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = "</div>" });                                
                            }

                            current_pos = stop_pos_o;
                            current_pos_last_step = current_pos;
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion

                #region t_mffunc
                if (si.Item1 == t_mffunc)
                {
                    int ioet = template.IndexOf(">", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);
                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<List<TemplateItem>> _operand = new List<List<TemplateItem>>();
                            stop_pos_o = ioet + 1;


                            while (template.IndexOf(t_mfoperand, stop_pos_o) > 0 &&
                                    (t_mffunc_end != template.Substring(stop_pos_o, t_mffunc_end.Length)) // в этом условии не учтены пробелы и другие игнорируемые знаки
                                  )
                            {
                                stop_pos_o = template.IndexOf(t_mfoperand, stop_pos_o) + t_mfoperand.Length;

                                List<TemplateItem> tti = SplitVars_v2(template, stop_pos_o, t_mfoperand_end, out stop_pos_o);

                                _operand.Add(tti);
                            }

                            if (
                                    template.IndexOf(t_mffunc_end, stop_pos_o) > 0
                                )
                            {


                                tt.Add(new TemplateItem() { tit = TemplateItemType.func_, name = var_name, operand = _operand });

                                current_pos = template.IndexOf(t_mffunc_end, stop_pos_o) + t_mffunc_end.Length;
                                current_pos_last_step = current_pos;
                            }
                            else
                            {
                                current_pos = template.Length;
                                current_pos_last_step = current_pos;
                            }
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion
                #endregion

                #region comment
                #region c_mfif
                if (si.Item1 == c_mfif)
                {
                    int ioet = template.IndexOf("-->", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);
                    
                    string elements = null;

                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                            elements = template.Substring(ioq2 + 1, ioet - ioq2 - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<TemplateItem> tti = SplitVars_v2(template, ioet + ioet_l, c_mfif_end, out stop_pos_o);
                            
                            tt.Add(new TemplateItem() { tit = TemplateItemType.if_, name = var_name, lti = tti });

                            current_pos = stop_pos_o;
                            current_pos_last_step = current_pos;
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion
                #region c_mfifnot
                if (si.Item1 == c_mfifnot)
                {
                    int ioet = template.IndexOf("-->", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);

                    string elements = null;

                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                            elements = template.Substring(ioq2 + 1, ioet - ioq2 - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<TemplateItem> tti = SplitVars_v2(template, ioet + ioet_l, c_mfifnot_end, out stop_pos_o);
                            
                            tt.Add(new TemplateItem() { tit = TemplateItemType.not_if_, name = var_name, lti = tti });

                            current_pos = stop_pos_o;
                            current_pos_last_step = current_pos;
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion
                #region c_mfifexists
                if (si.Item1 == c_mfifexists)
                {
                    int ioet = template.IndexOf("-->", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);

                    string elements = null;

                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                            elements = template.Substring(ioq2 + 1, ioet - ioq2 - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<TemplateItem> tti = SplitVars_v2(template, ioet + ioet_l, c_mfifexists_end, out stop_pos_o);
                            
                            tt.Add(new TemplateItem() { tit = TemplateItemType.if_exists_, name = var_name, lti = tti });

                            current_pos = stop_pos_o;
                            current_pos_last_step = current_pos;
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion
                #region c_mfifnotexists
                if (si.Item1 == c_mfifnotexists)
                {
                    int ioet = template.IndexOf("-->", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);

                    string elements = null;

                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                            elements = template.Substring(ioq2 + 1, ioet - ioq2 - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<TemplateItem> tti = SplitVars_v2(template, ioet + ioet_l, c_mfifnotexists_end, out stop_pos_o);
                            
                            tt.Add(new TemplateItem() { tit = TemplateItemType.if_not_exists_, name = var_name, lti = tti });

                            current_pos = stop_pos_o;
                            current_pos_last_step = current_pos;
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion
                #region c_mffor
                if (si.Item1 == c_mffor)
                {
                    int ioet = template.IndexOf("-->", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);

                    string elements = null;

                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                            elements = template.Substring(ioq2 + 1, ioet - ioq2 - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<TemplateItem> tti = SplitVars_v2(template, ioet + ioet_l, c_mffor_end, out stop_pos_o);
                            
                            tt.Add(new TemplateItem() { tit = TemplateItemType.for_, name = var_name, lti = tti });
                            
                            current_pos = stop_pos_o;
                            current_pos_last_step = current_pos;
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion

                #region c_mffunc
                if (si.Item1 == c_mffunc)
                {
                    int ioet = template.IndexOf("-->", current_pos, StringComparison.OrdinalIgnoreCase);
                    int iov = template.IndexOf(t_mfvalue_attr, current_pos, StringComparison.OrdinalIgnoreCase);
                    string var_name = null;
                    if (iov > 0)
                    {
                        int ioq = template.IndexOf("\"", iov, StringComparison.OrdinalIgnoreCase);
                        int ioq2 = ioq > 0 ? template.IndexOf("\"", ioq + 1, StringComparison.OrdinalIgnoreCase) : -1;
                        if (ioq2 > 0 && ioq2 < ioet)
                        {
                            var_name = template.Substring(ioq + 1, ioq2 - ioq - 1);
                        }
                    }

                    if (iov < ioet && var_name != null)
                    {
                        if (var_name != "")
                        {
                            int stop_pos_o;

                            List<List<TemplateItem>> _operand = new List<List<TemplateItem>>();
                            stop_pos_o = ioet + ioet_l;


                            while (template.IndexOf(t_mfoperand, stop_pos_o) > 0 &&
                                    (c_mffunc_end != template.Substring(stop_pos_o, c_mffunc_end.Length)) // в этом условии не учтены пробелы и другие игнорируемые знаки
                                  )
                            {
                                stop_pos_o = template.IndexOf(t_mfoperand, stop_pos_o) + t_mfoperand.Length;

                                List<TemplateItem> tti = SplitVars_v2(template, stop_pos_o, t_mfoperand_end, out stop_pos_o);

                                _operand.Add(tti);
                            }

                            if (
                                    template.IndexOf(c_mffunc_end, stop_pos_o) > 0
                                )
                            {


                                tt.Add(new TemplateItem() { tit = TemplateItemType.func_, name = var_name, operand = _operand });

                                current_pos = template.IndexOf(c_mffunc_end, stop_pos_o) + c_mffunc_end.Length;
                                current_pos_last_step = current_pos;
                            }
                            else
                            {
                                current_pos = template.Length;
                                current_pos_last_step = current_pos;
                            }
                        }
                        else
                        {
                            current_pos = template.Length;
                            current_pos_last_step = current_pos;
                        }
                    }
                    else
                    {
                        current_pos = template.Length;
                        current_pos_last_step = current_pos;
                    }
                }
                #endregion
                #endregion
                si = template.IndexOfAnyItem(start_words, current_pos);
            }

            if (!found_stop_word)
            {
                if (template.Length - current_pos_last_step > 0)
                {
                    tt.Add(new TemplateItem() { tit = TemplateItemType.text, name = template.Substring(current_pos, template.Length - current_pos_last_step) });
                }
                stop_pos = template.Length;
            }

            return tt;
        }

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
                                vls.AddOrUpdate("_num", j + 1);
                                vls.AddOrUpdate("_is_first", j == 0);
                                vls.AddOrUpdate("_is_last", j == lo.Count - 1);

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
