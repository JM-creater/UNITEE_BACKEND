using System.ComponentModel.DataAnnotations.Schema;
using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Models.Request
{
    public class RatingGetRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public int Value { get; set; }
    }
}
