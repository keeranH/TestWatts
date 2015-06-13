using Business.CalculatorEngine;

namespace Econocom.Business.CalculatorEngine
{
    public class GraphNode<T> : Node<T>
    { 
        public GraphNode() : base() { }
        public GraphNode(string nodeName, T value) : base(nodeName, value) { }
        public GraphNode(string nodeName, T value, string formula, string formulaExpression, NodeList<T> neighbors) : base(nodeName, value, formula, formulaExpression, neighbors) { }

        new public NodeList<T> Neighbors
        {
            get
            {
                if (base.Neighbors == null)
                    base.Neighbors = new NodeList<T>();

                return base.Neighbors;
            }
        }

        new public NodeList<T> Constituents
        {
            get
            {
                if (base.Constituents == null)
                    base.Constituents = new NodeList<T>();

                return base.Constituents;
            }
        }  
    }
}
