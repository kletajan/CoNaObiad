using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoNaObiad.Models
{
    public class CreateDishDto
    {
        [Required]
        public string Name { get; set; }
        public string Category { get; set; }
        public int TimeToPrepare { get; set; }

    }
}
