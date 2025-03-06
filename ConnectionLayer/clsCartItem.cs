using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DTOCart 
{
    
    public int CartID {  get; set; }

    public  uint NumberOfItems {  get; set; }

    public int UserID { get; set; }

    public int ProductID {  get; set; }

    public DTOCart() { 
             CartID = -1;
        this.UserID = -1;
        this.ProductID = -1;
        this.NumberOfItems = 1;
    }
}

namespace ConnectionLayer
{

   
    public static class clsCart
    {
        public static async Task<DTOCart?> Find(int ID)
        {


            string qery = "select top 1* From CartItems where CatygoryID=@CatygoryID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@CatygoryID", ID);


                        using (SqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTO CatyGory = new DTOCart(-1, "");


                                if (!(int.TryParse(Reader["CatygoryID"].ToString(), out int CatygoryID) ||
                                   Reader["CatygoryName"] == null))

                                {

                                    CatyGory.ID = CatygoryID;
                                    CatyGory.Name = Reader["CatygoryName"].ToString();



                                    return CatyGory;
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
        public static async Task<DTOCatygory?> Find(string Name)
        {

            string qery = "select top 1* From Catigories where CatygoryName=@CatygoryName";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@CatygoryName", Name);


                        using (SqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOCatygory CatyGory = new DTOCatygory(-1, "");


                                if (!(int.TryParse(Reader["CatygoryID"].ToString(), out int CatygoryID) ||
                                   Reader["CatygoryName"] == null))

                                {

                                    CatyGory.ID = CatygoryID;
                                    CatyGory.Name = Reader["CatygoryName"].ToString();



                                    return CatyGory;
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
        public static async Task<List<DTOCatygory>?> GetAll()
        {
            string qery = "Select*From Catigories ";

            List<DTOCatygory> Catygories = new List<DTOCatygory>();

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
                                if (!(int.TryParse(Reader["CatygoryID"].ToString(), out int CatyGoryID) ||
                                 Reader["CatygoryName"] != null))
                                {
                                    Catygories.Add(new DTOCatygory(CatyGoryID, Reader["CatygoryName"].ToString()));
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




            return Catygories;
        }
        public static async Task<int> Add(DTOCatygory dTOCatygory)
        {
            string qery = "insert into Catigories(CatygoryName)" +
                "values(@CatygoryName);Select SCOPE_IDENTITY()";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@CatygoryName", dTOCatygory.Name);



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
        public static async Task<bool> Update(DTOCatygory Catigory)
        {

            string qery = @"Update Catigories set 
                       
               CatygoryName=  @CatygoryName,
              
        where         CatygoryID=@CatygoryID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@CatygoryName", Catigory.ID);
                        command.Parameters.AddWithValue("@CatygoryID", Catigory.Name);



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

            string qery = @"Delete from  Catigories  where CatigoryID=@CatigoryID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@CatigoryID", ID);


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
}
