namespace ecommerce.Models.Dto
{
    public class CartPutDto
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public int Qty { get; set; }
    }
}
