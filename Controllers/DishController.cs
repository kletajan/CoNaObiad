using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoNaObiad.Entity;
using CoNaObiad.Models;
using CoNaObiad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace CoNaObiad.Controllers
{
    [Route("api/dish")]
    [ApiController]
    [Authorize] //wszystkie akcje w tym kontrolerze wymagaja autoryzacji
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        //aktualizacja dania
        [HttpPut("{id}") ]
        public ActionResult Update([FromBody] UpdateDishDto dto, [FromRoute] int id)
        {
            _dishService.Update(id, dto);

            return Ok();
        }

        //usuwanie dań
        [HttpDelete("{id}") ]
        public ActionResult Delete([FromRoute] int id)
        {
            _dishService.Delete(id);

            return NoContent();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Manager")]
        public ActionResult CreateDish([FromBody] CreateDishDto dto)
        {
           var id = _dishService.Create(dto);

           return Created($"/api/dish/{id}", null);
        }

        //pobieramy wszystkie dania
        [HttpGet]      
        [Authorize(Policy = "Atleast20")]
        public ActionResult<IEnumerable<DishDto>> GetAll([FromQuery]DishQuery query)
        {
;           var dishesDtos = _dishService.GetAll(query);

            return Ok(dishesDtos);
        }
         
        [HttpGet("{id}")]
        [AllowAnonymous] //mimo autoryzacji na calosc to jes wyjatek
        public ActionResult<DishDto> Get([FromRoute] int id)
        {
            var dish = _dishService.GetById(id);

            return Ok(dish);

        }
    }
}
