using AutoMapper;
using CoNaObiad.Entity;
using CoNaObiad.Exceptions;
using CoNaObiad.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoNaObiad.Services
{
    public interface IIngredientService
    {
        int Create(int dishId, CreateIngredientDto dto);
        IngredientDto GetById(int dishId, int ingredientId);
        List<IngredientDto> GetAll(int dishId);
        void RemoveAll(int dishId);
    }
    public class IngredientService : IIngredientService
    {
        private readonly DishDbContext _context;
        private readonly IMapper _mapper;

        public IngredientService(DishDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public int Create(int dishId, CreateIngredientDto dto)
        {
            var dish = GetDishById(dishId);

            var ingredientEntity = _mapper.Map<Ingredient>(dto);

            ingredientEntity.DishId = dishId;

            _context.Ingredients.Add(ingredientEntity);
            _context.SaveChanges();

            return ingredientEntity.Id;
        }

        public IngredientDto GetById(int dishId, int ingredientId)
            {
            var dish = GetDishById(dishId);

            var ingredient = _context.Ingredients.FirstOrDefault(i => i.Id == ingredientId);
            if (ingredient is null || ingredient.DishId != ingredientId)
            {
                throw new NotFoundException("Dish not found");
            }

            var ingredientDto = _mapper.Map<IngredientDto>(ingredient);
            return ingredientDto;

        }

        public List<IngredientDto> GetAll(int dishId)
        {
            var dish = GetDishById(dishId);

            var ingredientDtos = _mapper.Map<List<IngredientDto>>(dish.Ingredients);

            return ingredientDtos;
        }

        public void RemoveAll(int dishId)
        {
            var dish = GetDishById(dishId);

            _context.RemoveRange(dish.Ingredients);
            _context.SaveChanges();
        }

        private Dish GetDishById(int dishId)
        {
            var dish = _context.Dishes.Include(d => d.Ingredients).FirstOrDefault(d => d.Id == dishId);

            if (dish is null)
                throw new NotFoundException("Dish not found");

            return dish;
        }
    }
}
