using AutoMapper;
using Bookify.DTOs.Reservations;
using Bookify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace Bookify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReservationController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize] // user must be logged in
        public async Task<IActionResult> CreateReservation([FromBody] ReservationCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var isAvailable = await _unitOfWork.Reservations.IsRoomAvailableAsync(dto.RoomId, dto.CheckIn, dto.CheckOut);
            if (!isAvailable)
                return BadRequest("The selected room is not available for the chosen dates.");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user id
            var reservation = new Reservation
            {
                RoomId = dto.RoomId,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                Status = ReservationStatus.Pending,
                UserId = userId 
            };
            await _unitOfWork.Reservations.AddAsync(reservation);
            await _unitOfWork.SaveAsync();
            ReservationReadDTO response = new ReservationReadDTO();
            _mapper.Map(reservation, response);
            var payment = new Payment
            {
                ReservationId = reservation.Id,
                Amount = dto.Amount,
                PaymentType = dto.PaymentType,
                Status = PaymentStatus.Pending
            };
            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveAsync();
            if (dto.PaymentType == PaymentTypeEnum.Cash)
            {
                reservation.Status = ReservationStatus.Confirmed;
                payment.Status = PaymentStatus.Completed;

                await _unitOfWork.SaveAsync();

                return Ok(new
                {
                    message = "Reservation confirmed with cash payment.",
                    reservationId = reservation.Id,
                    paymentId = payment.Id
                });
            }
            if (dto.PaymentType == PaymentTypeEnum.CreditCard)
            {
                var paymentIntentService = new PaymentIntentService();
                var intent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
                {
                    Amount = (long)(dto.Amount * 100),
                    Currency = "usd",
                    Metadata = new Dictionary<string, string>
            {
                { "ReservationId", reservation.Id.ToString() },
                { "PaymentId", payment.Id.ToString() }
            },
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    }
                });

                // Store transaction reference
                payment.TransactionId = intent.Id;
                await _unitOfWork.SaveAsync();

                return Ok(new
                {
                    message = "Complete the payment to confirm reservation.",
                    clientSecret = intent.ClientSecret,
                    reservationId = reservation.Id,
                    paymentId = payment.Id
                });
            }

            return Ok(new { message = "Reservation created successfully", response });
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservations = await _unitOfWork.Reservations.GetReservationsByUserAsync(userId);
            return Ok(reservations);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _unitOfWork.Reservations.GetAllAsync());
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] DateTime checkin, [FromQuery] DateTime checkout)
        {

            var availableRooms = await _unitOfWork.Reservations.GetAvailableRoomsAsync(checkin, checkout);

            if (!availableRooms.Any())
                return NotFound("No rooms available for the selected dates.");

            return Ok(availableRooms);
        }
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateReservationStatusDTO dto)
        {
            var success = await _unitOfWork.Reservations.UpdateStatusAsync(id, dto.Status);
            if (!success) return NotFound();
            return Ok();
        }
    }
}