﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ImportDtos
{
    [XmlType("Song")]
    public class ImportSongsDto
    {
        [XmlElement("Name")]
        [Required, MinLength(3), MaxLength(20)]
        public string Name { get; set; }

        [XmlElement("Duration")]
        [Required]
        public string Duration { get; set; }

        [XmlElement("CreatedOn")]
        [Required]
        public string CreatedOn { get; set; }

        [XmlElement("Genre")]
        [Required]
        public string Genre { get; set; }

        [XmlElement("AlbumId")]
        public int? AlbumId { get; set; }

        [XmlElement("WriterId")]
        [Required]
        public int WriterId { get; set; }

        [XmlElement("Price")]
        [Required, Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
