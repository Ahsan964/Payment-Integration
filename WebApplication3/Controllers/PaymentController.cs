using Microsoft.Build.Utilities;
using PayPal.Api;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;
using Address = PayPal.Api.Address;

namespace WebApplication3.Controllers
{
    public class payment
    {
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string intent { get; internal set; }
        public Payer payer { get; internal set; }
        public List<Transaction> transactions { get; internal set; }
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
        public async Task<ActionResult> StripeAsync(string stripeToken, payment payment)
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


        [HttpGet]
        public ActionResult PayPal()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult PaymentWithCreditCard()
        {
            //create and item for which you are taking payment
            //if you need to add more items in the list
            //Then you will need to create multiple item objects or use some loop to instantiate object
            Item item = new Item();
            item.name = "Demo Item";
            item.currency = "USD";
            item.price = "5";
            item.quantity = "1";
            item.sku = "sku";

            //Now make a List of Item and add the above item to it
            //you can create as many items as you want and add to this list
            List<Item> itms = new List<Item>();
            itms.Add(item);
            ItemList itemList = new ItemList();
            itemList.items = itms;

            //Address for the payment
            PayPal.Api.Address billingAddress = new Address();
            billingAddress.city = "NewYork";
            billingAddress.country_code = "US";
            billingAddress.line1 = "23rd street kew gardens";
            billingAddress.postal_code = "43210";
            billingAddress.state = "NY";


            //Now Create an object of credit card and add above details to it
            //Please replace your credit card details over here which you got from paypal
            CreditCard crdtCard = new CreditCard();
            crdtCard.billing_address = billingAddress;
            crdtCard.cvv2 = "874";  //card cvv2 number
            crdtCard.expire_month = 1; //card expire date
            crdtCard.expire_year = 2020; //card expire year
            crdtCard.first_name = "Aman";
            crdtCard.last_name = "Thakur";
            crdtCard.number = "1234567890123456"; //enter your credit card number here
            crdtCard.type = "visa"; //credit card type here paypal allows 4 types

            // Specify details of your payment amount.
            Details details = new Details();
            details.shipping = "1";
            details.subtotal = "5";
            details.tax = "1";

            // Specify your total payment amount and assign the details object
            Amount amnt = new Amount();
            amnt.currency = "USD";
            // Total = shipping tax + subtotal.
            amnt.total = "7";
            amnt.details = details;

            // Now make a transaction object and assign the Amount object
            Transaction tran = new Transaction();
            tran.amount = amnt;
            tran.description = "Description about the payment amount.";
            tran.item_list = itemList;
            tran.invoice_number = "your invoice number which you are generating";

            // Now, we have to make a list of transaction and add the transactions object
            // to this list. You can create one or more object as per your requirements

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(tran);

            // Now we need to specify the FundingInstrument of the Payer
            // for credit card payments, set the CreditCard which we made above

            FundingInstrument fundInstrument = new FundingInstrument();
            fundInstrument.credit_card = crdtCard;

            // The Payment creation API requires a list of FundingIntrument

            List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
            fundingInstrumentList.Add(fundInstrument);

            // Now create Payer object and assign the fundinginstrument list to the object
            Payer payr = new Payer();
            payr.funding_instruments = fundingInstrumentList;
            payr.payment_method = "credit_card";

            // finally create the payment object and assign the payer object & transaction list to it
            Payment  pymnt = new Payment();
            pymnt.intent = "sale";
            pymnt.payer = payr;
            pymnt.transactions = transactions;

            try
            {
                //getting context from the paypal
                //basically we are sending the clientID and clientSecret key in this function
                //to the get the context from the paypal API to make the payment
                //for which we have created the object above.

                //Basically, apiContext object has a accesstoken which is sent by the paypal
                //to authenticate the payment to facilitator account.
                //An access token could be an alphanumeric string

                APIContext apiContext = PaypalConfiguration.GetAPIContext(); /*Configuration.GetAPIContext();*/

                //Create is a Payment class function which actually sends the payment details
                //to the paypal API for the payment. The function is passed with the ApiContext
                //which we received above.

                Payment createdPayment = pymnt.Create(apiContext);

                //if the createdPayment.state is "approved" it means the payment was successful else not

                if (createdPayment.state.ToLower() != "approved")
                {
                    return View("FailureView");
                }
            }
            catch (PayPal.PayPalException ex)
            {
                //Logger.Log("Error: " + ex.Message);
                return View("FailureView");
            }

            return View("SuccessView");
        }



        public ActionResult PaymentWithPaypal()
        {
            //getting the apiContext as earlier
            APIContext apiContext = PaypalConfiguration.GetAPIContext(); 

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    //it is returned by the create function call of the payment class

                    // Creating a payment
                    // baseURL is the url on which paypal sendsback the data.
                    // So we have provided URL of this controller only
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                                "/Payment/PaymentWithPayPal?";

                    //guid we are generating for storing the paymentID received in session
                    //after calling the create function and it is used in the payment execution

                    var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment

                    var createdPayment =  this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    //get links returned from paypal in response to Create function call

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section is executed when we have received all the payments parameters

                    // from the previous call to the function Create

                    // Executing a payment

                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.log("Error" + ex.Message);
                return View("FailureView");
            }

            return View("SuccessView");
        }

        private PayPal.Api.Payment payment;

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {

            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };

            itemList.items.Add(new Item()
            {
                name = "Item Name",
                currency = "USD",
                price = "5",
                quantity = "1",
                sku = "sku"
            });

            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            // similar as we did for credit card, do here and create details object
            var details = new Details()
            {
                tax = "1",
                shipping = "1",
                subtotal = "5"
            };

            // similar as we did for credit card, do here and create amount object
            var amount = new Amount()
            {
                currency = "USD",
                total = "7", // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "Transaction description.",
                invoice_number = "your invoice number",
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);
        }

    }

    public static class PaypalConfiguration
    {
        //Variables for storing the clientID and clientSecret key  
        public readonly static string ClientId;
        public readonly static string ClientSecret;
        //Constructor  
        static PaypalConfiguration()
        {
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }
        // getting properties from the web.config  
        public static Dictionary<string, string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }
        private static string GetAccessToken()
        {
            // getting accesstocken from paypal  
            string accessToken = new OAuthTokenCredential("AewWy0vrxJIBSteQ6aKBFEiLmFF3ApH7V9ptF1ALCVDGDnlyLokZxDl4j2qxhFsTIdx-n4zctMCRbf_G", "EN3f4WvoE5z6LN_Cg2QkyLjnwaqQ5WOvr6_F3bPC1wo5wccrWS6CuVxNmk0qLl1tOB5_chsCL0XM1Itg", GetConfig()).GetAccessToken();
            return accessToken;
        }
        public static APIContext GetAPIContext()
        {
            // return apicontext object by invoking it with the accesstoken  
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}