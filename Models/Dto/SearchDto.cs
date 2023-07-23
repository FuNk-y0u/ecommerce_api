namespace ecommerce.Models.Dto
{
    public class SearchDto
    {
        public string Text { get; set; }
        public int category { get; set; }
        public string sort { get; set; }

        public int pagesize { get; set; }
        public int page { get; set; }


    }
}
