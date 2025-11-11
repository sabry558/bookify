using Bookify.DTOs.Reservation;
using Bookify.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _unitOfWork.Reservations.GetAllAsync();
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById([FromRoute] int id)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
            if (reservation == null)
                return NotFound();
            return Ok(reservation);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] DateTime checkin, [FromQuery] DateTime checkout)
        {

            var availableRooms = await _unitOfWork.Reservations.GetAvailableRoomsAsync(checkin, checkout);

            if (!availableRooms.Any())
                return NotFound("No rooms available for the selected dates.");

            return Ok(availableRooms);
        }
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var isAvailable = await _unitOfWork.Reservations.IsRoomAvailableAsync(dto.RoomId, dto.CheckIn, dto.CheckOut);
            if (!isAvailable)
                return BadRequest("The selected room is not available for the chosen dates.");
            var reservation = new Reservation
            {
                RoomId = dto.RoomId,
                UserId = dto.UserId,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                Status = ReservationStatus.Confirmed
            };
            await _unitOfWork.Reservations.AddAsync(reservation);
            await _unitOfWork.SaveAsync();
            return Ok(new { message = "Reservation created successfully", reservation });
        }

    }
}
