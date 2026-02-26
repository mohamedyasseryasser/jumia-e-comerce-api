using e_comerce_api.DTO.couponsdto;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace e_comerce_api.services.reprosity
{
    public class Copounereprosity:ICopounereprosity
    {
        public Copounereprosity(context context)
        {
            Context = context;
        }

        public context Context { get; }
        public async Task<couponresponsedto> createcopoune(createcouponinputdto dto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                //validate coupon
                if (!await Context.Coupons.AnyAsync(c=>c.code==dto.Code))
                {
                    throw new InvalidExpressionException($"this coupoun code {dto.Code} is already exit");
                }
                 Copons copon = new Copons
                 {
                     code = dto.Code,
                     decription = dto.Description,
                     startAt = dto.StartDate,
                     endAt = dto.EndDate,
                     DiscountType = dto.DiscountType,
                     UsageLimit = dto.UsageLimit,
                     MinimumPurchase = dto.MinimumPurchase,
                     isActive = dto.IsActive ?? true,
                     DiscountPercentage=dto.DiscountAmount,
                 };
                copon.UsageCount = 0;
                await Context.Coupons.AddAsync(copon);
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mappingcoupon(copon);

            }
            catch (Exception ex) 
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<couponresponsedto> updatecoupone(updatecouponedto couponDto)
        {
            using var transaction =await Context.Database.BeginTransactionAsync();

            try
            {
                var coupon=await Context.Coupons.FirstOrDefaultAsync(c=>c.CoponId==couponDto.coupounid);
                if (coupon == null) {
                    throw new Exception("this coupone is not exit");
                }
                if (!string.IsNullOrWhiteSpace(couponDto.Description))
                {
                    coupon.decription = couponDto.Description;
                }

                if (couponDto.DiscountAmount.HasValue)
                {
                    coupon.DiscountPercentage = couponDto.DiscountAmount.Value;
                }

                if (couponDto.MinimumPurchase.HasValue)
                {
                    coupon.MinimumPurchase = couponDto.MinimumPurchase.Value;
                }

                if (!string.IsNullOrWhiteSpace(couponDto.DiscountType))
                {
                    coupon.DiscountType = couponDto.DiscountType;
                }

                if (couponDto.StartDate.HasValue)
                {
                    coupon.startAt = couponDto.StartDate.Value;
                }

                if (couponDto.EndDate.HasValue)
                {
                    coupon.endAt = couponDto.EndDate.Value;
                }

                if (couponDto.IsActive.HasValue)
                {
                    coupon.isActive = couponDto.IsActive.Value;
                }

                if (couponDto.UsageLimit.HasValue)
                {
                    coupon.UsageLimit = couponDto.UsageLimit.Value;
                }
                await Context.SaveChangesAsync();
                await transaction.CommitAsync( );
                return mappingcoupon(coupon);
            }
            catch (Exception ex) 
            {
                await transaction.RollbackAsync( );
                throw ex;
            }
        }
        public async Task<bool> deletecoupone(int id)
        {
            using var transaction =await Context.Database.BeginTransactionAsync();
            try
            {
                var coupon = await Context.Coupons.Include(c=>c.users).FirstOrDefaultAsync(c=>c.CoponId==id);
                if (coupon == null)
                {
                    throw new Exception($"this id is not found:{id}");
                }
                var orderscoupon = await Context.Orders.AnyAsync(o=>o.CouponId==id);
                if (orderscoupon)
                {
                    coupon.isActive=false;
                }
                else
                {
                    Context.RemoveRange(coupon.users);
                    Context.Remove(coupon);
                }
                await Context.SaveChangesAsync();
                await transaction.CommitAsync( );
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<int> getcouponscount(bool? isactive=null,string? searchterm=null)
        {
            try
            {
                IQueryable<Copons> query = Context.Coupons;

                // Apply filters
                if (isactive.HasValue)
                {
                    query = query.Where(c => c.isActive == isactive.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchterm))
                {
                    searchterm = searchterm.ToLower();
                    query = query.Where(c =>
                        c.code.ToLower().Contains(searchterm) ||
                        (c.decription != null && c.decription.ToLower().Contains(searchterm)));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                throw ;
            }
        }
        public async Task<couponresponsedto> GetCouponByIdAsync(int couponId)
        {
            try
            {
                var coupon = await Context.Coupons
                    .FirstOrDefaultAsync(c => c.CoponId == couponId);

                if (coupon == null)
                    return null;

                return  mappingcoupon(coupon);
            }
            catch (Exception ex)
            {
                 throw;
            }
        }

        public async Task<couponresponsedto> GetCouponByCodeAsync(string code)
        {
            try
            {
                var coupon = await Context.Coupons
                    .FirstOrDefaultAsync(c => c.code == code);

                if (coupon == null)
                    return null;

                return mappingcoupon(coupon);
            }
            catch (Exception ex)
            {
                 throw;
            }
        }
        public couponresponsedto mappingcoupon(Copons copons)
        {
            try
            {
                if (copons == null)
                    return null;

                var response = new couponresponsedto
                {
                    CoponId = copons.CoponId,
                    isActive = copons.isActive,
                    DiscountPercentage = copons.DiscountPercentage,
                    code = copons.code,
                    decription = copons.decription,
                    startAt = copons.startAt,
                    endAt = copons.endAt,
                    DiscountType = copons.DiscountType,
                    UsageLimit = copons.UsageLimit,
                    UsageCount = copons.UsageCount,
                    MinimumPurchase = copons.MinimumPurchase,

                    usercount = copons.users?.Count ?? 0,
                    ordercount = copons.orders?.Count ?? 0,

                    UserCoupons = copons.users?.Select(u => new usercouponinputdto
                    {
                        customer_id = u.customer_id,
                        AssignedAt = u.AssignedAt,
                        UsedAt = u.UsedAt
                    }).ToList() ?? new List<usercouponinputdto>(),

                    orderresponsedtos = copons.orders?.Select(o => new ordercouponresponsedto
                    {
                        orderid = o.Orderid
                    }).ToList() ?? new List<ordercouponresponsedto>()
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while mapping coupon", ex);
            }
        }

    }
    
}
