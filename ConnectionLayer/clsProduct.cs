using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DTOProduct { 

    public int ID { get; set; }
    public string Category { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }
   
    public string Imagepath { get; set; }

    public DTOProduct(int ID,string Category,string Name,decimal Price,string Imagepath)
    {
        this.ID = ID;
        this.Category = Category;
        this.Name = Name;
        this.Price = Price;
        this.Imagepath = Imagepath;

    }
}

namespace ConnectionLayer
{
    public static class clsProduct

    {

        public static async Task<DTOProduct?> Find(int ID)
        {


            string qery = "select top 1* From Products where ProductID=@ProductID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ProductID", ID);


                        using (SqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOProduct Product = new DTOProduct(-1, "", "", 0, "");


                                if (!(
                                    int.TryParse(Reader["ProductID"].ToString(), out int ProductID) || 
                                    Reader["ProductName"] == null ||
                                    Reader["ProductCatigory"] == null ||
                                   decimal.TryParse(Reader["ProductPrice"].ToString(),out decimal Price) ||
                                    Reader["ProductImagePath"] == null

                                    )
                                    )
                                {

                                    Product.ID = ProductID;
                                    Product.Price = Price;
                                    Product.Name = Reader["ProductName"].ToString();
                                    Product.Category = Reader["ProductCategory"].ToString();
                                    Product.Imagepath = Reader["ProductImagePath"].ToString();
                                 
                                    return Product;
                                }






                            }

                        }


                    }

                }
            }


            catch
            {

                return null;
            }


            return null;



        }



        public static async Task<List<DTOProduct>?> Get()
        {
            string qery = "select*from Products";

            List<DTOProduct> Products = new List<DTOProduct>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        using (SqlDataReader Reader = await command.ExecuteReaderAsync())
                        {
                            while (Reader.Read())
                            {

                                if (!(
                             int.TryParse(Reader["ProductID"].ToString(), out int ProductID) ||
                             Reader["ProductName"] == null ||
                             Reader["ProductCatigory"] == null ||
                            decimal.TryParse(Reader["ProductPrice"].ToString(), out decimal Price) ||
                             Reader["ProductImagePath"] == null

                             )
                             )
                                {
                                    DTOProduct Product = new DTOProduct(-1, "", "", 0, "");
                                    Product.ID = ProductID;
                                    Product.Price = Price;
                                    Product.Name = Reader["ProductName"].ToString();
                                    Product.Category = Reader["ProductCategory"].ToString();
                                    Product.Imagepath = Reader["ProductImagePath"].ToString();

                                    Products.Add(Product);
                                 }

                                else continue;




                            }

                        }


                    }

                }
            }


            catch
            {

                return null;
            }




            return Products;
        }


        public static async Task<int> Add(DTOProduct Product)
        {

            string qery = "insert into Products(ProductPrice,ProductImagePath ,ProductName,ProductCatigory)" +
                "values(@ProductPrice,@ProductImagePath,@ProductName,@ProductCatigory);Select SCOPE_IDENTITY()";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ProductPrice", Product.Price);
                        command.Parameters.AddWithValue("@ProductImagePath", Product.Imagepath);
                        command.Parameters.AddWithValue("@ProductName", Product.Name);
                        command.Parameters.AddWithValue("@ProductCatigory", Product.Category);
                      

                        object? objPersonID = await command.ExecuteScalarAsync();

                        if (objPersonID != null)
                        {

                            if (int.TryParse(objPersonID.ToString(), out int ID))
                            {
                                Product.ID = ID;
                                return ID;
                            }
                            else
                            {
                                return -1;
                            }
                        }


                    }

                }
            }


            catch
            {

                return -1;
            }




            return -1;

        }



        public static async Task<bool> Update(DTOProduct Product)
        {

            string qery = @"Update Products set 
                       
              ProductPrice=     @ProductPrice,
              ProductImagePath= @ProductImagePath,
              ProductName=      @ProductName,
              ProductCatigory=  @ProductCatigory,
 
         where ProductID=@ProductID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ProductPrice", Product.Price);
                        command.Parameters.AddWithValue("@ProductImagePath", Product.Imagepath);
                        command.Parameters.AddWithValue("@ProductName", Product.Name);
                        command.Parameters.AddWithValue("@ProductCatigory", Product.Category);
                        command.Parameters.AddWithValue("@ProductID", Product.ID);
                   

                        int NumberRowAffected = await command.ExecuteNonQueryAsync();

                        if (NumberRowAffected == 0)
                        {

                            return false;

                        }


                    }

                }
            }


            catch
            {

                return false;
            }




            return true;

        }

        public static async Task<bool> Delete(int ID)
        {

            string qery = @"Delete from Products  where ProductID=@ProductID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ProductID", ID);


                        int NumberRowAffected = await command.ExecuteNonQueryAsync();

                        if (NumberRowAffected == 0)
                        {

                            return false;

                        }


                    }

                }
            }


            catch
            {

                return false;
            }




            return true;

        }




    }
}
