using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace MusicHub.DataProcessor.Export_Dtos
{
    [XmlType("Song")]
    public class ExportSongDto
    {
        [XmlElement("SongName")]
        [Required, MinLength(3), MaxLength(20)]
        public string SongName { get; set; }

        [XmlElement("Writer")]
        public string Writer { get; set; }

        [XmlElement("Performer")]
        public string Performer { get; set; }

        [XmlElement("AlbumProducer")]
        public string AlbumProducer { get; set; }

        [Required]
        public string Duration { get; set; }
    }
}
