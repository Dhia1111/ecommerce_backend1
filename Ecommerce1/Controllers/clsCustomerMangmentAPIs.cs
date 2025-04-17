using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Stripe;

using Ecommerce1;
using Microsoft.AspNetCore.Http.HttpResults;
using Stripe.V2;

public class DTOPayment
{

    public List<DTOCartItem>? InCludedProductList { get; set; }

    public string PaymentMethodID { get; set; }

    public DTOPerson? PersonInf { get; set; }

    public DTOPayment()
    {
        this.PaymentMethodID = "";
    }

}


[Route("api/Ecommerce/CustomerMangment")]
[ApiController]


public class clsCustomerMangmentAPIs : ControllerBase
{



    [HttpGet("CartItems")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetCartItems()
    {
        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "UnAtherized"));
            }

            else
            {
                List<DTOCartItem>? Cart = await clsCartItem.GetCart(UserID.Value);

                if (Cart != null)
                    foreach (DTOCartItem item in Cart)
                    {
                        clsProduct? p=await clsProduct.Find(item.ProductID);
                     if(p!=null)   item.ImageURL = clsGlobale.SetImageURL(p.ImageName);

                    }
                return Ok(Cart);
            }

        }
        else
        {
            return BadRequest( new DTOGeneralResponse("You need to log in ",400, "UnAutherized"));
        }
    }

    [HttpGet("GetPerson")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DTOPerson?>> GetPerson()
    {
        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                Response.Cookies.Delete("Authentication");
                return BadRequest(new DTOGeneralResponse("You need to log in ", 400, "UnAutherized"));
            }

            else
            {
                clsUser? user = await clsUser.Find(UserID.Value);
                if (user != null)
                {
                    clsPerson? person = await clsPerson.Find(user.PersonID);

                    if (person != null) { return Ok(person.DTOperson); }

                    else return Ok(null);
                }
                else
                {
                    return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "UnAtherized"));

                }
            }
        }
        else
        {
            return BadRequest(new DTOGeneralResponse("You need to log in ", 400, "UnAutherized"));
        }

    }

    [HttpPost("Payment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]


    public async Task<ActionResult<object>> Payment([FromBody] DTOPayment PaymentInf)

    {
   
        clsUser? User = null;

        Guid? GuidID = Guid.Empty;



        if (PaymentInf == null || PaymentInf.InCludedProductList == null || PaymentInf.PersonInf == null)
        {
            return BadRequest(new DTOGeneralResponse("You need to send a valiad data ,please check your data and try again",400,"null request"));
        }

        if (PaymentInf.InCludedProductList.Count == 0)
        {
            return BadRequest(new DTOGeneralResponse("thier are no products", 400, "empty request"));


        }

        else if (string.IsNullOrEmpty(PaymentInf.PaymentMethodID))
        {
            return BadRequest(new DTOGeneralResponse("null request :make sure to provied the  data", 400, "Athorization and  Validation Error"));

        }

        string? CountryCoude = await clsLocation.GetCountryCode(PaymentInf.PersonInf.Country);
        if (CountryCoude == null)
        {


            return BadRequest(new DTOGeneralResponse("unvalid data request :the Country Name is Uncorect", 400, "Validation Error"));

        }
        if (!await clsValidation.ValidateLocationAsync(PaymentInf.PersonInf.PostCode, PaymentInf.PersonInf.City, CountryCoude))
        {


            return BadRequest(new DTOGeneralResponse("You need to send a valiad data ,please Check the location (post code or location are wrogn", 400, "Validation Error"));

        }

        if (Request.Cookies.TryGetValue("Authentication", out string token1))
        {


            int? UserID = clsGlobale.ExtractUserIdFromToken(token1);

            if (UserID == null)
            {

                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));
            }

            else
            {
                User = await clsUser.Find(UserID.Value);
                if (User == null)
                {
                    return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));


                }

            }
        }
        else
        {
             return BadRequest(new DTOGeneralResponse("you need to log in again", 400, "Validation Error"));


        }

        if (Request.Cookies.TryGetValue("GuidID", out string token2))
        {

            GuidID = clsGlobale.ExtractGuidIDFromToken(token2);

            if (!GuidID.HasValue)
            {




                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred,please log in again", 500, "Athorization abd  Validation Error"));

            }

            else
            {
                clsTransaction? Transaction = await clsTransaction.Find(GuidID.Value);

                if (Transaction != null)
                {
                    await clsGlobale.SendEmail(User, "the transaction is in process please wait", "Processing the transaction", false);

                     return BadRequest(new DTOGeneralResponse("the transaction is in process please wait", 400, "Validation Error"));

                }

            }
        }

        else
        {

            return BadRequest(new DTOGeneralResponse("the transaction is in process please wait", 400, "Validation Error"));

        }


        if (PaymentInf.PersonInf != null)
        {
            User.Person.PostCode = PaymentInf.PersonInf.PostCode;
            User.Person.FirstName = PaymentInf.PersonInf.FirstName;
            User.Person.LastName = PaymentInf.PersonInf.LastName;
            User.Person.City = PaymentInf.PersonInf.City;
            User.Person.Country = PaymentInf.PersonInf.Country;
            User.Person.Phone = PaymentInf.PersonInf.Phone;

            if (!await User.Person.Save())
            {
                 
                return StatusCode(500, new DTOGeneralResponse("Field to save Person Information", 500, "Saving Person inf failed"));

            }

        }




        //Get TotolPrice
        decimal TotolePrice = 0;

        if (PaymentInf.InCludedProductList != null)
        {
            foreach (DTOCartItem Cartitem in PaymentInf.InCludedProductList)
            {
                clsProduct? p = await (clsProduct.Find(Cartitem.ProductID));
                if (p != null)
                {
                    TotolePrice += (p.Price * Cartitem.NumberOfItems);
                }
            }
        }

        //Create The Product List IDs



        clsTransaction NewTransaction = new clsTransaction(PaymentInf.PaymentMethodID, DTOTransaction.enState.Pending, TotolePrice, User.UserID, GuidID.Value.ToString(), PaymentInf.InCludedProductList);



        if (!await NewTransaction.Save())
        {

            await clsGlobale.SendEmail(User, "We Could not Create a new transaction for you please try again later", "Processing the transaction", false);


            return StatusCode(500, new DTOGeneralResponse("the server is experiencing an internal problem wich can't store the transaction pleas try again!!!", 500, "Saving Transaction  failed"));


        }



        string SecrtKey = clsGlobale.GetStripSecret();



        try
        {
            StripeConfiguration.ApiKey = SecrtKey;

            var option = new PaymentIntentCreateOptions
            {
                Amount = (long)(TotolePrice * 100),
                Currency = "usd",
                PaymentMethod = PaymentInf.PaymentMethodID,
                Confirm = true,
                ReceiptEmail = User.Person.Email,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never" // Disables redirect-based payments
                }

            };
            var service = new PaymentIntentService();
            await service.CreateAsync(option);

        }

        catch (StripeException ex)
        {

            clsTransaction? PendingPaymentfailed = await clsTransaction.Find(Guid.Parse(NewTransaction.TransactionGUID.ToString()));

            if (PendingPaymentfailed != null)
            {
                PendingPaymentfailed.State = DTOTransaction.enState.Failed;
                await PendingPaymentfailed.Save();

            }

            if (ex != null)
            {

                //this validation is just for pructic you need to create a make sure to determe if the problem is the user request
                //or the server error
                return StatusCode(500, new DTOGeneralResponse($"Stripe Pervent the payment :{ex.Message} at {ex.Data}", 500, "Outer Service failure"));
                
            }

            else
            {
                return StatusCode(500, new DTOGeneralResponse($"Stripe Pervent the payment  at {DateTime.Now} unknown reason", 500, "Outer Service failure"));

            }
            

        }

        var deleteOptions = new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/" // Make sure to match the path
        };



        //Finsh payment

        clsTransaction? PendingPayment = await clsTransaction.Find(Guid.Parse(NewTransaction.TransactionGUID.ToString()));

        if (PendingPayment != null)
        {
            PendingPayment.State = DTOTransaction.enState.Succeeded;


            await PendingPayment.Save();


        }
        //delete old Cookies 
        Response.Cookies.Delete("GuidID", deleteOptions);
        Response.Cookies.Delete("Authentication", deleteOptions);



        //Create a new Cookies

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,   // Prevent JavaScript access
            Secure = true,     // Only send over HTTPS
            SameSite = SameSiteMode.None, // Prevent CSRF attacks
            Expires = DateTime.UtcNow.AddHours(1) // Set expiration
        };

        var GuidIDToken = clsGlobale.GenerateJwtToken(Guid.NewGuid());
        var AtherizationToken = clsGlobale.GenerateJwtToken(User.DTOUser);


        if (!string.IsNullOrEmpty(GuidIDToken) && !string.IsNullOrEmpty(AtherizationToken))
        {
            Response.Cookies.Append("GuidID", GuidIDToken, cookieOptions);
            Response.Cookies.Append("Authentication", AtherizationToken, cookieOptions);



        }

        else
        {
            await clsGlobale.SendEmail(User, "the Payment Completed secsessfuly but you need to log in again due to server error", "Processing the Payment", false);

            return StatusCode(500, new DTOGeneralResponse("the Payment Completed secsessfuly but you need to log in again due to server error", 500, "Cookie Genrating error"));


        }






        await clsGlobale.SendEmail(User, "the Payment Completed secsessfuly", "Processing the Payment", false);

        return Ok(new DTOGeneralResponse("Payment Complted secsessfuly", 200,"None"));




    }


    
}



