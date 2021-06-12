using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Payment.DatModel.DomainModel.Enumerations;
using Payment.Service;
using Payment.Service.Dto.Payment;
using Payment.Web.Api.Filters;
using System;
using System.Threading.Tasks;
namespace Payment.Web.Api
{
    [Route("api/authorize")]
    [Produces("application/json")]
    [FormatFilter]
    public class PaymentController : Controller
    {

        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {

            _paymentService = paymentService ?? throw new ArgumentException(nameof(paymentService));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }


        /// <summary>
        /// This method used for Get all the payment detail.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderReference"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> Get(int top = 10, int skip = 0)
        {

            try
            {
                var result = await _paymentService.Get(top, skip);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on Get all: {ex}");
                return BadRequest(JsonConvert.SerializeObject(ex.Message));
            }
        }
        /// <summary>
        /// This method used for Get all the payment detail for authorization
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderReference"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        [HttpPost()]
        [ValidateModel]
        public async Task<IActionResult> GetAuthorization([FromBody] PaymentRequestDto paymentRequest)
        {

            try
            {
                var result = await _paymentService.GetAuthorize(paymentRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on GetAuthorization controller: {ex}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This method used for Get all the payment detail for voided.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderReference"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        [HttpPost("/{id}/voids")]
        public async Task<IActionResult> GetVoid([System.Web.Http.FromUri] Guid id, [FromBody] string orderReference)
        {

            try
            {
                var result = await _paymentService.GetById(id, orderReference, Status.Voided);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on GetVoid: {ex}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This method used for Get all the payment detail for capture.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderReference"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        [HttpPost("/{id}/capture/")]
        public async Task<IActionResult> GetCapture([System.Web.Http.FromUri] Guid id, [FromBody] string orderReference)
        {

            try
            {
                var result = await _paymentService.GetById(id, orderReference, Status.Captured);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on GetCapture: {ex}");
                return BadRequest(ex.Message);
            }
        }

    }
}
