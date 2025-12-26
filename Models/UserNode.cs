namespace SocialNetworkAnalysis.Models
{
    
    public class UserNode
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        
        [cite_start]
        public double Activity { get; set; }        
        public double Interaction { get; set; }     
        public int ConnectionCount { get; set; }    

        
        public double X { get; set; }
        public double Y { get; set; }
        
        [cite_start]
        public string Color { get; set; } = "Gray"; 

        public UserNode(int id, string name, double activity, double interaction, int connectionCount)
        {
            Id = id;
            Name = name;
            Activity = activity;
            Interaction = interaction;
            ConnectionCount = connectionCount;
        }

        public override string ToString()
        {
            return $"{Id}: {Name} (Act: {Activity}, Int: {Interaction})";
        }
    }
}