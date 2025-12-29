using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetworkAnalysis.Models;

namespace SocialNetworkAnalysis.Services
{
    public class AlgorithmService
    {
        
        public UserNode FindMostPopularUser(GraphService graph)
        {
            return graph.Nodes.Values.OrderByDescending(u => u.ConnectionCount).FirstOrDefault();
        }

        
        public List<UserNode> FindShortestPath(GraphService graph, UserNode start, UserNode end)
        {
            var queue = new Queue<UserNode>();
            var previous = new Dictionary<UserNode, UserNode>(); 
            var visited = new HashSet<UserNode>(); 

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

        
        public List<UserNode> FindPathDijkstra(GraphService graph, UserNode start, UserNode end)
        {
            var distances = new Dictionary<UserNode, double>();
            var previous = new Dictionary<UserNode, UserNode>();
            var nodes = new List<UserNode>();

            
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

        
        private List<UserNode> ReconstructPath(Dictionary<UserNode, UserNode> previous, UserNode start, UserNode end)
        {
            var path = new List<UserNode>();
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

        
        private List<UserNode> GetNeighbors(GraphService graph, UserNode node)
        {
            var neighbors = new List<UserNode>();
            foreach (var edge in graph.Edges)
            {
                if (edge.Source == node) neighbors.Add(edge.Target);
                else if (edge.Target == node) neighbors.Add(edge.Source);
            }
            return neighbors;
        }
    }
}