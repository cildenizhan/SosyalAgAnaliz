using System.Collections.Generic;
using System.Linq;
using System.Windows.Media; 
using SocialNetworkAnalysis.Models;
using SocialNetworkAnalysis.Services;

namespace SocialNetworkAnalysis.Algorithms
{
    public class WelshPowell
    {
        
        public Dictionary<Node, string> ColorGraph(IGraphService graph)
        {
            string[] colors = { "Red", "Blue", "Green", "Orange", "Purple", "Pink", "Brown", "Cyan", "Magenta", "Lime" };
            var nodeColors = new Dictionary<Node, string>();
            
            
            var sortedNodes = graph.Nodes.Values.OrderByDescending(n => n.ConnectionCount).ToList();
            int colorIndex = 0;

            while (sortedNodes.Count > 0)
            {
                string currentColor = colorIndex < colors.Length ? colors[colorIndex] : "Gray";
                var root = sortedNodes[0];
                nodeColors[root] = currentColor;
                sortedNodes.RemoveAt(0);

                var nodesToColor = new List<Node>();

                foreach (var node in sortedNodes)
                {
                    if (!IsNeighbor(graph, root, node) && CanBeColored(graph, node, currentColor, nodeColors))
                    {
                        nodeColors[node] = currentColor;
                        nodesToColor.Add(node);
                    }
                }

                foreach (var node in nodesToColor) sortedNodes.Remove(node);
                colorIndex++;
            }
            return nodeColors;
        }

        private bool IsNeighbor(IGraphService graph, Node u1, Node u2)
        {
            return graph.Edges.Any(e => (e.Source == u1 && e.Target == u2) || (e.Source == u2 && e.Target == u1));
        }

        private bool CanBeColored(IGraphService graph, Node node, string color, Dictionary<Node, string> currentColors)
        {
            
            var neighbors = new List<Node>();
            foreach (var edge in graph.Edges)
            {
                if (edge.Source == node) neighbors.Add(edge.Target);
                else if (edge.Target == node) neighbors.Add(edge.Source);
            }

            
            foreach (var neighbor in neighbors)
            {
                if (currentColors.ContainsKey(neighbor) && currentColors[neighbor] == color) return false;
            }
            return true;
        }
    }
}