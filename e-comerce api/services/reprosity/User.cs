using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using e_comerce_api.DTO;
using Microsoft.EntityFrameworkCore;

namespace e_comerce_api.services.reprosity
{
    public class User:IUser
    {
        public User(UserManager<applicationuser> userManager,context context) 
        {
            UserManager = userManager;
            Context = context;
        }

        public UserManager<applicationuser> UserManager { get; }
        public context Context { get; }
        
        public async Task<Userdto> getuserbyid(useridandroledto dto)
        {
            var result = await UserManager.FindByIdAsync(dto.UserId);
            if (result==null)
            {
                throw new Exception("user is not found");
            }
            var address = await Context.Addresses.Where(a => a.UserId == result.Id).ToListAsync();
            var userdto = new Userdto
            {
                createdAt = result.createdAt,
                email = result.Email,
                IsActive = result.IsActive,
                lastlogin = result.lastlogin,
                name = result.UserName,
                phone = result.PhoneNumber,
                role = dto.role,
                updatedAt = result.updatedAt,
                userid = result.Id,
                addresses = address.Select(a=>new addressresponsedto
                {
                    AddressName=a.AddressName,
                    City=a.City,
                    PostalCode=a.PostalCode,
                    State = a.State,
                    IsDefault = a.IsDefault,
                    Country = a.Country,
                    StreetAddress=a.StreetAddress,
                }).ToList(),
            };
            return userdto;
        }
        public async Task<UserStatusUpdateDto> updateuserstate(string userid,UserStatusUpdateDto dto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var result = await UserManager.FindByIdAsync(userid.ToString());
                if (result == null)
                {
                    throw new Exception("user is not found");
                }
                result.IsActive = dto.IsActive;
                result.updatedAt=DateTime.Now;
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new UserStatusUpdateDto { IsActive=result.IsActive};
            }
            catch (Exception ex) {
                await transaction.RollbackAsync();

                throw new Exception($"{ex.Message}");
              }
        }
        public async Task<UserStatusUpdateDto> deleteuser(string userid)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var result =await UserManager.FindByIdAsync(userid.ToString());
                if (result == null)
                {
                    throw new Exception("user is not found");
                }
                result.IsActive = false;
                result.updatedAt= DateTime.Now;
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new UserStatusUpdateDto {IsActive=result.IsActive};
            }
            catch (Exception ex) {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }    
        }
        //customers CRUD
        // get all customers   
        public async Task<PagedResult<CustomerResponsedto>> GetAllCustomersAsync(int pageNumber, int pageSize)
        {
            try
            {
                // إجمالي عدد العملاء
                var totalItems = await Context.Customers.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // جلب العملاء مع بيانات المستخدم والعناوين
                var customers = await Context.Customers
                    .AsNoTracking()
                    .Include(c => c.user)
                        .ThenInclude(u => u.addresses)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // تحويل البيانات إلى DTO
                var customerDtos = customers.Select(c => new CustomerResponsedto
                {
                    Customerid = c.Customerid,
                    user_id = c.user_id,
                    lastlogin = c.user.lastlogin,
                    createdAt = c.user.createdAt,
                    updatedAt = c.user.updatedAt,
                    IsActive = c.user.IsActive,
                    name = c.user.UserName,
                    email = c.user.Email,
                    phone = c.user.PhoneNumber,
                    addresses = c.user.addresses.Select(a => new addressresponsedto
                    {
                        StreetAddress = a.StreetAddress,
                        City = a.City,
                        State = a.State,
                        PostalCode = a.PostalCode,
                        Country = a.Country,
                        IsDefault = a.IsDefault,
                        AddressName = a.AddressName
                    }).ToList()
                }).ToList();

                // إرجاع النتيجة على شكل PagedResult
                return new PagedResult<CustomerResponsedto>
                {
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    Data = customerDtos
                };
            }
            catch (Exception ex)
            {
                // ممكن تسجل الخطأ في Log بدلاً من تجاهله
                throw new Exception($"Error retrieving customers: {ex.Message}");
            }
        }
        //get customer by id
        public async Task<CustomerResponsedto> getcustomerbyid(int customerid)
        {
            try
            {
                var c = await Context.Customers.Include(c=>c.user).FirstOrDefaultAsync(c => c.Customerid == customerid);
                if (c == null)
                    throw new Exception("this customer is not found");
                var customerDto = new CustomerResponsedto
                {
                    Customerid = c.Customerid,
                    user_id = c.user_id,
                    lastlogin = c.user.lastlogin,
                    createdAt = c.user.createdAt,
                    updatedAt = c.user.updatedAt,
                    IsActive = c.user.IsActive,
                    name = c.user.UserName,
                    email = c.user.Email,
                    phone = c.user.PhoneNumber,
                    addresses = c.user.addresses.Select(a => new addressresponsedto
                    {
                        StreetAddress = a.StreetAddress,
                        City = a.City,
                        State = a.State,
                        PostalCode = a.PostalCode,
                        Country = a.Country,
                        IsDefault = a.IsDefault,
                        AddressName = a.AddressName
                    }).ToList()
                };
                return customerDto;
            }
            catch(Exception ex) { throw new Exception($"{ex.Message}")  ; }
        }
        //update customer
        public async Task<bool> updatecustomer(customerupdatedto dto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var exitcustomer = await Context.Customers.Include(c=>c.user).ThenInclude(u=>u.addresses).FirstOrDefaultAsync(c=>c.Customerid==dto.customer_id);
                if (exitcustomer == null)
                    throw new Exception("this customer is not found");
                exitcustomer.user.UserName = dto.name;
                exitcustomer.user.PhoneNumber= dto.phone;
                exitcustomer.user.Email= dto.email;
                exitcustomer.user.addresses = dto.addresses.Select(a=>new address
                {
                    AddressName = a.AddressName,
                    IsDefault=a.IsDefault,
                    City = a.City,
                    State=a.State,
                    StreetAddress = a.StreetAddress,
                    PostalCode = a.PostalCode,
                    Country = a.Country,
                }).ToList();
                 Context.Customers.Update(exitcustomer);
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            } catch (Exception ex) { 
                 await transaction.RollbackAsync();
                throw;
            }
        }
       //delete customer
       public async Task<UserStatusUpdateDto> deletecustomer(int customerid)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var exitcustomer = await Context.Customers.Include(c => c.user).FirstOrDefaultAsync(c => c.Customerid == customerid);
                if (exitcustomer == null)
                    throw new Exception("this customer is not found");
                exitcustomer.user.IsActive = false;
                await Context.SaveChangesAsync();
                UserStatusUpdateDto dto = new UserStatusUpdateDto
                {
                    IsActive = exitcustomer.user.IsActive,
                };
                await transaction.CommitAsync();
                return dto;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        //crud seller
        // ✅ Get all sellers with pagination
        public async Task<PagedResult<SellerResponseDto>> GetAllSellersAsync(int pageNumber, int pageSize)
        {
            try
            {
                var totalItems = await Context.Sellers.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var sellers = await Context.Sellers
                    .AsNoTracking()
                    .Include(s => s.user)
                        .ThenInclude(u => u.addresses)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var sellerDtos = sellers.Select(s => new SellerResponseDto
                {
                    Sellerid = s.Sellerid,
                    user_id = s.user_id,
                    name = s.user.UserName,
                    email = s.user.Email,
                    phone = s.user.PhoneNumber,
                    businessName = s.businessName,
                    businessDesription = s.businessDesription,
                    CustomerServicesPhone = s.CustomerServicesPhone,
                    logo = s.logo,
                    rate = s.rate,
                    is_verfied = s.is_verfied,
                    verfiedAt = s.verfiedAt,
                    createdAt = s.user.createdAt,
                    updatedAt = s.user.updatedAt,
                    lastlogin = s.user.lastlogin,
                    IsActive = s.user.IsActive
                }).ToList();

                return new PagedResult<SellerResponseDto>
                {
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    Data = sellerDtos
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sellers: {ex.Message}");
            }
        }

        // ✅ Get seller by ID
        public async Task<SellerResponseDto> GetSellerByIdAsync(int sellerId)
        {
            try
            {
                var s = await Context.Sellers.Include(s => s.user)
                                             .FirstOrDefaultAsync(s => s.Sellerid == sellerId);
                if (s == null)
                    throw new Exception("This seller is not found");

                return new SellerResponseDto
                {
                    Sellerid = s.Sellerid,
                    user_id = s.user_id,
                    name = s.user.UserName,
                    email = s.user.Email,
                    phone = s.user.PhoneNumber,
                    businessName = s.businessName,
                    businessDesription = s.businessDesription,
                    CustomerServicesPhone = s.CustomerServicesPhone,
                    logo = s.logo,
                    rate = s.rate,
                    is_verfied = s.is_verfied,
                    verfiedAt = s.verfiedAt,
                    createdAt = s.user.createdAt,
                    updatedAt = s.user.updatedAt,
                    lastlogin = s.user.lastlogin,
                    IsActive = s.user.IsActive
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // ✅ Update seller
        public async Task<bool> UpdateSellerAsync(SellerUpdateDto dto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var existingSeller = await Context.Sellers
                    .Include(s => s.user)
                    .FirstOrDefaultAsync(s => s.Sellerid == dto.Sellerid);

                if (existingSeller == null)
                    throw new Exception("This seller is not found");

                existingSeller.user.UserName = dto.name;
                existingSeller.user.PhoneNumber = dto.phone;
                existingSeller.user.Email = dto.email;
                existingSeller.businessName = dto.businessName;
                existingSeller.businessDesription = dto.businessDesription;
                existingSeller.CustomerServicesPhone = dto.CustomerServicesPhone;
                existingSeller.logo = dto.logo;

                Context.Sellers.Update(existingSeller);
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ✅ Delete (Deactivate) seller
        public async Task<UserStatusUpdateDto> DeleteSellerAsync(int sellerId)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var existingSeller = await Context.Sellers.Include(s => s.user)
                    .FirstOrDefaultAsync(s => s.Sellerid == sellerId);

                if (existingSeller == null)
                    throw new Exception("This seller is not found");

                existingSeller.user.IsActive = false;
                await Context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new UserStatusUpdateDto { IsActive = existingSeller.user.IsActive };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}


