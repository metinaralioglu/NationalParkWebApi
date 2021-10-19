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
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : Controller
    {
        private INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var nationalParksList = _npRepo.GetNationalParks();

            var dtoNationalParksList = new List<NationalParkDto>();

            foreach (var item in nationalParksList)
            {
                dtoNationalParksList.Add(_mapper.Map<NationalParkDto>(item));
            }



            //dtoNationalParksList = ((from d in nationalParksList
            //                         select new NationalParkDto()
            //                         {
            //                             Id = d.Id,
            //                             Name = d.Name,
            //                             State = d.State,
            //                             Created = d.Created,
            //                             Established = d.Established
            //                         })).ToList();

            return Ok(dtoNationalParksList);

        }
        [HttpGet("{nationalParkId:int}",Name ="GetNationalPark")]
        public IActionResult GetNationalPark(int nationalParkId)
        {

            var nationalPark = _npRepo.GetNationalPark(nationalParkId);

            if (nationalPark == null)
            {
                return NotFound();
            }
            var dtoNationalPark = new NationalParkDto();


            dtoNationalPark = _mapper.Map<NationalParkDto>(nationalPark);


            return Ok(dtoNationalPark);

        }

        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_npRepo.NationalParkExits(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_npRepo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new {nationalParkId=nationalParkObj.Id},nationalParkObj);


            
        }


        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null || nationalParkId!= nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }
            if (_npRepo.NationalParkExits(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_npRepo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }
        }
    }
}
