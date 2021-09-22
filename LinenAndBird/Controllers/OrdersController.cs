using LinenAndBird.DataAccess;
using LinenAndBird.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinenAndBird.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        BirdRepository _birdRepo;
        HatRepository _hatRepo;
        OrdersRepository _orderRepo;

        public OrdersController(BirdRepository birdRepo, HatRepository hatRepo, OrdersRepository ordersRepo)
        {
            _birdRepo = birdRepo;
            _hatRepo = hatRepo;
            _orderRepo = ordersRepo;
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            return Ok(_orderRepo.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(Guid id)
        {
            Order order = _orderRepository.Get(id);

            if (order == null)
            {
                return NotFound("No order exists with that id");
            }

            return Ok(order);
            
        }


        //[HttpPost("bird/{birdId}/hat{hatId}/{price}")] // could do this, but a better way is to create a model specifically for this

        //public IActionResult CreateOrder(Guid birdId, Guid hatId, double price)
        [HttpPost]
        public IActionResult CreateOrder(CreateOrderCommand command)
        {
            var hatToOrder = _hatRepo.GetById(command.HatId);
            var birdToOrder = _birdRepo.GetById(command.BirdId);

            if (hatToOrder == null)
                return NotFound("There was no matching hat in the database.");
            if (birdToOrder == null)
                return NotFound("There was no matching bird in the databse");
            var order = new Order
            {
                Bird = birdToOrder,
                Hat = hatToOrder,
                Price = command.Price
            };

            _orderRepo.Add(order);

            return Created($"/api/orders/{order.Id}", order);
        }
    }
}
