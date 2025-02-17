

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DTOCatygory {

    public int ID {  get; set; }
    public string Name { get; set; }
    public DTOCatygory(int ID, string Name) {

        ID = ID;
        Name = Name;
    }
}

namespace ConnectionLayer
{
    
    public static class clsCatygorie
    
    {

        public static async Task<DTOCatygory?> Find(int ID)
        {


            string qery = "select top 1* From Catygories where CatygoryID=@CatygoryID";


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


                                DTOCatygory CatyGory = new DTOCatygory(-1,"");


                                if (!(int.TryParse(Reader["CatygoryID"].ToString(), out int CatygoryID) ||
                                   Reader["CatygoryName"]==null))

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

            string qery = "select top 1* From Catygories where CatygoryName=@CatygoryName";


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
            string qery = "Select*From Catygories ";

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
                                 Reader["CatygoryName"]!=null))
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

            string qery = "insert into Catygories(CatygoryName)" +
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

            string qery = @"Update Catygories set 
                       
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

}
