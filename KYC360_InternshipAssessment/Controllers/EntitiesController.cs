using KYC360_InternshipAssessment.Models;
using KYC360_InternshipAssessment.Service;
using Microsoft.AspNetCore.Mvc;

namespace KYC360_InternshipAssessment.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class EntitiesController : ControllerBase
    {
        private IEntityService _entityService;

        public EntitiesController(IEntityService entityService)
        {
            _entityService = entityService;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string? search, [FromQuery] Gender? gender, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery(Name = "countries")] List<string> countries, [FromQuery]
            PaginationParameters pagination, bool sortByFirstName = true)
        {

            IEnumerable<Entity> results;
            if (search != null)
            {
                results = _entityService.GetAllBySearchQuery(search);

            }
            else if (gender != null || startDate != DateTime.MinValue || endDate != DateTime.MinValue || countries.Any())
            {
                results = _entityService.GetAllEntitiesByFilter(gender, startDate, endDate, countries);

            }
            else
            {
                results = _entityService.GetAll();
            }


            var paginatedItems = results.Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                .Take(pagination.PageSize)
                                .ToList();

            if (sortByFirstName)
                paginatedItems.Sort((x, y) => string.Compare(x.Names.First().FirstName, y.Names.First().FirstName));

            return Ok(new { TotalItems = results.Count(), Items = paginatedItems });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var entity = _entityService.GetById(id);

            if (entity == null) return NotFound("Entity does not exist");
            return Ok(entity);
        }

        [HttpPost]
        public IActionResult Create(CreateRequest model)
        {
            var id = _entityService.Create(model);
            return Ok(new { message = $"Entity created with ID {id}" });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateRequest model)
        {
            var result = _entityService.Update(id, model);

            return (result == true) ? Ok(new { message = "Entity updated" }) : NotFound();

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            var result = _entityService.DeleteById(id);

            return (result == true) ? Ok() : NotFound();
        }
    }
}
