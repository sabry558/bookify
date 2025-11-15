using AutoMapper;
using Bookify.Dtos.Rooms;
using Bookify.Models;
using Bookify.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Room
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _unitOfWork.Rooms.GetAllAsync();
            //var result = _mapper.Map<IEnumerable<RoomReadDTO>>(rooms);
            return Ok(rooms);
        }

        // GET: api/Room/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id, true);

            if (room == null)
                return NotFound();

            var result = _mapper.Map<RoomReadDTO>(room);
            return Ok(result);
        }

        // POST: api/Room
        [HttpPost]
        [Authorize(Roles = "Admin,Employee")] // only hotel staff can create rooms 
        public async Task<IActionResult> Create([FromBody] RoomCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = _mapper.Map<Room>(dto);

            await _unitOfWork.Rooms.AddAsync(room);
            await _unitOfWork.SaveAsync();

            return Ok(new { message = "Room created successfully", room });
        }

        // PUT: api/Room/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Employee")] 
        public async Task<IActionResult> Update(int id, [FromBody] RoomCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                return NotFound();

            // Map new values into existing entity 
            room.RoomTypeId = dto.RoomTypeId;
            // You may map other properties as needed (RoomNumber, Floor) if DTO exposes them.
            await _unitOfWork.Rooms.UpdateAsync(room);
            await _unitOfWork.SaveAsync();

            return Ok(new { message = "Room updated successfully", room });
        }

        // DELETE: api/Room/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Employee")] 
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                return NotFound();

            await _unitOfWork.Rooms.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            return Ok(new { message = "Room deleted successfully" });
        }
    }
}