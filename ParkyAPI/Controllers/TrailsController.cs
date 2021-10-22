using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    [Route("api/Trails")]
     //[Route("api/v{version:apiVersion}/nationalparks")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailController : Controller
    {
        private readonly ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of trail.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200,Type = typeof(List<TrailDto>))]

        public IActionResult GetTrails()
        {
            var trailsList = _trailRepo.GetTrails();

            var dtoTrailsList = new List<TrailDto>();

            foreach (var item in trailsList)
            {
                dtoTrailsList.Add(_mapper.Map<TrailDto>(item));
            }



            //dtoTrailsList = ((from d in trailsList
            //                         select new TrailDto()
            //                         {
            //                             Id = d.Id,
            //                             Name = d.Name,
            //                             State = d.State,
            //                             Created = d.Created,
            //                             Established = d.Established
            //                         })).ToList();

            return Ok(dtoTrailsList);

        }

        /// <summary>
        /// Get individual trail
        /// </summary>
        /// <param name="trailId">Id of the trail</param>
        /// <returns></returns>
        [HttpGet("{trailId:int}",Name ="GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int trailId)
        {

            var trail = _trailRepo.GetTrail(trailId);

            if (trail == null)
            {
                return NotFound();
            }
            var dtoTrail = new TrailDto();


            dtoTrail = _mapper.Map<TrailDto>(trail);


            return Ok(dtoTrail);

        }

        //[HttpGet("GetTrailInNationalPark/{nationalParkId:int}")]
        [HttpGet("[action]/{nationalParkId:int}")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrailsInNationalPark(int nationalParkId)
        {

            var objList = _trailRepo.GetTrailsInNationalPark(nationalParkId);

            if (objList == null)
            {
                return NotFound();
            }

            var objDto = new List<TrailDto>();
            foreach (var obj in objList)
            {
                 objDto.Add(_mapper.Map<TrailDto>(obj));
            }

           


            return Ok(objDto);

        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_trailRepo.TrailExits(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(404, ModelState);
            }
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var trailObj = _mapper.Map<Trail>(trailDto);

            if (!_trailRepo.CreateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrail", new {trailId=trailObj.Id},trailObj);


            
        }


        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpdateDto trailDto)
        {
            if (trailDto == null || trailId!= trailDto.Id)
            {
                return BadRequest(ModelState);
            }
            

            var trailObj = _mapper.Map<Trail>(trailDto);

            if (!_trailRepo.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailId)
        {

            if (!_trailRepo.TrailExits(trailId))
            {
                return NotFound();
            }

            var trailObj = _trailRepo.GetTrail(trailId);



            if (!_trailRepo.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
