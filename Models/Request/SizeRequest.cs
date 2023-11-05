using System.ComponentModel.DataAnnotations;

namespace UNITEE_BACKEND.Models.Request
{
    public class SizeRequest
    {
        public int ProductId { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
    }
}
