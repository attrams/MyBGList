using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyBGList.DTO;
using MyBGList.Models;

namespace MyBGList.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    public readonly ApplicationDbContext _context;
    public readonly ILogger<DomainsController> _logger;
    public readonly IConfiguration _configuration;
    public readonly UserManager<ApiUser> _userManager;
    public readonly SignInManager<ApiUser> _signInManager;


    public AccountController(
        ApplicationDbContext context,
        ILogger<DomainsController> logger,
        IConfiguration configuration,
        UserManager<ApiUser> userManager,
        SignInManager<ApiUser> signInManager
    )
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost]
    [ResponseCache(CacheProfileName = "NoCache")]
    public async Task<ActionResult> Register(RegisterDTO registerDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var newUser = new ApiUser();
                newUser.UserName = registerDTO.UserName;
                newUser.Email = registerDTO.Email;
                var result = await _userManager.CreateAsync(newUser, registerDTO.Password!);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {userName} ({email}) has been created.", newUser.UserName, newUser.Email);

                    return StatusCode(StatusCodes.Status201Created, $"User '{newUser.UserName}' has been created.");
                }
                else
                {
                    throw new Exception(string.Format("Error: {0}", string.Join(" ", result.Errors.Select(error => error.Description))));
                }
            }
            else
            {
                var details = new ValidationProblemDetails(ModelState);
                details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                details.Status = StatusCodes.Status400BadRequest;

                return new BadRequestObjectResult(details);
            }
        }
        catch (System.Exception e)
        {
            var exceptionDetails = new ProblemDetails();
            exceptionDetails.Detail = e.Message;
            exceptionDetails.Status = StatusCodes.Status500InternalServerError;
            exceptionDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";

            return StatusCode(StatusCodes.Status500InternalServerError, exceptionDetails);
        }
    }

    [HttpPost]
    [ResponseCache(CacheProfileName = "NoCache")]
    public async Task<ActionResult> Login(LoginDTO loginDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(loginDTO.UserName!);

                if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password!))
                {
                    throw new Exception("Invalid login attempt.");
                }
                else
                {
                    var signingCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]!)),
                        SecurityAlgorithms.HmacSha256
                    );

                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, user.UserName!));

                    var jwtObject = new JwtSecurityToken(
                        issuer: _configuration["JWT:Issuer"],
                        audience: _configuration["JWT:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddSeconds(300),
                        signingCredentials: signingCredentials
                    );

                    var jwtString = new JwtSecurityTokenHandler().WriteToken(jwtObject);

                    return StatusCode(StatusCodes.Status200OK, jwtString);
                }
            }
            else
            {
                var details = new ValidationProblemDetails(ModelState);
                details.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                details.Status = StatusCodes.Status400BadRequest;

                return new BadRequestObjectResult(details);
            }
        }
        catch (System.Exception e)
        {
            var exceptionDetails = new ProblemDetails();
            exceptionDetails.Detail = e.Message;
            exceptionDetails.Status = StatusCodes.Status401Unauthorized;
            exceptionDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

            return StatusCode(StatusCodes.Status401Unauthorized, exceptionDetails);
        }
    }
}