using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class Payment
    {
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
    }
    // Payment VM in models

    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Stripe()
        {
            ViewBag.Amount = 4000;
          
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> StripeAsync(string stripeToken, Payment payment)
        {
            try
            {
                ViewBag.error = "";
                //if (string.IsNullOrEmpty(Session["ordertotal"]?.ToString()) || string.IsNullOrEmpty(Session["ordermasterId"]?.ToString()) || HttpContext.Session["UserId"] == null)
                //{
                //    return RedirectToAction("Checkout", "Orders");
                //}
                //var ConversionRate = "";
                //ConversionRate = string.IsNullOrEmpty(Session["ConversionRate"]?.ToString()) ? "1" : Session["ConversionRate"]?.ToString();

                var paymentmodel = new PaymentVM
                {
                    Name = payment.Name,
                    Email = payment.Email,
                    Phone = payment.Phone,
                    Description = string.IsNullOrEmpty(payment.Description) ? "this payment from stripe" : payment.Description,
                    StripeToken = stripeToken,
                    Amount = 4000
                };

                // Create Payment
                StripeConfiguration.ApiKey = "sk_test_51KBa9uANh8Y9GkzKAVgpxGvzjvGirQJhrLSkImtNjjTxZMKZTArKlXAA2SDPEKzTFeCuzFLGyRiGMFcjOLBerpaS00YHRK6ct8";
                var customeroptions = new CustomerCreateOptions
                {
                    Email = payment.Email,
                    Name = payment.Name,
                    Phone = payment.Phone,
                    Description = payment.Description,
                    Source = stripeToken
                };
                var customerservice = new CustomerService();
                var customer = customerservice.Create(customeroptions);
                payment.Description = "this payment from stripe";
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt64(4000),
                    Currency = "usd",
                    Description = payment.Description,
                    //  Source = stripeToken,
                    Customer = customer.Id,
                };
                var service = new ChargeService();
                dynamic charge = service.Create(options);
                dynamic result = charge;

                //ResponseViewModel resmodel  =  HelperFunctions.ResponseHandler(result);
                Charge chargeobj = (Charge)result;
                //if (result != null && chargeobj.Paid == true)
                //{
                //    var ordervm = new OrderVM
                //    {
                //        Id = Convert.ToInt32(Session["ordermasterId"]?.ToString()),
                //        Currency = string.IsNullOrEmpty(Session["currency"]?.ToString()) ? "PKR" : Session["currency"]?.ToString(),
                //        ConversionRate = decimal.Parse(ConversionRate),
                //        PaymentMode = PaymentType.Stripe.ToString(),
                //        Status = OrderStatus.Confirmed.ToString(),
                //        TotalPrice = Convert.ToDecimal(Session["ordertotal"]),
                //        PaymentStatus = true,
                //    };
                //    var dd = await _orders.UpdateOrderMAster(ordervm);
                //    // ------------Remove from cart------------
                //    var customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                //    var cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                //    var removeCart = await _cart.DisableCart(customerId, cookie);

                    return View("PaymentStatus", paymentmodel);
                //}
                //else
                ////{
                //    TempData["error"] = chargeobj.FailureMessage;
                //    return RedirectToAction("Stripe");
               // }
            }
            catch (StripeException ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Stripe");
            }

        }

        [HttpGet]
        public ActionResult PaymentStatus(PaymentVM paymentViewModel)
        {
            return View(paymentViewModel);
        }
    }
}