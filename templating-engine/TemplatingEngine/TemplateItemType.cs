using System;
using System.Collections.Generic;
using System.Text;

namespace MyFantasy.TemplatingEngine
{
    public enum TemplateItemType
    {
        text,
        var,
        if_,
        for_,
        not_if_,
        if_exists_,
        if_not_exists_,
        func_,
    }
}
