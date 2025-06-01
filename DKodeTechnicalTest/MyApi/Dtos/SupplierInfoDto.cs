namespace MyApi.Dtos
{
    // DTO class for supplier info
    // Field names match with names used in sql string from supplier info repo
    public class SupplierInfoDto
    {
        public string SupplierName { get; set; }
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public int TotalStockQuantity { get; set; }
        public decimal AverageDiscountedPrice { get; set; }
        public decimal TotalStockValue { get; set; }
        public string ShippedBy { get; set; }
    }
}
