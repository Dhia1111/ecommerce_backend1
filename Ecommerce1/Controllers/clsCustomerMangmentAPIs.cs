﻿using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Stripe;

using Ecommerce1;
using Microsoft.AspNetCore.Http.HttpResults;

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
                return StatusCode(500, "An unexpected server error occurred.");
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
            return BadRequest("You need to log in ");
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
                return BadRequest();
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
                    return StatusCode(500);

                }
            }
        }
        else
        {
            return BadRequest();
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


        if (PaymentInf == null ||  PaymentInf.InCludedProductList == null||PaymentInf.InCludedProductList.Count==0)
        {
            return BadRequest(new { status = "Not Set", message = "null requests ", ErrorType = "null requests" });

        }

        else if (string.IsNullOrEmpty(PaymentInf.PaymentMethodID))
        {
            BadRequest(new { status = "Not Set", message = "null request :make sure to provied the  data ", ErrorType = "Athorization and  Validation Error" });
        }

        if (Request.Cookies.TryGetValue("Authentication", out string token1))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token1);

            if (UserID == null)
            {
                return StatusCode(500, new { status = "Not Set", message = "An unexpected server error occurred.", ErrorType = "null requests" });
            }

            else
            {
                User = await clsUser.Find(UserID.Value);
                if (User == null)
                {
                    return StatusCode(500, new { status = "Not Set", message = "An unexpected server error occurred.", ErrorType = "null requests" });


                }

            }
        }
        else
        {
            return BadRequest(new { status = "Not Set", message = "you need to log in again", ErrorType = "null requests" });

        }

        if (Request.Cookies.TryGetValue("GuidID", out string token2))
        {

            GuidID = clsGlobale.ExtractGuidIDFromToken(token2);

            if (!GuidID.HasValue)
            {


                return StatusCode(500, new { status = "Not Set", message = "\"An unexpected server error occurred. please log in again ", ErrorType = "Athorization abd  Validation Error" });



            }

            else
            {
                clsTransaction? Transaction = await clsTransaction.Find(GuidID.Value);

                if (Transaction != null)
                {
                 await   clsGlobale.SendEmail(User, "the transaction is in process please wait", "Processing the transaction", false);
                
                 return BadRequest(new { status = "not set", message = "\"the transaction is in process please wait ", ErrorType = "double pay" });

                }

            }
        }

        else
        {
            return BadRequest(new { status = "Not Set", message = "you need to log in again", ErrorType = "null requests" });
        }

        //Update PersonInf

        if (PaymentInf.PersonInf != null) { 
            User.Person.PostCode = PaymentInf.PersonInf.PostCode;   
            User.Person.FirstName = PaymentInf.PersonInf.FirstName;
            User.Person.LastName = PaymentInf.PersonInf.LastName;
            User.Person.City = PaymentInf.PersonInf.City;
            User.Person.Country = PaymentInf.PersonInf.Country;
            User.Person.Phone = PaymentInf.PersonInf.Phone;
            if (!await User.Person.Save())
            {
                return StatusCode(500,new { status = "Not Set", message = "Field to save Person Information", ErrorType = "filed" });
            }

        }

        //Load the Cart For User
        bool UpdatingCart = false;
        foreach (DTOCartItem item in PaymentInf.InCludedProductList)
        {
            UpdatingCart = await User.UpdateToCart(item);
            if (!UpdatingCart)
            {
                await clsGlobale.SendEmail(User, "We Could not handle the Cart Pleas try again later", "Processing the transaction", false);
                return StatusCode(500, new { status = "Not Set", message = "Handling Save Error", ErrorType = "null requests" });
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

                        TotolePrice += p.Price;
                    }
                }
            }
        //Create The Product List IDs

        List<int> ProductListIDs = new List<int>();

           foreach(DTOCartItem CartItem in PaymentInf.InCludedProductList){
            ProductListIDs.Add(CartItem.ProductID);

        }

            clsTransaction NewTransaction = new clsTransaction(PaymentInf.PaymentMethodID, DTOTransaction.enState.Pending, TotolePrice, User.UserID, GuidID.Value.ToString(),ProductListIDs);




            if (!await NewTransaction.Save())
            {

            await clsGlobale.SendEmail(User, "We Could not Create a new transaction for you please try again later", "Processing the transaction", false);

            return StatusCode(500, new { status = "Not Set", message = "\"the server is experiencing an internal problem wich can't store the transaction pleas try again!!!", ErrorType = "Saving error" });


            }



            string SecrtKey = clsGlobale.GetStripSecret();



            try
            {
                StripeConfiguration.ApiKey = SecrtKey;

                var option = new PaymentIntentCreateOptions
                {
                    Amount = long.Parse((TotolePrice * 100).ToString()),
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
            
            var PaymentIntent = await service.CreateAsync(option);
             
            Response.Cookies.Delete("GuidID");

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,   // Prevent JavaScript access
                    Secure = true,     // Only send over HTTPS
                    SameSite = SameSiteMode.Strict, // Prevent CSRF attacks
                    Expires = DateTime.UtcNow.AddHours(1) // Set expiration
                };

                var GuidIDToken = clsGlobale.GenerateJwtToken(Guid.NewGuid());
                if (!string.IsNullOrEmpty(GuidIDToken))
                {
                    Response.Cookies.Append("GuidID", GuidIDToken, cookieOptions);


                }
                else
                {
                await clsGlobale.SendEmail(User, "the Payment Completed secsessfuly but you need to log in again due to server error", "Processing the Payment", false);

                return StatusCode(500, new { stuts = "Not set", message = "the Payment Completed secsessfuly but you need to log in again due to server error", ErrorType = "Cookie Genrating error" });

                }
                            
             //Finsh payment

            clsTransaction? PendingPayment = await clsTransaction.Find(Guid.Parse(NewTransaction.TransactionGUID.ToString()));

            if (PendingPayment != null)
            {
                PendingPayment.State = DTOTransaction.enState.Succeeded;

                //ClearPendingPaymentCart 
                if(!await User.ClearCart())
                {
                    await clsGlobale.SendEmail(User, "the payment is Complited secsessfuly and the product is in order ,but do not purshs any thing and log out then log in antile the Cart is Clear", "Processing the Payment", false);

                    return StatusCode(500,new { stuts = "", message = "the payment is Complited secsessfuly and the product is in order ,but do not purshs any thing and log out then log in antile the Cart is Clear", ErrorType = "Connection cut or  overLoaded server" });

                }
                await PendingPayment.Save();

            }


            //Send OrderInformation to your seplayer 

            //and Email to the Client

            await clsGlobale.SendEmail(User, "the Payment Completed secsessfuly", "Processing the Payment", false);

            return Ok(new { stuts = "Ok", message = "the Payment Completed secsessfuly", ErrorType = "" });
            }

            catch (StripeException ex)
        {
            clsTransaction? PendingPayment = await clsTransaction.Find(Guid.Parse(NewTransaction.TransactionGUID.ToString()));

            if (PendingPayment != null)
                {
                    PendingPayment.State = DTOTransaction.enState.Failed;
                    await PendingPayment.Save();

                }
                if (ex.StripeError != null)
                {

                    //this validation is just for pructic you need to create a make sure to determe if the problem is the user request
                    //or the server error
                    return StatusCode(500,new { stuts = "failed", message = ex.Message, ErrorType = ex.StripeError.Type });
                }
            
                else
                {
                    return StatusCode(500,new { stuts = "failed", message = ex.Message, ErrorType = "UnKnown" });


                }

            }




        }

    }



