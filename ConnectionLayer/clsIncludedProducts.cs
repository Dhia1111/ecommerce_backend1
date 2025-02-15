using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 public  class DTOIncludedProducts{

    public int ID {  get; set; }
    public int TransactionID {  get; set; }
    public int ProductID { get; set; }

    public DTOIncludedProducts(int ID ,int TransactionID,int ProductID)
    {
        this.ID = ID;
        this.TransactionID = TransactionID;
        this.ProductID = ProductID;

    }
      
    }

namespace ConnectionLayer
{
    public static class clsIncludedProducts
    
        
    {
        public static async Task<DTOIncludedProducts?> Find(int ID)
           {


            string qery = "select top 1* From IncludedProducts where IncludedProductID=@IncludedProductID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@IncludedProductID", ID);


                        using (SqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOIncludedProducts IncludedProduct = new DTOIncludedProducts(-1, -1, -1);


                                if (!(int.TryParse(Reader["IncludedProductID"].ToString(), out int IncludedProductID) ||
                                    int.TryParse(Reader["TransactionID"].ToString(), out int TransactionID) ||
                                    int.TryParse(Reader["ProductID"].ToString(), out int ProductID) ))
                                 
                                {

                                    IncludedProduct.TransactionID = TransactionID;
                                    IncludedProduct.ProductID = ProductID;
                                    IncludedProduct.ID = IncludedProductID;
                                 


                                    return IncludedProduct;
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

        public static async Task<List<DTOIncludedProducts>?> GetAll()
        {
            string qery = "Select*From IncludedProducts ";

            List<DTOIncludedProducts> Users = new List<DTOIncludedProducts>();

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
                                if (!(int.TryParse(Reader["IncludedProducts"].ToString(), out int IncludedProducts) ||
                                 int.TryParse(Reader["TransactionID"].ToString(), out int TransactionID) ||
                                 int.TryParse(Reader["ProductID"].ToString(), out int ProductID)))
                                {
                                    Users.Add(new DTOIncludedProducts(IncludedProducts,TransactionID,ProductID));
                                }
                                else
                                {
                                    continue;
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




            return Users;
        }


        public static async Task<int> Add(DTOIncludedProducts IncludedProducts)
        {

            string qery = "insert into IncludedProducts(TransactionID,ProductID)" +
                "values(@TransactionID,@ProductID);Select SCOPE_IDENTITY()";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@TransactionID",IncludedProducts.TransactionID);
                        command.Parameters.AddWithValue("@ProductID", IncludedProducts.ProductID);
                       


                        object? objPersonID = await command.ExecuteScalarAsync();

                          if (objPersonID != null)
                          {

                            if (int.TryParse(objPersonID.ToString(), out int ID))
                            {
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


        public static async Task<bool> Update(DTOIncludedProducts IncludedProduct)
        {

            string qery = @"Update IncludedProducts set 
                       
               TransactionID=  @TransactionID,
              ProductID=  @ProductID,
              
        where         IncludedProductID=@IncludedProductID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@IncludedProductID", IncludedProduct.ID);
                        command.Parameters.AddWithValue("@TransactionID", IncludedProduct.TransactionID);
                        command.Parameters.AddWithValue("@ProductID", IncludedProduct.ProductID);
                      

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

            string qery = @"Delete from  IncludedProducts  where IncludedProductID=@IncludedProductID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@IncludedProductID", ID);


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


