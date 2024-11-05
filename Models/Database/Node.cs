using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContainerManagement.Models.Database
{
    [Table("Node")]
    public partial class Node
    {
        [Key]
        [Required]
        public string Name { get; set; }

        [Required]
        public string DockerAPI { get; set; }
    }
}