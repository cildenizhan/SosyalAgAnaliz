namespace SocialNetworkAnalysis.Models
{
    public class UserNode
    {
        public int Id { get; set; }
        
        
        public string? Name { get; set; } 
        
        public double Activity { get; set; }        
        public double Interaction { get; set; }     
        public int ConnectionCount { get; set; }    
        public double X { get; set; }
        public double Y { get; set; }
        
        
        public string Color { get; set; } = "Gray"; 

        public UserNode() { }
    }
}