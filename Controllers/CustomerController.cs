using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Test_App.Helpers;
using Test_App.Repository;
using Test_App.RequestModels;
using static Test_App.Repository.CustomerRepo;

namespace Test_App.Controllers
{
  [Route("/[controller]")]
  [ApiController]

  
  public class CustomerController : ControllerBase
  {
    private readonly ICustomerRepo _customerRepo;
    private readonly ApplicationDbContext _db;
    public CustomerController(ICustomerRepo customerRepo, ApplicationDbContext db)
    {
      _db = db;
      _customerRepo = customerRepo;
    }

    [HttpGet("getAllCustomers")]
    public async Task<IActionResult> GetAllCustomers()
    {
      if (!ModelState.IsValid)
      {
        return BadRequest();
      }
      var result = await _customerRepo.GetAllCustomers();

      return Ok(result);
    }

    [HttpPost("createCustomer")]
    public async Task<IActionResult> CreateCustomer(CreateRequestModel obj)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest();
      }
      var result = await _customerRepo.CreateCustomer(obj);

      return Ok(result);
    }

    [HttpPost("verifyPhoneNumber")]
    public async Task<IActionResult> verifyPhoneNumber(verifyOtp obj)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest();
      }
      var result = await _customerRepo.VerifyPhoneNumber(obj);

      return Ok(result);
    }
  }
}
