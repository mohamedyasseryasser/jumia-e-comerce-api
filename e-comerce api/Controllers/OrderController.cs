using e_comerce_api.DTO;
using e_comerce_api.DTO.orderdto;
using e_comerce_api.services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;

namespace e_comerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly Iorderreprosity _orderRepository;
        private readonly IUser user;

        public OrderController(Iorderreprosity orderRepository,IUser _user)
        {
            _orderRepository = orderRepository;
            user = _user;
        }
        [HttpPost("createorder")]
        public async Task<IActionResult> CreateOrder([FromBody] createorderdto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid order data",
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()
                });
            }

            try
            {
                var result = await _orderRepository.createorder(orderDto);

                return Ok(new ApiResponse<orderresponsedto>
                {
                    message = "Order created successfully",
                    data = result,
                    statue = true
                }
                );
            }

            catch (Exception ex)
            {
                // Unexpected error
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while creating the order",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpGet("getorderbyid/{id}")]
        public async Task<IActionResult> GetOrderbyid(int orderid) 
        {
            try
            {
               var orderresult =await _orderRepository.GetOrderByIdAsync(orderid);
                if (orderresult==null)
                {
                    return NotFound(new ApiErrorResponse()
                    {
                        Message = "this orderid is notfount"
                    });
                }
                return Ok(new ApiResponse<orderresponsedto?>() { 
                  data=orderresult,
                  message="successfully",
                  statue=true
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500,new ApiErrorResponse()
                {
                    Message = $"An error occurred while retrieving order with id = {orderid}",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpGet("getorders")]
        public async Task<IActionResult> getorders([FromQuery]PaginationDto pagination)
        {
            try
            {
                if (pagination.pageNumber < 1)
                    pagination.pageNumber = 1;

                if (pagination.pageSize < 1)
                    pagination.pageSize = 10;
                var ordersresult = await _orderRepository.getorders(pagination);
                var totalcount = await _orderRepository.getorderscount();
                return Ok(new ApiResponse<object>()
                {
                    data=new
                    {
                        items=ordersresult,
                        TotalCount = totalcount,
                        PageNumber = pagination.pageNumber,
                        PageSize = pagination.pageSize,
                        TotalPages = (int)Math.Ceiling(totalcount / (double)pagination.pageSize),
                    },
                    message="successfully",
                    statue=true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse()
                {
                    Message = "failed",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpGet("getordersbycustomerid/{id}")]
        public async Task<IActionResult> getordersbycustomerid(int customerid,[FromQuery]PaginationDto pagination)
        {
            try
            {
                var customer = await user.getcustomerbyid(customerid);
                if (customer==null)
                {
                    return NotFound(new ApiErrorResponse()
                    {
                        Message = $"this customerid:{customerid} is not found"
                    });
                }

                if (pagination.pageNumber < 1)
                    pagination.pageNumber = 1;

                if (pagination.pageSize < 1)
                    pagination.pageSize = 10;
                var orders = await _orderRepository.getorderbycustomerid(customerid,pagination);
                int totalcount = orders.Count;
                return Ok(new ApiResponse<object>
                
                {
                    data = new {
                    items = orders,
                    TotalCount = totalcount,
                    PageNumber = pagination.pageNumber,
                    PageSize = pagination.pageSize,
                    TotalPages = (int)Math.Ceiling(totalcount / (double)pagination.pageSize),
                    },
                    statue=true,
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while retrieving customer orders",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpPost("cansleorder/{id}")]
        public async Task<IActionResult> canselorder(int orderid)
        {
            try
            {
                var orderresult = await _orderRepository.canselorder(orderid);
                if (!orderresult.success)
                {
                    return NotFound(new ApiErrorResponse()
                    {
                        Message=orderresult.message
                    });
                }
                return Ok(new ApiResponse<object> {
                  data= new { 
                   success=true,
                   message=orderresult.message
                  },
                  statue=true,
                  message=orderresult.message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while retrieving customer orders",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpPost("canslesuborder/{id}")]
        public async Task<IActionResult> canselsuborder(int suborderid)
        {
            try
            {
                var orderresult = await _orderRepository.cansledsuborder(suborderid);
                if (!orderresult.success)
                {
                    return NotFound(new ApiErrorResponse()
                    {
                        Message = orderresult.message
                    });
                }
                return Ok(new ApiResponse<object>
                {
                    data = new
                    {
                        success = true,
                        message = orderresult.message
                    },
                    statue = true,
                    message = orderresult.message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while retrieving customer orders",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpPost("updateorder/{id}")]
        public async Task<IActionResult> updateorder(int orderid,[FromBody]updateinputorderdto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Invalid order data",
                        ErrorMessages = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToArray()
                    });
                }
                if (orderid!=dto.OrderId)
                {
                    return BadRequest(new ApiErrorResponse()
                    {
                        Message="orderid is not equal dt.orderid"
                    });
                }
                if (!await _orderRepository.OrderExistsAsync(dto.OrderId))
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Message = "Order not found",
                      
                    });
                }
                var updatedOrder = await _orderRepository.updateorder(orderid, dto);

           

                return Ok(new ApiResponse<orderresponsedto>
                {
                    message = "Order updated successfully",
                    data = updatedOrder,
                    statue = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while updating the order",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpDelete("{id}")]
        [EnableRateLimiting("strict")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!await _orderRepository.OrderExistsAsync(id))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        message = "Order not found",
                        statue = false,
                        data = null
                    });
                }

                var result = await _orderRepository.deleteorder(id);

                // Invalidate caches
                //InvalidateOrderCache(id);
                //InvalidateAllOrderCaches();

                return Ok(new ApiResponse<object>
                {
                   message = "Order deleted successfully",
                    data = null,
                    statue = true
                });
            }
            catch (Exception ex)
            {
                 return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while deleting the order",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
    }
    }

