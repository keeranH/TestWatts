using System.Collections.Generic;
using Business.CalculatorEngine;
using Econocom.Business.CalculatorEngine;

namespace Econocom.Calculateur
{
    public class Graph<T> : IEnumerable<T>
    {
        private NodeList<T> nodeSet;

        public Graph() : this(null) { }
        public Graph(NodeList<T> nodeSet)
        {
            if (nodeSet == null)
                this.nodeSet = new NodeList<T>();
            else
                this.nodeSet = nodeSet;
        }

        public void AddNode(GraphNode<T> node)
        {
            // adds a node to the graph
            nodeSet.Add(node);
        }

        public void AddNode(string nodeName, T value, string formula, string formulaExpression)
        {
            // adds a node to the graph
            nodeSet.Add(new GraphNode<T>(nodeName, value, formula, formulaExpression, null));
        }

        public void AddDirectedEdge(GraphNode<float> from, GraphNode<float> to, int cost, float? value)
        {
            from.Neighbors.Add(to);
            to.Constituents.Add(from);
        }

        public void AddDirectedEdge(string from, string to, float? value)
        {
            var fromNode = GetNode(from);

            var toNode = GetNode(to);//new GraphNode<T>(to.ToString(), value);
            fromNode.Neighbors.Add(toNode);
            toNode.Constituents.Add(fromNode);
        }

        public void AddUndirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
        {
            from.Neighbors.Add(to);            
            to.Constituents.Add(from);
        }

        public bool Contains(string value)
        {
            return nodeSet.FindByValue(value) != null;
        }

        public GraphNode<T> GetNode(string value)
        {
            return (GraphNode<T>)nodeSet.FindByName(value);
        }


        public bool Remove(string value)
        {
            // first remove the node from the nodeset
            GraphNode<T> nodeToRemove = (GraphNode<T>)nodeSet.FindByValue(value);
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            // otherwise, the node was found
            nodeSet.Remove(nodeToRemove);

            // enumerate through each node in the nodeSet, removing edges to this node
            foreach (GraphNode<T> gnode in nodeSet)
            {
                int index = gnode.Neighbors.IndexOf(nodeToRemove);
                if (index != -1)
                {
                    // remove the reference to the node and associated cost
                    gnode.Neighbors.RemoveAt(index);
                }
            }

            return true;
        }

        public NodeList<T> Nodes
        {
            get
            {
                return nodeSet;
            }
        }

        public int Count
        {
            get { return nodeSet.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return null;
        }

        public float Process()
        {
            foreach (GraphNode<T> node in nodeSet)
            {
                //if (node.Constituents == null)
                //    ProcessValue();
                //else
                //{
                //    node.
                //}
            }
            return 0;
        }

       
             
    }
}
