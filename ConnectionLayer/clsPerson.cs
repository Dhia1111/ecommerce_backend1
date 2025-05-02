using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.ComponentModel.DataAnnotations;




public class DTOPerson
{


     public int PersonID { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public string Phone { get; set; }
    public string Country { get; set; }
    public string PostCodeAndLocation { get; set; }
    public string  City { get; set; }


    public DTOPerson(int personID, string firstName, string lastName, string email, string phone, string Country,string City,string PostCode)
    {
        this.PersonID = personID;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email;
        this.Phone = phone;
        this.Country = Country;
        this.City = City;
        this.PostCodeAndLocation = PostCode;
  
    }


    public DTOPerson()
    {
        this.PersonID = -1;
        this.FirstName = "";
        this.LastName = "";
        this.Email = "example@.com";
        this.Phone = "";
        this.Country = "";
        this.City = "";
        this.PostCodeAndLocation = "";

    }


}


namespace ConnectionLayer
{

    public static class clsPerson
    {

        public static async Task<DTOPerson?> Find(int ID)
        {

           
            string qery = "select top 1* From People where PersonID=@PersonID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@PersonID", ID);


                        using (SqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOPerson Person = new DTOPerson(-1, "", "", "", "", "", "", "");

                               
                                if((int.TryParse(Reader["PersonID"].ToString(),out int PersonID) && Reader["FirstName"] != null && Reader["LastName"] != null || Reader["Email"] != null || Reader["Phone"] != null || Reader["Country"] != null
                                    && Reader["City"] != null && Reader["PostCode"] != null))
                                {

                                    Person.PersonID = PersonID;
                                    Person.FirstName = Reader["FirstName"].ToString();
                                    Person.LastName = Reader["LastName"].ToString();
                                    Person.Email = Reader["Email"].ToString();
                                    Person.Phone = Reader["Phone"].ToString();
                                    Person.Country = Reader["Country"].ToString();
                                    Person.City = Reader["City"].ToString();
                                    Person.PostCodeAndLocation = Reader["PostCode"].ToString();

                                    return Person;
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



        public static async Task<List<DTOPerson>?> GetPeoPle()
        {
            string qery = "select*From People";

            List<DTOPerson> people = new List<DTOPerson>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        using (SqlDataReader Reader =await command.ExecuteReaderAsync())
                        {
                            while (Reader.Read())
                            {

                                if ((int.TryParse(Reader["PersonID"].ToString(), out int PersonID) || Reader["FirstName"] == null || Reader["LastName"] == null || Reader["Email"] == null || Reader["Phone"] == null || Reader["Country"] == null
                                                               || Reader["City"] == null || Reader["PostCode"] == null))
                                {
                                    continue;
                                }


                           else    people.Add(new DTOPerson(PersonID, Reader["FirstName"].ToString(), Reader["LastName"].ToString(), Reader["Email"].ToString(), Reader["Phone"].ToString(), Reader["Country"].ToString(), Reader["City"].ToString(), Reader["PostCode"].ToString()));





                            }

                        }


                    }

                }
            }


            catch 
            {

                return null;
            }




            return people;
        }


        public static async Task<int> AddPerson(DTOPerson person)
        {

            string qery = "insert into People(FirstName,LastName ,Email,Phone,Country,City,PostCode)" +
                "values(@FirstName,@LastName,@Email,@Phone,@Country,@City,@PostCode);Select SCOPE_IDENTITY()";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@FirstName", person.FirstName);
                        command.Parameters.AddWithValue("@LastName", person.LastName);
                        command.Parameters.AddWithValue("@Email", person.Email);
                        command.Parameters.AddWithValue("@Phone", person.Phone);
                        command.Parameters.AddWithValue("@Country", person.Country); 
                        command.Parameters.AddWithValue("@City", person.City);
                        command.Parameters.AddWithValue("@PostCode", person.PostCodeAndLocation);



                        object? objPersonID =await command.ExecuteScalarAsync();

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



        public static async Task<bool> UpdatePerson(DTOPerson Person)
        {

            string qery = @"Update People set 
                       
               FirstName=  @FirstName,
              LastName=  @LastName,
              Email=  @Email,
              Phone=  @Phone,
              Country=  @Country,
              City=  @City,
              PostCode=  @PostCode

where PersonID=@PersonID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@FirstName", Person.FirstName);
                        command.Parameters.AddWithValue("@LastName", Person.LastName);
                        command.Parameters.AddWithValue("@Email", Person.Email);
                        command.Parameters.AddWithValue("@Phone", Person.Phone);
                        command.Parameters.AddWithValue("@Country", Person.Country);
                        command.Parameters.AddWithValue("@City", Person.City);
                        command.Parameters.AddWithValue("@PostCode", Person.PostCodeAndLocation);
                        command.Parameters.AddWithValue("@PersonID", Person.PersonID);


                        int NumberRowAffected =await command.ExecuteNonQueryAsync();

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

            string qery = @"Delete from People  where PersonID=@PersonID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@PersonID", ID);


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