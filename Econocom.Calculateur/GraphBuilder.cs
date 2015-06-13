using System.Collections.Generic;
using System.Linq;

namespace Econocom.Calculateur
{
    public class GraphBuilder<T> where T:struct
    {
        //private List<Metric> _deviceMetrics;
        //public Graph<T?> Graph { get; set; }        

        //public GraphBuilder(List<Metric> deviceMetrics)
        //{
        //    _deviceMetrics = deviceMetrics;
        //    Graph = new Graph<T?>();
        //}

        //public void BuildGraph()
        //{
        //    var normalisedMetrics = NormaliseMetrics();
            
        //    foreach (var normalisedMetric in normalisedMetrics)
        //    {
        //        Graph.AddNode(normalisedMetric.Code, null, normalisedMetric.Formula, normalisedMetric.Formula);
        //    }

        //    foreach (var normalisedMetric in normalisedMetrics)
        //    {
        //        if (normalisedMetric.Constituents != null && normalisedMetric.Constituents.Any())
        //        {
        //            var constituents = normalisedMetric.Constituents.Split(',').ToList();
        //            foreach (var constituent in constituents)
        //            {
        //                Graph.AddDirectedEdge(constituent, normalisedMetric.Code, normalisedMetric.Value);
        //            }
        //        }
        //    }
        //}

        //public List<Metric> NormaliseMetrics()
        //{
        //    var externalMetrics = _deviceMetrics.Where(m => m.Datasource == "ExternalMetric").ToList();
        //    ProcessExternalMetrics(externalMetrics);

        //    var objectMetrics = _deviceMetrics.Where(m => m.Datasource == "ObjectMetric").ToList();
        //    ProcessObjectMetrics(objectMetrics);

        //    var graphMetrics = _deviceMetrics.Where(m => m.AddToGraph).ToList();

        //    return graphMetrics;
        //}
        
        //public void CreateSectorMetricGraph(int sectorId)
        //{

        //}

        //private void ProcessExternalMetrics(IEnumerable<Metric> externalMetrics)
        //{
        //    foreach (var externalMetric in externalMetrics)
        //    {
        //        ProcessExternalMetricData(externalMetric);
        //    }
            
        //}

        //private void ProcessExternalMetricData(Metric metric)
        //{
        //    if (metric.Data != null)
        //    {
        //        var kvpList = ProcessMetricData(metric);

        //        if (kvpList!=null)
        //        {
        //            var dbObject = GetDBObject(kvpList);
        //            metric.DataValue = dbObject;
        //        }
        //    }          
        //}

        //private Dictionary<string, string> ProcessMetricData(Metric metric)
        //{
        //    if (metric.Data != null)
        //    {
        //        string[] metricArray = metric.Data.Split(';');
        //        var metricList = new List<string>(metricArray.Length);
        //        metricList.AddRange(metricArray);

        //        string[] kvpArray = null;
        //        var kvpList = new Dictionary<string, string>();

        //        foreach (var variable in metricList)
        //        {
        //            kvpArray = variable.Split('=');
        //            kvpList.Add(kvpArray[0], kvpArray[1]);
        //        }

        //        return kvpList;
        //    }
        //    return null;
        //}

        //private dynamic GetDBObject(IDictionary<string, string> kvp)
        //{
        //    dynamic first = null;

        //    if (kvp != null && kvp.ContainsKey("Table") && (kvp.ContainsKey("Field") || kvp.ContainsKey("Condition")))
        //    {
        //        var dbHelper = new DbHelper();
        //        var sql = "select * from " + kvp["Table"]+" ";
        //        if (kvp.ContainsKey("Condition"))
        //            sql += " where " + kvp["Condition"];
        //        var list = dbHelper.GetDynamicSqlData(sql);
        //        first = list.First();
        //    }

        //    return first;
        //}

        //private void ProcessObjectMetrics(IEnumerable<Metric> objectMetrics)
        //{
        //    foreach (var objectMetric in objectMetrics)
        //    {
        //        ProcessObjectMetricData(objectMetric);
        //    }
        //}

        //private void ProcessObjectMetricData(Metric metric)
        //{
        //    if (metric.Data != null)
        //    {
        //        var kvpList = ProcessMetricData(metric);

        //        if (kvpList != null)
        //        {
        //            var metricValue = GetMetricValue(kvpList);
        //            metric.Value = metricValue;
        //        }
        //    }     
        //}

        //private float GetMetricValue(IDictionary<string, string> kvp)
        //{
        //    foreach (var deviceMetric in _deviceMetrics)
        //    {
        //        if (kvp["Metric"].Equals(deviceMetric.Code))
        //        {
        //            dynamic metricObject = deviceMetric.DataValue;                                     
        //            if (metricObject != null){
        //                foreach (dynamic variable in metricObject)
        //                {
        //                    if (kvp["Field"].Equals(variable.Key))
        //                        return variable.Value;
        //                }
        //            }
        //        }
        //    }

        //    return 0;
        //}

        //private void ProcessGraphMetrics(List<Metric> graphMetrics)
        //{

        //}
    }
}
