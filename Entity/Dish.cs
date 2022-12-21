using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoNaObiad.Entity
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int TimeToPrepare { get; set; }
        public int? CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }

        public virtual List<Ingredient> Ingredients { get; set; }

    }
}
