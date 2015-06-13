using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Business.CalculatorEngine;
using Econocom.Calculateur;


namespace Econocom.Business.CalculatorEngine
{
    public class TestGraph
    {
        
        //public void TestDynamic()
        //{
        //    var connectionString = ConfigurationManager.ConnectionStrings["EconocomContext"].ConnectionString;
        //    DbHelper dbHelper = new DbHelper();
        //    IEnumerable<dynamic> list = dbHelper.GetDynamicSqlData("select * from Usage");
        //    var first = list.First();

        //    foreach (dynamic variable in first)
        //    {
        //        Console.WriteLine(variable.Key + "--" + variable.Value);
        //    }
         
        //   // foreach(var kv in (IDictionary<string, object>)list) Console.WriteLine("Key:{0} Value:{1}", kv.Key, kv.Value);

        //    Dictionary<string, object> metrics = new Dictionary<string, object>();
        //    metrics.Add("Usage", first);
        //    dynamic usage;
        //    metrics.TryGetValue("Usage", out usage);

        //     var jour =0;
        //    if (usage != null)
        //        foreach (dynamic variable in usage)
        //        {
        //            if ("jour".Equals(variable.Key))
        //                jour = variable.Value;

        //        }


        //    var count = list.Count();

            
        //}

        //public void BuildGraph()
        //{           
        //    var web = new Graph<float?>();
        //    web.AddNode("key0", null, null, null);
        //    web.AddNode("key1", null, "key0+3", "(key0+3)");
        //    web.AddNode("key2", null, "key0+5", "(key0+5)");
        //    web.AddNode("key3", null, "key1*5", "(key1*5)");
        //    web.AddNode("key4", null, "(key1+key2)", "(key1+key2)");
        //    web.AddNode("key5", null, "(key1+key2+k3)", "(key1+key2+key3)");
        //    web.AddNode("key6", null, "(key3+key4)", "(key3+key4)");
        //    web.AddNode("total", null, "(key5+key6)", "(key5+key6)");

        //    web.AddDirectedEdge("key0", "key1", 18);
        //    web.AddDirectedEdge("key0", "key2", 19);

        //    web.AddDirectedEdge("key1", "key3", 21);
        //    web.AddDirectedEdge("key1", "key5", 22);

        //    web.AddDirectedEdge("key2", "key4", 23);
        //    web.AddDirectedEdge("key2", "key5", 24);

        //    web.AddDirectedEdge("key3", "key5", 25);
        //    web.AddDirectedEdge("key3", "key6", 26);

        //    web.AddDirectedEdge("key4", "key6", 27);

        //    web.AddDirectedEdge("key5", "total", 28);
        //    web.AddDirectedEdge("key6", "total", 29);

        //    Debug.WriteLine("web "+ web.Nodes.Count);

        //    web.Nodes.Single(c => c.NodeName=="key0").Value = 3;

        //    var value = web.Nodes.Single(c => c.NodeName == "total").Value;
        //    if (value == null) return;
        //    var total = (float) value;

        //    Debug.WriteLine("web " + total);
        //}
    }
}
