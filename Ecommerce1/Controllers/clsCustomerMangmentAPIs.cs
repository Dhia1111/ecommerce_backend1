using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Ecommerce1;


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
                        item.ImageURL = clsGlobale.SetImageURL(item.Product.ImageName);

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
                if (user  != null)
                {
                    clsPerson? person =  await clsPerson.Find(user.PersonID);

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

}

