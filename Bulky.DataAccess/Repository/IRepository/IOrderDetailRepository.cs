using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
      //void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);

      //  void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
    }
}
