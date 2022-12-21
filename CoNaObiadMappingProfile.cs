using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoNaObiad.Entity;
using CoNaObiad.Models;

namespace CoNaObiad
{//mapowanie bazy do bazy dostępnej dla klienta
    public class CoNaObiadMappingProfile : Profile
    {
        public CoNaObiadMappingProfile()
        {
            CreateMap<Dish, DishDto>();

            CreateMap<Ingredient, IngredientDto>();

            CreateMap<CreateDishDto, Dish>();

            CreateMap<CreateIngredientDto, Ingredient>();
        }
    }
}
