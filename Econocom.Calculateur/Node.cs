using System.Collections.Generic;
using System.Linq;
using Business.CalculatorEngine;

namespace Econocom.Business.CalculatorEngine
{
    public class Node<T> 
    {   
        private Dictionary<string, float> nodeValues = new Dictionary<string, float>();       

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) IsModified = false;
            else IsModified = true;
            field = value;           
            return true;
        }

        private void RecalculateNeighbourValues(NodeList<T> neighbours)
        {           
            if (neighbours != null && neighbours.Count > 0)
            {
                var neighboursNodes = new NodeList<T>();
                foreach (var node in neighbours)
                {
                    var tempNode = node;
                    if (tempNode.Neighbors != null)
                    {
                        foreach (var newNeighbours in tempNode.Neighbors)
                        {
                            neighboursNodes.Add(newNeighbours);
                        }
                    }

                    SubstituteNodeValueInFormulaExpression(tempNode);
                }
                RecalculateNeighbourValues(neighboursNodes);
            }

        }

        private void RecalculateNeighbourValues()
        {
            var neighbours = this.Neighbors;
            var x = this.Constituents;
            if (neighbours != null && neighbours.Count > 0)
            {
                // we need to calculate the first neightbours first
                // then we proceed with the neighbour's neighbours in hierarchy fashion
                foreach (var node in neighbours)
                {
                    var tempNode = node;
                    SubstituteNodeValueInFormulaExpression(tempNode);
                }

                foreach (var neighbourNode in neighbours)
                {
                    var tempNeighbourNode = neighbourNode;
                    RecalculateNodeValue(tempNeighbourNode);
                }
            }
        }

        private void RecalculateNodeValue(Node<T> node)
        {
            SubstituteNodeValueInFormulaExpression(node);
            if (node.Neighbors != null && node.Neighbors.Count > 0)
            {
                foreach (var childNode in node.Neighbors)
                {
                    RecalculateNodeValue(childNode);
                }
            }                    
        }


        private void SubstituteNodeValueInFormulaExpression(Node<T> node)
        {
            //string subsitutedExpression = node.Constituents.Aggregate(node.FormulaExpression, (current, constituent) => current.Replace(constituent.NodeName, constituent.Value.ToString()));
            string subsitutedExpression = node.FormulaExpression;
            foreach (var key in nodeValues.Keys.Where(key => subsitutedExpression.Contains(key)))
            {
                subsitutedExpression = subsitutedExpression.Replace(key, nodeValues[key].ToString());
            }
            subsitutedExpression = node.Constituents.Where(constituentNode => constituentNode.Value != null).Aggregate(subsitutedExpression, (current, constituentNode) => current.Replace(constituentNode.NodeName, (dynamic)constituentNode.Value.ToString()));
            var c = new MSScriptControl.ScriptControl();
            c.Language = "VBScript";            
            dynamic result = c.Eval(subsitutedExpression);
            if(!nodeValues.ContainsKey(node.NodeName))
                nodeValues.Add(node.NodeName, result);
            node.Value = result ;           
        }
       
        public string NodeName { get; set; }
        public string Label { get; set; }
        public string FormulaExpression { get; set; }
        public string Formula { get; set; }         
        protected NodeList<T> Neighbors { get; set; }
        protected NodeList<T> Constituents { get; set; }
        public bool IsModified { get; set; }
        private T nodeValue;

        public Node()
        {
            Neighbors = null;
        }

        public Node(string nodeName, T value) : this(nodeName, value, null, null, null)
        {
        }

        public Node(string nodeName, T value, string formula, string formulaExpression, NodeList<T> neighbors)
        {
            this.NodeName = nodeName;
            this.Value = value;
            this.Formula = formula;
            this.FormulaExpression = formulaExpression;
            this.Neighbors = neighbors;
        }

        public T Value
        {
            get { return nodeValue; }
            set { SetField(ref nodeValue, value, "Value"); }
        }
    }
}
