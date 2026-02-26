using e_comerce_api.constant;
using e_comerce_api.DTO;
using e_comerce_api.DTO.orderdto;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace e_comerce_api.services.reprosity
{
    public class orderreprosity : Iorderreprosity
    {
        public orderreprosity(context Context, Icartreprosity _icartreprosity)
        {
            this.Context = Context;
            Icartreprosity = _icartreprosity;
        }
        public context Context { get; }
        public Icartreprosity Icartreprosity { get; }

        //create order
        public async Task<orderresponsedto> createorder(createorderdto orderDto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                // Validate and calculate order
                var (isValid, errorMessage, calculatedOrder) = await ValidateAndCalculateOrderAsync(orderDto);

                if (!isValid)
                    throw new InvalidOperationException(errorMessage);

                // Use the calculated order instead of the input
                orderDto = calculatedOrder;
                var order = new order {
                    customer_id = orderDto.CustomerId,
                    address_id = orderDto.AddressId,
                    CouponId = orderDto.CouponId,
                    TotalAmount = orderDto.TotalAmount,
                    DiscountAmount = orderDto.DiscountAmount,
                    ShippingFee = orderDto.ShippingFee,
                    TaxAmount = orderDto.TaxAmount,
                    FinalAmount = orderDto.FinalAmount,
                    PaymentMethod = orderDto.PaymentMethod,
                    PaymentStatus = PaymentStatus.Pending, // Default status
                    AffiliateId = orderDto.AffiliateId,
                    AffiliateCode = orderDto.AffiliateCode,
                    statue = OrderStatus.Pending,
                    createdAt = DateTime.UtcNow,
                    updatedAt = DateTime.UtcNow
                };
                Context.Orders.Add(order);
                await Context.SaveChangesAsync();
                //create sub orders
                foreach (var suborder in orderDto.SubOrders)
                {
                    var subOrder = new SubOrder
                    {
                        OrderId = order.Orderid,
                        seller_id = suborder.seller_id,
                        Subtotal = suborder.Subtotal,
                        Status = SubOrderStatus.Pending, // Default status
                        StatusUpdatedAt = DateTime.UtcNow
                       ,
                        TrackingNumber = "1",
                        ShippingProvider = "ali"
                    };

                    await Context.SubOrders.AddAsync(subOrder);
                    await Context.SaveChangesAsync();
                    // Add order items for this sub-order
                    foreach (var orderItemDto in orderDto.Items)
                    {
                        var orderItem = new OrderItems
                        {
                            sub_order_id = subOrder.SubOrderId,
                            product_id = orderItemDto.ProductId,
                            quantity = orderItemDto.Quantity,
                            PriceAtPurchase = orderItemDto.PriceAtPurchase,
                            TotalPrice = orderItemDto.TotalPrice,
                            variant_id = orderItemDto.VariantId
                        };

                        await Context.OrderItems.AddAsync(orderItem);
                    }
                }

                await Context.SaveChangesAsync();
                updateinventory(orderDto);
               var userid = await Context.Customers.FirstOrDefaultAsync(c => c.Customerid == orderDto.CustomerId);
                await transaction.CommitAsync();
                await Icartreprosity.clearcart(userid.user_id);

                //maping order
                var response = await GetOrderByIdAsync(order.Orderid);
                if (response == null)
                {
                    throw new Exception("there is error in response creation");
                }

                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        /// <summary>
        /// Validates and calculates order totals before creating or updating an order
        /// </summary>
        private async Task<(bool IsValid, string ErrorMessage, createorderdto CalculatedOrder)> ValidateAndCalculateOrderAsync(createorderdto orderDto)
        {
            try
            {
                var calculatedorder = new createorderdto
                {
                    CustomerId = orderDto.CustomerId,
                    AddressId = orderDto.AddressId,
                    CouponId = orderDto.CouponId,
                    PaymentMethod = orderDto.PaymentMethod,
                    AffiliateId = orderDto.AffiliateId,
                    AffiliateCode = orderDto.AffiliateCode,
                    Items = orderDto.Items,
                    SubOrders = new List<createsuborderdto>()
                };

                //verfiy customer id
                var customer = await Context.Customers.FirstOrDefaultAsync(c => c.Customerid == orderDto.CustomerId);
                if (customer == null)
                    return (false, $"Customer with ID {orderDto.CustomerId} not found", null);
                //verfiy address id
                var address = await Context.Addresses.FirstOrDefaultAsync(a => a.AddressId == orderDto.AddressId
                );//&& a.UserId == customer.user_id);
                if (address == null)
                    return (false, $"Address with ID {orderDto.AddressId} not found or does not belong to the customer", null);

                //1 : get each product and calcluted 
                if (calculatedorder.Items != null && calculatedorder.Items.Count > 0)
                {
                    var itemsBySeller = new Dictionary<int, List<createorderitemsdto>>();

                    foreach (var item in calculatedorder.Items)
                    {
                        //verfiy exit product or not
                        var product = await Context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId && p.isaveliable == true);
                        if (product == null)
                            return (false, $"Product with ID {item.ProductId} not found or is not available", null);
                        if ((!item.VariantId.HasValue || item.VariantId == 0) && product.stockquantity < item.Quantity)
                        {
                            return (false, $"Product with ID {item.ProductId} does not have enough stock. Available: {product.stockquantity}", null);
                        }
                        if (item.VariantId.HasValue)
                        {

                            var variant = await Context.ProductVariants
                                   .FirstOrDefaultAsync(v => v.ProductVariantId == item.VariantId &&
                                                          v.product_id == item.ProductId &&
                                                          v.isaveliable == true);

                            if (variant == null)
                                return (false, $"Variant with ID {item.VariantId} not found or is not available", null);

                            if (variant.stockquantity < item.Quantity)
                                return (false, $"Variant with ID {item.VariantId} does not have enough stock. Available: {variant.stockquantity}", null);
                        }
                        // Get seller ID
                        var sellerId = product.seller_id.Value;

                        // Verify seller exists and is verified
                        var seller = await Context.Sellers.FirstOrDefaultAsync(s => s.Sellerid == sellerId && s.is_verfied == true);
                        if (seller == null)
                            return (false, $"Seller with ID {sellerId} not found or is not verified", null);

                        // Add item to the appropriate seller group
                        if (!itemsBySeller.ContainsKey(sellerId))
                        {
                            itemsBySeller[sellerId] = new List<createorderitemsdto>();
                        }

                        itemsBySeller[sellerId].Add(item);

                    }
                    //prepare suborders
                    decimal ordertotal = 0;
                    foreach (var sellergroup in itemsBySeller)
                    {
                        var sellerid = sellergroup.Key;
                        var gropuitems = sellergroup.Value;
                        //make sub order for mohamed
                        var calculatedSubOrder = new createsuborderdto
                        {
                            seller_id = sellerid,
                            Items = new List<createorderitemsdto>(),
                        };
                        decimal subordertotal = 0;
                        foreach (var item in gropuitems)
                        {
                            decimal itemPrice = 0;
                            var product = await Context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
                            if (item.VariantId != 0 && item.VariantId.HasValue)
                            {
                                var variant = await Context.ProductVariants.FirstOrDefaultAsync(v => v.ProductVariantId == item.VariantId);
                                itemPrice = variant.basicprice;
                                if (variant.discountpercentage > 0)
                                {
                                    itemPrice = itemPrice - (itemPrice * variant.discountpercentage / 100);
                                }
                            }
                            else
                            {
                                // Calculate price with discount if applicable
                                itemPrice = product.basicprice;
                                if (product.discountpercentage > 0)
                                {
                                    itemPrice = itemPrice - (itemPrice * product.discountpercentage / 100);
                                }
                            }
                            // Round to 2 decimal places
                            itemPrice = Math.Round(itemPrice, 2);

                            // Calculate total for this item
                            decimal itemTotal = itemPrice * item.Quantity;
                            subordertotal += itemTotal;
                            // Create calculated order item
                            var calculatedItem = new createorderitemsdto
                            {
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                PriceAtPurchase = itemPrice,
                                TotalPrice = itemTotal,
                                VariantId = item.VariantId
                            };
                            calculatedSubOrder.Items.Add(calculatedItem);
                        }

                        calculatedSubOrder.Subtotal = Math.Round(subordertotal, 2);
                        ordertotal += subordertotal;
                        calculatedorder.SubOrders.Add(calculatedSubOrder);
                    }
                    // Step 3: Calculate discount from coupon if provided
                    decimal discountAmount = 0;

                    if (orderDto.CouponId.HasValue)
                    {
                        var copid = orderDto.CouponId;

                        var coupon = await Context.Coupons
                            .FirstOrDefaultAsync(c => c.CoponId == orderDto.CouponId && c.isActive == true);

                        if (coupon == null)
                            return (false, $"Coupon with ID {orderDto.CouponId} not found or is not active", null);

                        // Verify coupon date validity
                        var currentDate = DateTime.UtcNow;
                       if (currentDate < coupon.startAt || currentDate > coupon.endAt)
                            return (false, "Coupon is not valid at this time", null);

                        // Verify minimum purchase
                        if (coupon.MinimumPurchase.HasValue && ordertotal < coupon.MinimumPurchase.Value)
                            return (false, $"Order subtotal does not meet the minimum purchase requirement of {coupon.MinimumPurchase.Value:C} for this coupon", null);

                        // Calculate discount
                        if (coupon.DiscountType == CouponType.Fixed)
                        {
                            discountAmount = coupon.DiscountPercentage.Value;
                        }
                        else if (coupon.DiscountType == CouponType.Percentage)
                        {
                            discountAmount = ordertotal * (coupon.DiscountPercentage.Value / 100);
                        }

                        // Cap discount at the order subtotal
                        discountAmount = Math.Min(discountAmount, ordertotal);
                        discountAmount = Math.Round(discountAmount, 2);
                    }

                    // Step 4: Calculate tax and shipping
                    decimal taxRate = 0.05m;
                    decimal taxAmount = Math.Round(ordertotal * taxRate, 2);
                    decimal shippingFee = 10.00m;

                    // Reduce or eliminate shipping for larger orders
                    if (ordertotal > 100)
                        shippingFee = 5.00m;

                    if (ordertotal > 200)
                        shippingFee = 0.00m;

                    // Step 5: Calculate final amount
                    decimal finalAmount = ordertotal - discountAmount + taxAmount + shippingFee;
                    finalAmount = Math.Round(finalAmount, 2);

                    // Set all calculated values to the order
                    calculatedorder.TotalAmount = ordertotal;
                    calculatedorder.DiscountAmount = discountAmount;
                    calculatedorder.TaxAmount = taxAmount;
                    calculatedorder.ShippingFee = shippingFee;
                    calculatedorder.FinalAmount = finalAmount;
                }
                else
                {
                    return (false, "No order items provided", null);
                }

                return (true, string.Empty, calculatedorder);
            }

            catch (Exception ex)
            {
                return (false, $"Error validating order: {ex.Message}", null);
            }
        }
        public async Task updateinventory(createorderdto dto)
        {
            foreach (var sub in dto.SubOrders)
            {
                foreach (var item in dto.Items)
                {
                    if (item.VariantId.HasValue)
                    {
                        var variant = await Context.ProductVariants.FindAsync( item.VariantId.Value);
                        if (variant != null)
                        {
                            variant.stockquantity -= item.Quantity;
                         }
                        if (variant.isdefault == true)
                        {

                            // Always update product quantity
                            var product = await Context.Products.FindAsync(item.ProductId);
                            if (product != null)
                            {
                                product.stockquantity -= item.Quantity;
 
                            }
                        }
                    }
                    else
                    {
                        var product = await Context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
                        if (product != null)
                        {
                            product.stockquantity -= item.Quantity;
                        }
                    }
                }
            }
            await Context.SaveChangesAsync();
        }
        public async Task<orderresponsedto> GetOrderByIdAsync(int orderId)
        {
            var order = await Context.Orders
                .Include(o => o.SubOrders)
                    .ThenInclude(so => so.Items)
                .FirstOrDefaultAsync(o => o.Orderid == orderId);

            if (order == null)
                return null;

            var response = mappingoneorder(order);

            return response;
        }
        public async Task<List<orderresponsedto>> getorders(PaginationDto pagination)
        {
            try
            {
                var orders = await Context.Orders
                       .Include(o => o.SubOrders)
                           .ThenInclude(so => so.Items)
                       .OrderByDescending(o => o.createdAt)
                       .Skip((pagination.pageNumber - 1) * pagination.pageSize)
                       .Take(pagination.pageSize)
                       .ToListAsync();

                return mappingorder(orders);
            }
            catch (Exception ex) {
                throw new Exception("error getting orders");
            }
        }
        public async Task<List<orderresponsedto>> getorderbycustomerid(int customerid, PaginationDto pagination)
        {
            try
            {
                var order = await Context.Orders.Where(o => o.customer_id == customerid)
                    .Include(o => o.SubOrders)
                    .ThenInclude(s => s.Items)
                    .OrderByDescending(o => o.createdAt).
                    Skip((pagination.pageNumber - 1) * (pagination.pageSize)).Take(pagination.pageSize).ToListAsync();
                return mappingorder(order);
            }
            catch (Exception ex)
            {
                throw new Exception($"error in getting orders with customerid:{customerid}");
            }
        }
        public async Task<(bool success, string message)> canselorder(int orderid)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var order = await Context.Orders.Include(o => o.SubOrders).
                    ThenInclude(s => s.Items).
                    FirstOrDefaultAsync(o => o.Orderid == orderid);
                if (order == null)
                    return (false, "Order not found or does not belong to this customer");


                if (order.statue != OrderStatus.Pending && order.statue != OrderStatus.Processing)
                    return (false, $"Cannot cancel order with status '{order.statue}'");
                order.updatedAt = DateTime.Now;
                order.statue = OrderStatus.Canceled;
                foreach (var suborder in order.SubOrders)
                {
                    suborder.Status = SubOrderStatus.Canceled;
                    //restore inventory
                    await restoreitemsinsuborder(suborder.Items);
                }
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (true, "order is cansled successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"order cansled is failed:{ex.Message}");
            }
        }
        public async Task<(bool success, string message)> cansledsuborder(int suborderid)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                var suborder = await Context.SubOrders.Include(s => s.Items).Include(s => s.order).
                    FirstOrDefaultAsync(s => s.SubOrderId == suborderid);
                if (suborder == null)
                    return (false, "Suborder not found or does not belong to this seller");

                // Check if suborder can be canceled (only pending or processing suborders can be canceled)
                if (suborder.Status != OrderStatus.Pending && suborder.Status != OrderStatus.Processing)
                    return (false, $"Cannot cancel suborder with status '{suborder.Status}'");

                suborder.StatusUpdatedAt = DateTime.Now;
                suborder.Status = SubOrderStatus.Canceled;
                await restoreitemsinsuborder(suborder.Items);
                //check if all suborders is cansled if so on , cansele main order
                var allsuborders = await Context.SubOrders.Where(s => s.OrderId == suborder.OrderId).ToListAsync();
                if (allsuborders.All(s => s.Status == SubOrderStatus.Canceled))
                {
                    var order = await Context.Orders.FindAsync(suborder.OrderId);
                    if (order != null)
                    {
                        order.statue = OrderStatus.Canceled;
                        order.updatedAt = DateTime.UtcNow;
                    }
                }
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (true, "cansled successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"failed:{suborderid}");
            }
        }
        public async Task<int> getorderscount()
        {
            try
            {

                var orders = await Context.Orders.CountAsync();
                return orders;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> getordercountbycustomerid(int customerid)
        {
            try
            {
                var orders = await Context.Orders.Where(o => o.customer_id == customerid).CountAsync();
                return orders;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> OrderExistsAsync(int id)
        {
            try
            {
                return await Context.Orders.AnyAsync(o => o.Orderid == id);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<orderresponsedto> updateorder(int id, updateinputorderdto orderDto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                //validate the update
                var (isvalid, messageerror) = await validateandupdateorder(id, orderDto);
                if (!isvalid)
                {
                    throw new InvalidOperationException(messageerror);
                }

                var order = await Context.Orders.FindAsync(id);
                if (order == null)
                    throw new KeyNotFoundException($"Order with ID {id} not found");

                // Update only the provided fields
                if (orderDto.CouponId.HasValue)
                    order.CouponId = orderDto.CouponId;

                if (orderDto.DiscountAmount.HasValue)
                    order.DiscountAmount = orderDto.DiscountAmount;

                if (orderDto.ShippingFee.HasValue)
                    order.ShippingFee = orderDto.ShippingFee;

                if (orderDto.TaxAmount.HasValue)
                    order.TaxAmount = orderDto.TaxAmount;

                if (orderDto.FinalAmount.HasValue)
                    order.FinalAmount = orderDto.FinalAmount.Value;

                if (!string.IsNullOrEmpty(orderDto.PaymentStatus))
                    order.PaymentStatus = orderDto.PaymentStatus;

                // Add this block to handle OrderStatus updates
                if (!string.IsNullOrEmpty(orderDto.OrderStatus))
                    order.statue = orderDto.OrderStatus;

                order.updatedAt = DateTime.UtcNow;

                Context.Orders.Update(order);
                await Context.SaveChangesAsync();

                await transaction.CommitAsync();

                //get updated order
                var updatedorder = await Context.Orders
                    .Include(o => o.SubOrders)
                    .ThenInclude(s => s.Items).
                    FirstOrDefaultAsync(o => o.Orderid == orderDto.OrderId);
                return mappingoneorder(updatedorder);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> deleteorder(int orderid)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var order = await Context.Orders.
                    Include(o=>o.SubOrders).
                    FirstOrDefaultAsync(o=>o.Orderid==orderid);
                if (order==null)
                {
                    throw new Exception($"this order is not fount: {orderid}");
                }
                //first delete all orderitems
                foreach (var sub in order.SubOrders)
                {

                    var orderitems = await Context.OrderItems.Where(i=>i.sub_order_id==sub.SubOrderId).ToListAsync();
                    Context.OrderItems.RemoveRange(orderitems);
                }
                await Context.SaveChangesAsync();
                //secount delete suborders
                var suborders = await Context.SubOrders.Where(s=>s.OrderId==orderid).ToListAsync();
                Context.SubOrders.RemoveRange(suborders);
                await Context.SaveChangesAsync();
                //delete order
                 Context.Orders.Remove(order);
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
        public async Task<(bool, string)> validateandupdateorder(int orderId, updateinputorderdto dto)
        {//validate order
            var order = await Context.Orders.FindAsync(orderId);
            if (order == null)
                return (false, $"Order with ID {orderId} not found");

            //payment statues
            if (!string.IsNullOrEmpty(dto.PaymentStatus))
            {
                var validStatuses = new[] { "pending", "paid", "failed", "refunded", "partially_refunded" };
                if (!validStatuses.Contains(dto.PaymentStatus.ToLower()))
                    return (false, $"Invalid payment status: {dto.PaymentStatus}. Valid values are: {string.Join(", ", validStatuses)}");
            }
            //validate order statue
            if (!string.IsNullOrEmpty(dto.OrderStatus))
            {
                var validateorderstatues = new[] { "pending", "processing", "shipped", "Delivered", "canceled", "returned", "failed" };
                if (!validateorderstatues.Contains(dto.OrderStatus.ToLower()))
                {
                    return (false, $"Invalid order status: {dto.OrderStatus}. Valid values are: {string.Join(", ", validateorderstatues)}");
                }
            }
            //validate copoune
            if (dto.CouponId.HasValue)
            {
                var coupon = await Context.Coupons
                    .FirstOrDefaultAsync(c => c.CoponId == dto.CouponId && c.isActive == true);

                if (coupon == null)
                    return (false, $"Coupon with ID {dto.CouponId} not found or is not active");

                var currentdate = DateTime.UtcNow;
                if (currentdate > coupon.endAt || currentdate < coupon.startAt)
                {
                    return (false, "Coupon is not valid at this time");
                }
            }
            return (true, "validate successfully");
        }
        public async Task restoreitemsinsuborder(ICollection<OrderItems> items)
        {

            foreach (var item in items) {
                if (!item.variant_id.HasValue || item.ProductVariant == null)
                {
                    var product = await Context.Products.FirstOrDefaultAsync(p => p.ProductId == item.product_id);
                    if (product != null)
                    {
                        product.stockquantity += item.quantity;
                    }
                }
                else if (item.variant_id.HasValue)
                {
                    var variant = await Context.ProductVariants.FirstOrDefaultAsync
                        (v => item.product_id == v.product_id &&
                    v.ProductVariantId == item.variant_id);
                    if (variant != null)
                    {
                        variant.stockquantity += item.quantity;

                        if (variant.isdefault == true)
                        {
                            var product = await Context.Products.FirstOrDefaultAsync(p => p.ProductId == item.product_id);
                            if (product != null)
                            {
                                product.stockquantity += item.quantity;
                            }
                        }
                    }
                }
            }

        }
        public List<orderresponsedto> mappingorder(List<order> orders)
        {
            if (orders == null || orders.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(orders));
            }
            List<orderresponsedto> response = new List<orderresponsedto>();
            foreach (var order in orders)
            {
                orderresponsedto orderresponse = new orderresponsedto
                {
                    Orderid = order.Orderid,
                    TotalAmount = order.TotalAmount,
                    createdAt = order.createdAt,
                    updatedAt = order.updatedAt,
                    DiscountAmount = order.DiscountAmount,
                    ShippingFee = order.ShippingFee,
                    TaxAmount = order.TaxAmount,
                    FinalAmount = order.FinalAmount,
                    PaymentMethod = order.PaymentMethod,
                    PaymentStatus = order.PaymentStatus,
                    statue = order.statue,
                    address_id = order.address_id,
                    CouponId = order.CouponId,
                    AffiliateCode = order.AffiliateCode,
                    AffiliateId = order.AffiliateId,
                    //SubOrders = order.SubOrders,
                    customer_id = order.customer_id.Value,
                };
                // all suborders

                foreach (var suborder in order.SubOrders)
                {
                    var suborderdto = new suborderdto
                    {
                        OrderId = suborder.OrderId.Value,
                        SubOrderId = suborder.SubOrderId,
                        seller_id = suborder.seller_id.Value,
                        ShippingProvider = suborder.ShippingProvider,
                        Status = suborder.Status,
                        StatusUpdatedAt = suborder.StatusUpdatedAt,
                        Subtotal = suborder.Subtotal,
                        TrackingNumber = suborder.TrackingNumber,
                    };
                    var orderitems = new List<orderitemsdto>();
                    foreach (var item in suborder.Items)
                    {
                        var itemdto = new orderitemsdto
                        {
                            OrderItemsId = item.OrderItemsId,
                            PriceAtPurchase = item.PriceAtPurchase,
                            sub_order_id = item.sub_order_id.Value,
                            product_id = item.product_id.Value,
                            quantity = item.quantity,
                            TotalPrice = item.TotalPrice,
                            variant_id = item.variant_id.Value,
                        };
                        orderitems.Add(itemdto);
                    }
                    suborderdto.Items = orderitems;
                    orderresponse.SubOrders.Add(suborderdto);
                }
                response.Add(orderresponse);
            }
            return response;
        }
        public orderresponsedto mappingoneorder(order order)
        {
            if (order==null)
            {
                return null;
            }
            var response = new orderresponsedto
            {

                Orderid = order.Orderid,
                TotalAmount = order.TotalAmount,
                createdAt = order.createdAt,
                updatedAt = order.updatedAt,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TaxAmount = order.TaxAmount,
                FinalAmount = order.FinalAmount,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                statue = order.statue,
                address_id = order.address_id,
                CouponId = order.CouponId,
                AffiliateCode = order.AffiliateCode,
                AffiliateId = order.AffiliateId,
                customer_id = order.customer_id.Value,
            };
            //suborders
            
            foreach (var suborder in order.SubOrders)
            {
                var suborderdto = new suborderdto { 
                  OrderId=suborder.OrderId.Value,
                  SubOrderId=suborder.SubOrderId,
                  seller_id=suborder.seller_id.Value,
                  ShippingProvider=suborder.ShippingProvider,
                  Status=suborder.Status,
                  StatusUpdatedAt=suborder.StatusUpdatedAt,
                  Subtotal=suborder.Subtotal,
                  TrackingNumber=suborder.TrackingNumber,
                };
                var orderitems =new List<orderitemsdto>();
                foreach (var item in suborder.Items)
                {
                    var itemdto = new orderitemsdto {
                        OrderItemsId=item.OrderItemsId,
                        PriceAtPurchase=item.PriceAtPurchase,
                        sub_order_id=item.sub_order_id.Value,
                        product_id=item.product_id.Value,
                        quantity=item.quantity,
                        TotalPrice=item.TotalPrice,
                        variant_id=item.variant_id.Value,
                    };
                    orderitems.Add(itemdto);
                }
                suborderdto.Items = orderitems;
                response.SubOrders.Add(suborderdto);
            }
            return response;
    } 
    }
}