using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json; 
using System.Text.Encodings.Web; 
using System.Text.Unicode; 
using System.Globalization; 
using SocialNetworkAnalysis.Models;

namespace SocialNetworkAnalysis.Services
{
    public class FileService
    {
        
        public GraphService LoadGraph(string filePath)
        {
            var graphService = new GraphService();
            
            if (!File.Exists(filePath)) return graphService;

            var lines = File.ReadAllLines(filePath);
            
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(',');
                if (parts.Length < 5) continue;

                try 
                {
                    int id = int.Parse(parts[0]);
                    string name = parts[1];

                    
                    double activity = double.Parse(parts[2], CultureInfo.InvariantCulture); 
                    double interaction = double.Parse(parts[3], CultureInfo.InvariantCulture);
                    
                    int connectionCount = int.Parse(parts[4]);
                    string neighborsStr = parts[5].Trim('"');

                    Node node = new Node
                    {
                        Id = id,
                        Name = name,
                        Activity = activity,
                        Interaction = interaction,
                        ConnectionCount = connectionCount
                    };
                    graphService.AddNode(node);

                    if (!string.IsNullOrEmpty(neighborsStr))
                    {
                        var neighborIds = neighborsStr.Split(';');
                        foreach (var nIdStr in neighborIds)
                        {
                            if (int.TryParse(nIdStr, out int targetId))
                            {
                                graphService.AddEdge(id, targetId);
                            }
                        }
                    }
                }
                catch
                {
                    continue; 
                }
            }
            return graphService;
        }

        
        public void SaveGraphJson(IGraphService graphService, string filePath)
        {
            
            var data = new GraphDataDTO
            {
                Nodes = graphService.Nodes.Values.ToList(),
                Edges = graphService.Edges.Select(e => new EdgeDTO 
                { 
                    SourceId = e.Source.Id, 
                    TargetId = e.Target.Id, 
                    Weight = e.Weight 
                }).ToList()
            };

            
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) 
            };

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
        }

        
        public GraphService LoadGraphJson(string filePath)
        {
            var graphService = new GraphService();
            if (!File.Exists(filePath)) return graphService;

            string json = File.ReadAllText(filePath);
            
            
            if (string.IsNullOrWhiteSpace(json)) return graphService;

            var data = JsonSerializer.Deserialize<GraphDataDTO>(json);

            if (data == null) return graphService;

            
            foreach (var node in data.Nodes)
            {
                graphService.AddNode(node);
            }

            
            foreach (var edgeDto in data.Edges)
            {
                graphService.AddEdge(edgeDto.SourceId, edgeDto.TargetId);
            }

            return graphService;
        }
    }

   
    public class GraphDataDTO
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<EdgeDTO> Edges { get; set; } = new List<EdgeDTO>();
    }

    public class EdgeDTO
    {
        public int SourceId { get; set; }
        public int TargetId { get; set; }
        public double Weight { get; set; }
    }
}