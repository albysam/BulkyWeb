using BulkyWeb.Models;
using BulkyWeb.Services;
namespace BulkyWeb.Services
{
    internal interface IEmailService
    {

        void SendEmail(Message message);


    }
}
