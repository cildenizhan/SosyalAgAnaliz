using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetworkAnalysis.Models;

namespace SocialNetworkAnalysis.Services
{
    public class AlgorithmService
    {
        
        public Node FindMostPopularUser(GraphService graph)
        {
            return graph.Nodes.Values.OrderByDescending(u => u.ConnectionCount).FirstOrDefault();
        }

        
        public List<Node> FindShortestPath(GraphService graph, Node start, Node end)
        {
            var queue = new Queue<Node>();
            var previous = new Dictionary<Node, Node>(); 
            var visited = new HashSet<Node>(); 

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == end) break; 

                var neighbors = GetNeighbors(graph, current);

                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        previous[neighbor] = current; 
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return ReconstructPath(previous, start, end);
        }

        
        public List<Node> FindPathDijkstra(GraphService graph, Node start, Node end)
        {
            var distances = new Dictionary<Node, double>();
            var previous = new Dictionary<Node, Node>();
            var nodes = new List<Node>();

            
            foreach (var node in graph.Nodes.Values)
            {
                if (node == start) distances[node] = 0;
                else distances[node] = double.MaxValue;
                
                nodes.Add(node);
            }

            while (nodes.Count > 0)
            {
                
                nodes.Sort((x, y) => distances[x].CompareTo(distances[y]));
                var current = nodes[0];
                nodes.Remove(current);

                if (current == end) break;
                if (distances[current] == double.MaxValue) break; 

                
                foreach (var neighbor in GetNeighbors(graph, current))
                {
                    
                    double weight = Math.Sqrt(Math.Pow(current.X - neighbor.X, 2) + Math.Pow(current.Y - neighbor.Y, 2));
                    
                    double alt = distances[current] + weight;
                    
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = current;
                    }
                }
            }

            return ReconstructPath(previous, start, end);
        }

        
        private List<Node> ReconstructPath(Dictionary<Node, Node> previous, Node start, Node end)
        {
            var path = new List<Node>();
            var temp = end;

            if (!previous.ContainsKey(end)) return null; 

            while (temp != null && previous.ContainsKey(temp))
            {
                path.Add(temp);
                temp = previous[temp];
            }
            path.Add(start); 
            path.Reverse(); 
            return path;
        }

        
        private List<Node> GetNeighbors(GraphService graph, Node node)
        {
            var neighbors = new List<Node>();
            foreach (var edge in graph.Edges)
            {
                if (edge.Source == node) neighbors.Add(edge.Target);
                else if (edge.Target == node) neighbors.Add(edge.Source);
            }
            return neighbors;
        }
        

        
        private bool IsNeighbor(GraphService graph, Node u1, Node u2)
        {
            return graph.Edges.Any(e => (e.Source == u1 && e.Target == u2) || (e.Source == u2 && e.Target == u1));
        }

        
        private bool CanBeColored(GraphService graph, Node node, string color, Dictionary<Node, string> currentColors)
        {
            var neighbors = GetNeighbors(graph, node);
            foreach (var neighbor in neighbors)
            {
                if (currentColors.ContainsKey(neighbor) && currentColors[neighbor] == color)
                {
                    return false; 
                }
            }
            return true;
        }
        
        public List<Node> DFS(GraphService graph, Node start)
        {
            var visited = new HashSet<Node>();
            var stack = new Stack<Node>();
            var result = new List<Node>();

            stack.Push(start);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    result.Add(current);

                    
                    foreach (var neighbor in GetNeighbors(graph, current))
                    {
                        if (!visited.Contains(neighbor))
                        {
                            stack.Push(neighbor);
                        }
                    }
                }
            }
            return result;
        }

        
        public List<Node> GetTop5Users(GraphService graph)
        {
            return graph.Nodes.Values
                .OrderByDescending(u => u.ConnectionCount) 
                .Take(5) 
                .ToList();
        }

    }
}