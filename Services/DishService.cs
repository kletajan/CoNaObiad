using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CoNaObiad.Authorization;
using CoNaObiad.Entity;
using CoNaObiad.Models;
using CoNaObiad.Exceptions;

namespace CoNaObiad.Services
{
    public interface IDishService
    {
        DishDto GetById(int id);
        PagedResult<DishDto> GetAll(DishQuery query);
        int Create(CreateDishDto dto);
        void Delete(int id);
        void Update(int id, UpdateDishDto dto);
    }

    public class DishService : IDishService
    {
        private readonly DishDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<DishService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public DishService(DishDbContext dbContext, IMapper mapper, ILogger<DishService> logger
            , IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }
        public int Create(CreateDishDto dto)
        {
            var dish = _mapper.Map<Dish>(dto);
            dish.CreatedById = _userContextService.GetUserId;
            _dbContext.Dishes.Add(dish);
            _dbContext.SaveChanges();

            return dish.Id;
        }

        public void Update(int id, UpdateDishDto dto)
        {
            //_logger.LogWarning($"Dish with id: {id} DELETE action invoked");

            var dish = _dbContext
            .Dishes
            .FirstOrDefault(d => d.Id == id);

            if (dish is null)
                throw new NotFoundException("Dish not found");


            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, dish,
               new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            dish.Name = dto.Name;
            dish.Category = dto.Category;
            dish.TimeToPrepare = dto.TimeToPrepare;

            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            _logger.LogError($"Dish with id:{id} DELETE action invoked");

            var dish = _dbContext
            .Dishes
            .FirstOrDefault(d => d.Id == id);

            if (dish is null)
                throw new NotFoundException("Dish not found");


            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, dish,
               new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            _dbContext.Dishes.Remove(dish);
            _dbContext.SaveChanges();

        }

        public DishDto GetById(int id)
        {
            var dish = _dbContext
            .Dishes
            .Include(d => d.Ingredients)
            .FirstOrDefault(d => d.Id == id);

            if (dish is null)
                throw new NotFoundException("Dish not found");

            var result = _mapper.Map<DishDto>(dish);
            return result;
        }

        public PagedResult<DishDto> GetAll(DishQuery query)
        {
            var baseQuery = _dbContext
                .Dishes
                .Include(d => d.Ingredients)
                .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                            || r.Category.ToLower().Contains(query.SearchPhrase.ToLower())));

            if(!string.IsNullOrEmpty(query.SortBy))
            {
                var columnSelectors = new Dictionary<string, Expression<Func<Dish, object>>>
                    {
                    {nameof(Dish.Name), r => r.Name },
                    {nameof(Dish.Category), r => r.Category },
                    {nameof(Dish.TimeToPrepare), r => r.TimeToPrepare },
                };

                var selectedColumn = columnSelectors[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var dishes = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();


            var totalItemsCount = baseQuery.Count();

            var dishesDtos = _mapper.Map<List<DishDto>>(dishes);

            var result = new PagedResult<DishDto>(dishesDtos, totalItemsCount, query.PageSize, query.PageNumber);


            return result;
        }
    }
}
