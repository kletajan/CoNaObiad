using CoNaObiad.Models;
using CoNaObiad.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoNaObiad.Controllers
{
        [Route("api/dish/{dishId}/ingredient")]
        [ApiController]
        public class IngredientController : ControllerBase
        {
            private readonly IIngredientService _ingredientService;

            public IngredientController(IIngredientService ingredientService)
            {
                _ingredientService = ingredientService;
            }
            [HttpDelete]
            public ActionResult Delete([FromRoute] int dishId)
            {
                _ingredientService.RemoveAll(dishId);
                return NoContent();
            }
            [HttpPost]
            public ActionResult Post([FromRoute]int dishId, [FromBody] CreateIngredientDto dto)
            {
                var newIngredientId = _ingredientService.Create(dishId, dto);

                return Created($"api/dish/{dishId}/ingredient/{newIngredientId}", null);
            }

        [HttpGet("{ingredientId}")]
        public ActionResult<IngredientDto> Get([FromRoute] int dishId, [FromRoute] int ingredientId)
        {
            IngredientDto ingredient = _ingredientService.GetById(dishId, ingredientId);
            return Ok(ingredient);
        }

        [HttpGet]
        public ActionResult<List<IngredientDto>> Get([FromRoute] int dishId)
        {
            var result = _ingredientService.GetAll(dishId);
            return Ok(result);
        }
    }
}
