using e_comerce_api.DTO;
using e_comerce_api.DTO.cartdto;
using e_comerce_api.Enum;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace e_comerce_api.services.reprosity
{
    public class cartreprosity:Icartreprosity
    {
        public cartreprosity(context context) {
            Context = context;
        }
        public context Context { get; }
        //get cart by customer id
        public async Task<cartdto> getcartbycustomerid(int customerid,bool isincludeitems=true)
        {
            //verfy exit customer
            var exitcustomer = await Context.Customers.AsNoTracking().FirstOrDefaultAsync(c=>c.Customerid==customerid);
            if (exitcustomer == null)
            {
                throw new Exception("this customer is not found");
            }
            Cart exitcart = null;
            if (isincludeitems!=true)
            {
                exitcart = await Context.Carts.AsNoTracking().FirstOrDefaultAsync(c=>c.customer_id==customerid);
            }
            else 
            {
                exitcart = await Context.Carts
            .Where(c => c.customer_id == customerid )
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.productVariants)  
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Variant) 
            .FirstOrDefaultAsync();
            }
            if (exitcart==null)
            {
                throw new Exception("this cart is not found");
            }
            var response = new cartdto { 
             Cartid=exitcart.Cartid,
             createdAt=exitcart.createdAt,
             updatedAt=exitcart.updatedAt,
             customerid=exitcustomer.Customerid,
            statue=exitcart.statue,
            totalAmount=exitcart.totalAmount,
            };
            foreach (var item in exitcart.CartItems)
            {
                var cartitem = new CartItemDto
                {
                    CartId = exitcart.Cartid,
                    CartItemId = item.CartItemsId,
                    PriceAtAddition = item.PriceAtAddition,
                    Quantity = item.Quantity,
                    ProductId = item.Product.ProductId,
                    VariantId = item.VariantId,
                    // ProductImage=item.Product.Mainimageurl,
                    //   ProductName=item.Product.Name,
                    // VariantName=item.Variant.VariantName
                };
                response.cartItemDtos.Add(cartitem);
            }
            if (isincludeitems == true&&exitcart.CartItems!=null)
            {
                foreach (var item in exitcart.CartItems)
                {
                    var cartItemdto = response.cartItemDtos.FirstOrDefault(c=>c.CartItemId==item.CartItemsId);
                    cartItemdto.ProductImage = item.Product.Mainimageurl;
                    cartItemdto.ProductName = item.Product.Name;
                    if (item.VariantId.HasValue&&item.Variant!=null)
                    {
                        cartItemdto.ProductName = item.Variant.VariantName;
                        cartItemdto.ProductImage=item.Variant.Mainimageurl;
                    }
                }
                    
            }  
            
            return response;
        }
        //get cartitem by id
        public async Task<CartItemDto> getcartitembyid(int cartitemid)
        {
            try
            {
                var exitcartitem = await Context.CartItems
                    .Include(c=>c.Product).Include(c=>c.Variant)
                    .FirstOrDefaultAsync(c=>c.CartItemsId==cartitemid);
                if (exitcartitem == null)
                {
                    throw new Exception("this cartitem is not found");
                }
                var response = new CartItemDto { 
                 CartId = exitcartitem.CartId.Value,
                 CartItemId=exitcartitem.CartItemsId
                 ,PriceAtAddition=exitcartitem.PriceAtAddition,
                 ProductId=exitcartitem.Product.ProductId,
                 Quantity=exitcartitem.Quantity,
                 VariantId=exitcartitem.Variant.ProductVariantId
                };
                response.ProductName = exitcartitem.Product.Name;
                response.ProductImage=exitcartitem.Product.Mainimageurl;
                if (exitcartitem.VariantId.HasValue&&exitcartitem.Variant!=null)
                {
                    response.ProductName = exitcartitem.Variant.VariantName;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //add cartitems
        public async Task<CartItemDto> addcartitems(string userid, cartiteminputdto dto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                //verfication from userid
                var customerid = getcustomerid(userid);
                if (customerid == 0)
                {
                    throw new Exception("not found");
                }
                //if exitcart or not
                var cart = await Context.Carts.Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.customer_id == customerid && c.statue == cartstatue.Open);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        createdAt = DateTime.Now,
                        statue = cartstatue.Open,
                        customer_id = customerid,
                        updatedAt = DateTime.Now,
                    };
                    await Context.Carts.AddAsync(cart);
                    await Context.SaveChangesAsync();
                }

                //exit product 
                var product = await Context.Products.Include(p => p.productVariants)
                    .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);
                if (product == null)
                {
                    throw new Exception("this product is not found");
                }
                //verfy variant
                productVariant variant = null;
                if (dto.VariantId.HasValue)
                {
                     variant = product.productVariants.FirstOrDefault(v => v.ProductVariantId == dto.VariantId);
                    if (variant == null)
                    {
                        throw new Exception("this variant is not found");
                    }
                    //verfy variant quantity
                    if (variant.stockquantity < dto.Quantity)
                    {
                        throw new Exception("stockquantity is not enougth");
                    }
                }
                else
                {
                    if (product.stockquantity<dto.Quantity)
                    {
                        throw new Exception("stockquantity is not enougth");

                    }
                }



                //if item is already exit
                CartItems newcartitems = null;
                var exitcartitem = cart.CartItems.FirstOrDefault(c =>
                c.VariantId == variant.ProductVariantId &&
                c.ProductId == dto.ProductId
                );
                if (exitcartitem!=null)
                {
                    int totalquantity=exitcartitem.Quantity+dto.Quantity;
                    if (dto.VariantId.HasValue)
                    {
                        if (variant.stockquantity<totalquantity)
                        {
                            throw new Exception("stockquantity is not enougth");
                        }
                    }
                    else
                    {
                        if (product.stockquantity<totalquantity)
                        {
                            throw new Exception("stockquantity is not enougth");
                        }
                    }
                    exitcartitem.Quantity = totalquantity;
                    newcartitems = exitcartitem;
                }
                else
                {
                 decimal pricetouse=variant!=null?
                        variant.basicprice - (variant.basicprice * (variant.discountpercentage ) / 100m)
                        : product.basicprice - (product.basicprice * (product.discountpercentage ) / 100m);
                    newcartitems = new CartItems
                    {
                        CartId = cart.Cartid,
                        ProductId = product.ProductId,
                        Quantity = dto.Quantity,
                        VariantId = variant.ProductVariantId,
                        PriceAtAddition = pricetouse,
                    };
                    await Context.CartItems.AddAsync(newcartitems);
                }
                   cart.updatedAt = DateTime.Now;
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                var response = new CartItemDto();
                response.ProductName = product.Name;
                response.ProductImage = product.Mainimageurl;
                if (newcartitems.VariantId.HasValue)
                {
                    response.ProductName = variant.VariantName;
                    response.ProductImage=variant.Mainimageurl;
                }
                response.CartId = cart.Cartid;  
                response.Quantity = dto.Quantity;
                response.CartItemId=newcartitems.CartItemsId;
                response.PriceAtAddition = newcartitems.PriceAtAddition;
                return response;
            }
            catch (Exception ex)
            { 
            await transaction.RollbackAsync();
                throw;
            }
            }
        //update cartitem quantity 
        public async Task<CartItemDto> updatecartitemquantity(string userid,updatecartitem dto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                var customerid = getcustomerid(userid);
                if (customerid==0)
                {
                    throw new Exception("this customer is not found");
                }
                var cartitem = await Context.CartItems.
                    Include(c=>c.Cart)
                    .Include(c=>c.ProductId)
                    .Include(c=>c.Variant).
                    FirstOrDefaultAsync(c=>c.CartItemsId==dto.cartitem&&c.Cart.statue==cartstatue.Open);
                if (cartitem == null)
                {
                    throw new Exception("this cartitem is not found");
                }
                if(cartitem.Cart.customer_id != customerid)
                {
                    throw new Exception("this cartitem with not this customer");
                }
                if (cartitem.VariantId.HasValue&&cartitem.Variant!=null)
                {
                    if (cartitem.Variant.stockquantity<dto.newquantity)
                    {
                        throw new Exception("this quantity is not enougth");
                    }
                }
                else
                {
                    if (cartitem.Product.stockquantity < dto.newquantity)
                    {
                        throw new Exception("this quantity is not enougth");
                    }
                }
                cartitem.Quantity=dto.newquantity;
                cartitem.Cart.updatedAt=DateTime.Now;
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new CartItemDto
                {
                    CartId = cartitem.CartId.Value,
                    CartItemId = cartitem.CartItemsId
                ,
                    PriceAtAddition = cartitem.PriceAtAddition,
                    ProductId = cartitem.Product.ProductId,
                    Quantity = cartitem.Quantity,
                    VariantId = cartitem.Variant.ProductVariantId
                };
                response.ProductName = cartitem.Product.Name;
                response.ProductImage = cartitem.Product.Mainimageurl;
                if (cartitem.VariantId.HasValue && cartitem.Variant != null)
                {
                    response.ProductName = cartitem.Variant.VariantName;
                }
                return response;
            }
            catch (Exception ex) 
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        //remove cartitem
        public async Task<bool> removecartitem(string userid, int cartitemid)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                var customerid = getcustomerid(userid);
                if (customerid == 0)
                {
                    throw new Exception("this customer is not found");
                }
                var cartitem =await Context.CartItems.
                   Include(c=>c.Cart).
                    FirstOrDefaultAsync(c => c.CartItemsId == cartitemid && c.Cart.statue == cartstatue.Open);
                if (cartitem == null)
                {
                    return false;
                }
                if (cartitem.Cart.customer_id != customerid)
                {
                    return false;
                }
                Context.CartItems.Remove(cartitem);
                cartitem.Cart.updatedAt = DateTime.UtcNow;

                await Context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex) { 
              await transaction.RollbackAsync();
                throw;
            }
            }
        //clear cart
        public async Task<bool> clearcart(string userid)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var customerid = getcustomerid(userid);
                if (customerid == 0)
                {
                    throw new Exception("this user is not found");
                }
                var cart = await Context.Carts.Include(c => c.CartItems).
                    FirstOrDefaultAsync(c =>
                c.customer_id == customerid &&
                c.statue == cartstatue.Open);

                if (cart == null||cart.CartItems==null) { return true; }


                Context.CartItems.RemoveRange(cart.CartItems);

                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
            }
        //product exit in cart or no
        public async Task<bool> productexitincart(string userid,int productid,int? variantid=null)
        {
            try
            {
                var customerid = getcustomerid(userid);
                if (customerid == 0)
                {
                    throw new Exception("this user is not found");
                }
                var cart = await Context.Carts.Include(c => c.CartItems).
                    FirstOrDefaultAsync(c =>
                c.customer_id == customerid &&
                c.statue == cartstatue.Open);

                if (cart == null || cart.CartItems == null) { return false; }
                return cart.CartItems.Any(c => c.ProductId == productid && c.VariantId == variantid);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<bool> CartItemExistsAndBelongsToCustomerAsync(string userId, int cartItemId)
        {
            try
            {

                var customerId = getcustomerid(userId);
                if (customerId == 0) throw new KeyNotFoundException("Customer was not found");
                return await Context.CartItems
                    .Include(ci => ci.Cart)
                    .AnyAsync(ci => ci.CartItemsId == cartItemId && ci.Cart.customer_id == customerId);
            }
            catch (Exception ex)
            {
                 throw;
            }
        }
        public async Task<int> GetCartItemsCountAsync(int customerId)
        {
            try
            {
                // Get cart
                var cart = await Context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.customer_id == customerId);

                if (cart == null || cart.CartItems == null)
                    return 0;

                return cart.CartItems.Count;
            }
            catch (Exception ex)
            {
                 throw;
            }
        }
        //save in wishlist
        public async Task saveiteminwishlist(int customerid,int cartitemid)
        {
            var exitcartitem = await Context.CartItems.
                Include(c=>c.Cart).
                Include(c=>c.Product)
                .FirstOrDefaultAsync(c=>c.CartItemsId==cartitemid);
           
            if (exitcartitem == null)
                throw new KeyNotFoundException($"Cart item with ID {cartitemid} not found");

            if (exitcartitem.Cart.customer_id!=customerid)
            {
                throw new UnauthorizedAccessException("Cart item does not belong to this customer");
            }

            var wishlist = await Context.Wishlists.FirstOrDefaultAsync(w=>w.CustomerId==customerid);
            if(wishlist == null)
            {
                wishlist = new WishList { 
                 CreatedAt=DateTime.Now,
                 CustomerId=customerid,
                };
                await Context.Wishlists.AddAsync(wishlist);
                await Context.SaveChangesAsync();
                WithItems withItems = new WithItems { 
                 AddedAt=DateTime.Now,
                 ProductId=exitcartitem.Product.ProductId,
                 Product=exitcartitem.Product,
                 WishlistId=wishlist.WishlistId,
                };
                await Context.SaveChangesAsync();
            }
            else
            {

            }
        }
        //get customerid
        public int getcustomerid(string userid)
        {
           return  Context.Customers.FirstOrDefault(c=>c.user_id==userid)?.Customerid??0;
        }
    }
}
