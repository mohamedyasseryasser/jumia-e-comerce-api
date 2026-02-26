namespace e_comerce_api.DTO
{
    public class addproductattrvaluedto
    {
       public string attributevalue {  get; set; }
       public int attributeid {  get; set; }
       public int productid {  get; set; }
       public int variantid {  get; set; }
    }
    public class productattributevalueresponse:addproductattrvaluedto
    {
        public string attributename {  get; set; }
    }
    public class updateproductattributevaluedto
    {
        public int attributeid { get; set; }
        public int productid { get; set; }
        public int variantid { get; set; }
        public int valueid {  get; set; }
        public string value {  get; set; }
    }
}
