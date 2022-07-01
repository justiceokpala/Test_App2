using System;
using System.Linq;
using System.Threading.Tasks;
using Test_App.Helpers;
using Test_App.Models;
using Test_App.RequestModels;
using Test_App.ResponseModels;
using static Test_App.Repository.CustomerRepo;

namespace Test_App.Repository
{
  public interface ICustomerRepo
  {
    Task<GenericResponseModel> CreateCustomer(CreateRequestModel obj);
    Task<GenericResponseModel> GetAllCustomers();
    Task<GenericResponseModel> VerifyPhoneNumber(verifyOtp otp);
  }

  public class CustomerRepo : ICustomerRepo
  {
    private readonly ApplicationDbContext _db;
    public CustomerRepo(ApplicationDbContext db)
    {
      _db = db;
    }
    public async Task<GenericResponseModel> GetAllCustomers()
    {
      try
      {
        var result = from vh in _db.Customers
                     select new 
                     {
                       vh.Id,
                       vh.Email,
                       vh.FirstName,
                       vh.LastName,
                       vh.StateId,
                       vh.PhoneNumber,
                       vh.PhoneNumberConfirmed
                     };
        if (!result.Any())
        {
          return new GenericResponseModel { StatusCode = 200, StatusMessage = "Successful, No Record Available", };
        }
        return new GenericResponseModel { StatusCode = 200, StatusMessage = "Successful", Data = result.ToList<object>(), };
      }
      catch (Exception)
      {

        throw;
      }
      throw new NotImplementedException();
    }
    public async Task<GenericResponseModel> CreateCustomer(CreateRequestModel obj)
    {
      try
      {
        User userG = _db.Customers.FirstOrDefault(x => x.PhoneNumber == obj.PhoneNumber);
        if (userG != null)
        {
          return new GenericResponseModel { StatusCode = 400, StatusMessage = "This user already exists." };
        }
        if (userG != null && userG.PhoneNumberConfirmed == false )
        {
          return new GenericResponseModel { StatusCode = 400, StatusMessage = "This user already exists. Activate your number." };
        }
        //var g = 
        User model = new()
        {
          Email = obj.Email,
          FirstName = obj.FirstName,
          LastName = obj.LastName,
          StateId = obj.StateId,
          PhoneNumber = obj.PhoneNumber,
          PhoneNumberConfirmed = false,
        };
        if (!string.IsNullOrWhiteSpace(obj.Password))
          model.Password = GenerateHash(obj.Password);

        await _db.Customers.AddAsync(model);
        await _db.SaveChangesAsync();

        MobileNumberConfirmationCodes codes = new MobileNumberConfirmationCodes()
        {
          UserId = model.Id,
          PhoneNumber = model.PhoneNumber,
          Code = randomCodesGenMobile(),
        };

        await _db.MobileNumberConfirmationCodes.AddAsync(codes);
        await _db.SaveChangesAsync();
        return new GenericResponseModel { StatusCode = 200, StatusMessage = "Created Successfully. Kindly verify Phone Number. Otp has been sent" };
      }
      catch (Exception)
      {

        throw;
      }
    }

    public async Task<GenericResponseModel> VerifyPhoneNumber(verifyOtp otp)
    {
      try
      {
        User user = _db.Customers.FirstOrDefault(x => x.PhoneNumber == otp.PhoneNumber);
        if (user.PhoneNumberConfirmed)
        {
          return new GenericResponseModel { StatusCode = 400, StatusMessage = "This user has already been confirmed" };
        }
        MobileNumberConfirmationCodes confirmationCodes = _db.MobileNumberConfirmationCodes.FirstOrDefault(x => x.PhoneNumber == otp.PhoneNumber && x.Code == otp.code);
        if (confirmationCodes != null)
        {
          user.PhoneNumberConfirmed = true;
          _db.Update(user);
          await _db.SaveChangesAsync();

          return new GenericResponseModel { StatusCode = 400, StatusMessage = "Phone Number verified" };
        }
        return new GenericResponseModel { StatusCode = 400, StatusMessage = "This code/Phone number dont match" };
      }
      catch (Exception)
      {

        throw;
      }
    }
    private string GenerateHash(string pin)
    {
      return BCrypt.Net.BCrypt.HashPassword(pin);
    }
    public string randomCodesGenMobile()
    {
      try
      {
        Random rnd = new();
        int codes = rnd.Next(0000, 9000);

        return codes.ToString();
      }
      catch (Exception)
      {
        throw;
      }
    }
    public class verifyOtp
    { 
      public string PhoneNumber { get; set; }
      public string code { get; set; }
    };
  }
}
