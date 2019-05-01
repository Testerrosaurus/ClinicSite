using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
      _userManager = userManager;
      _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult IsLoggedIn()
    {
      return new JsonResult(User.Identity.IsAuthenticated);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<string>> Register(string email, string userName, string password)
    {
      Response.ContentType = "application/json";

      if (ModelState.IsValid)
      {
        IdentityUser user = new IdentityUser { Email = email, UserName = userName };

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
          await _signInManager.SignInAsync(user, false);

          HttpContext.User = await _signInManager.CreateUserPrincipalAsync(user);
        }
        else
        {
          string errors = "";
          foreach (var error in result.Errors)
          {
            errors += error.Description;
          }
          return Ok(errors);
        }
      }

      return Ok("Registered");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<string>> Login(string userName, string password, bool remember)
    {
      Response.ContentType = "application/json";

      if (ModelState.IsValid)
      {
        var result = await _signInManager.PasswordSignInAsync(userName, password, remember, false);
        if (result.Succeeded)
        {
          var user = await _userManager.FindByNameAsync(userName);
          HttpContext.User = await _signInManager.CreateUserPrincipalAsync(user);

          return Ok("Success");
        }
        else
        {
          return Ok("Fail");
        }
      }

      return Ok("ModelState not valid");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<string>> Logout()
    {
      Response.ContentType = "application/json";

      await _signInManager.SignOutAsync();

      HttpContext.User = new ClaimsPrincipal();

      return Ok("Success");
    }
  }
}
