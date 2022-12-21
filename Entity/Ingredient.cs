using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoNaObiad.Entity
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int DishId { get; set; }
        public virtual Dish Dish { get; set; }

    }
}
