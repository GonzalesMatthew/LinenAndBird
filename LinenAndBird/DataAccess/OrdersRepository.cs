using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinenAndBird.DataAccess
{
    public class OrdersRepository
    {
        static List<Order> _orders = new List<Order>
            {
                new Order
                {

                }
            };

        internal void Add(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
