using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetworkAnalysis.Models;

namespace SocialNetworkAnalysis.Services
{
    public class AlgorithmService
    {
        public Node? FindMostPopularUser(IGraphService graph)
        {
            return graph.Nodes.Values.OrderByDescending(u => u.ConnectionCount).FirstOrDefault();
        }

        public List<Node>? FindShortestPath(IGraphService graph, Node start, Node end)
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

        
        public List<Node>? FindPathDijkstra(IGraphService graph, Node start, Node end)
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
                    
                    
                    
                    double diffActivity = Math.Pow(current.Activity - neighbor.Activity, 2);
                    double diffInteraction = Math.Pow(current.Interaction - neighbor.Interaction, 2);
                    double diffConnections = Math.Pow(current.ConnectionCount - neighbor.ConnectionCount, 2); 

                    
                    double denominator = 1.0 + Math.Sqrt(diffActivity + diffInteraction + diffConnections);

                    
                    double weight = 1.0 / denominator;

                    

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

        public List<Node> DFS(IGraphService graph, Node start)
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

        public List<Node> GetTop5Users(IGraphService graph)
        {
            return graph.Nodes.Values
                .OrderByDescending(u => u.ConnectionCount) 
                .Take(5) 
                .ToList();
        }

        public List<List<Node>> FindConnectedComponents(IGraphService graph)
        {
            var components = new List<List<Node>>();
            var visited = new HashSet<Node>();

            foreach (var node in graph.Nodes.Values)
            {
                if (!visited.Contains(node))
                {
                    var component = DFS(graph, node);
                    components.Add(component);

                    foreach (var n in component)
                    {
                        visited.Add(n);
                    }
                }
            }
            return components;
        }

        private List<Node>? ReconstructPath(Dictionary<Node, Node> previous, Node start, Node end)
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

        private List<Node> GetNeighbors(IGraphService graph, Node node)
        {
            var neighbors = new List<Node>();
            foreach (var edge in graph.Edges)
            {
                if (edge.Source == node) neighbors.Add(edge.Target);
                else if (edge.Target == node) neighbors.Add(edge.Source);
            }
            return neighbors;
        }
        
        public List<Node>? FindPathAStar(IGraphService graph, Node start, Node end)
        {
            var distances = new Dictionary<Node, double>();
            var previous = new Dictionary<Node, Node>();
            var openSet = new List<Node>(); 

            foreach (var node in graph.Nodes.Values)
                distances[node] = double.MaxValue;

            distances[start] = 0;
            openSet.Add(start);

            while (openSet.Count > 0)
            {
                
                openSet.Sort((a, b) => 
                {
                    double fA = distances[a] + CalculateHeuristic(a, end);
                    double fB = distances[b] + CalculateHeuristic(b, end);
                    return fA.CompareTo(fB);
                });

                var current = openSet[0];
                openSet.Remove(current);

                if (current == end) break;

                foreach (var neighbor in GetNeighbors(graph, current))
                {
                    
                    double weight = CalculateDynamicWeight(current, neighbor); 

                    double newDist = distances[current] + weight;
                    if (newDist < distances[neighbor])
                    {
                        distances[neighbor] = newDist;
                        previous[neighbor] = current;
                        if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                    }
                }
            }
            return ReconstructPath(previous, start, end);
        }

       
        private double CalculateHeuristic(Node a, Node b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        private double CalculateDynamicWeight(Node current, Node neighbor)
        {
             double diffActivity = Math.Pow(current.Activity - neighbor.Activity, 2);
             double diffInteraction = Math.Pow(current.Interaction - neighbor.Interaction, 2);
             double diffConnections = Math.Pow(current.ConnectionCount - neighbor.ConnectionCount, 2);
             double denominator = 1.0 + Math.Sqrt(diffActivity + diffInteraction + diffConnections);
             return 1.0 / denominator;
        }
    }
}