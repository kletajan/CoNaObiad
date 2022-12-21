using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoNaObiad.Models
{
    public class CreateIngredientDto
    {
        [Required]
        public string Name { get; set; }

        public int DishId { get; set; }
    }
}
