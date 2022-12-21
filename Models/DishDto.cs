using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoNaObiad.Models
{
    public class DishDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int TimeToPrepare { get; set; }

        public List<IngredientDto> Ingredients { get; set; }
    }
}
