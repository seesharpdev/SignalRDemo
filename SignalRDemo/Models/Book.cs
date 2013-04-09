using System.Xml.Serialization;

namespace SignalRDemo.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        [XmlIgnore]
        public string ImageName { get; set; }
    }
}