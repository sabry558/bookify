using Microsoft.AspNetCore.Mvc;
using Stripe;
using Bookify.Models;
using System.IO;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class StripeWebhookController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public StripeWebhookController(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var webhookSecret = _configuration["Stripe:WebhookSecret"];

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                webhookSecret
            );
        }
        catch (StripeException)
        {
            return BadRequest();
        }

        // ✅ Compare string constant
        if (stripeEvent.Type == "payment_intent.succeeded")
        {
            var intent = stripeEvent.Data.Object as PaymentIntent;
            if (intent == null) return BadRequest();

            var paymentId = int.Parse(intent.Metadata["PaymentId"]);
            var reservationId = int.Parse(intent.Metadata["ReservationId"]);

            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);

            if (payment != null && reservation != null)
            {
                payment.Status = PaymentStatus.Completed;
                reservation.Status = ReservationStatus.Confirmed;
                await _unitOfWork.SaveAsync();
            }
        }

        return Ok();
    }
}
