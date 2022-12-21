using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoNaObiad.Entity;
using Microsoft.EntityFrameworkCore;

namespace CoNaObiad
{
    public class CoNaObiadSeeder
    {
        private readonly DishDbContext _dbContext;
        public CoNaObiadSeeder(DishDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Seed()
        {
            if(_dbContext.Database.CanConnect())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();
                if(pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }
                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }

                if(!_dbContext.Dishes.Any())
                {
                    var dishes = GetDishes();
                    _dbContext.Dishes.AddRange(dishes);
                    _dbContext.SaveChanges();
                }
            }
        }
        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "User"
                },
                new Role()
                {
                    Name = "Manager"
                },
                new Role()
                {
                    Name = "Admin"
                },
            };

            return roles;
        }
        private IEnumerable<Dish>GetDishes()
        {
            var dishes = new List<Dish>()
            {
                new Dish()
                {
                    Name = "Jajecznica",
                    Category = "Śniadanie",
                    TimeToPrepare = 300,
                    Ingredients = new List<Ingredient>()
                    {
                        new Ingredient()
                        {
                            Name = "Jajo",
                        },
                        new Ingredient()
                        {
                            Name = "masło",
                        },
                    },
                },
                new Dish()
                {
                    Name = "Spaghetti Bolognese",
                    Category = "Obiad",
                    TimeToPrepare = 1200,
                    Ingredients = new List<Ingredient>()
                    {
                        new Ingredient()
                        {
                            Name = "makaron spaghetti",
                        },
                        new Ingredient()
                        {
                            Name = "Pasatta Pomidorowa",
                        },
                        new Ingredient()
                        {
                            Name = "Mielone mięso wołowe",
                        },
                        new Ingredient()
                        {
                            Name = "cebula",
                        },
                        new Ingredient()
                        {
                            Name = "czosnek",
                        },
                        new Ingredient()
                        {
                            Name = "olej",
                        },
                    },
                },
            };
            return dishes;
        }
    }
}
