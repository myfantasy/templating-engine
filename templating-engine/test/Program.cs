using MyFantasy.TemplatingEngine;
using System;
using System.Linq;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {


            string template = @"<!--mf_func mf_var=""courier_get_data""--><!--mf_operand-->@@@courier_id<!--/mf_operand--><!--mf_operand-->courier<!--/mf_operand--><!--/mf_func-->
<!--mf_func mf_var=""_format_date""--><!--mf_operand-->time<!--/mf_operand--><!--mf_operand-->yyyyMMddTHH:mm:ss.fffffff<!--/mf_operand--><!--mf_operand-->time_format<!--/mf_operand--><!--/mf_func-->
{
""type"":""delivery_in_courier"",
""delivery_id"":@@@delivery_id,
""title"":""Товары у курьера"",
""text"":""Ваши товары переданы курьеру @@@courier.name, тел: <mf_value mf_var=""courier.phone""></mf_value>. Ожидайте, пожалуйста, звонка от курьера."",
""courier_phone"":@@@courier.phone,
""courier_name"":""@@@courier.name"",
""dt"":""@@@time_format+02:00""
}";

            TemplateAddFunctions.Init((o, s) => template);

            string json = @"{""poo_id"":313, ""delivery_id_1"":151889986, ""courier_id"":19869, ""time"":""2018-04-28T08:39:42.866073""}";

            var ps = json.TryGetFromJson();

            var rend = TemplateManager.RenderTemplate(null, "", ps);

            Console.WriteLine(rend);
        }
    }
}
