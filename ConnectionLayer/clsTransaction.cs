using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLayer
{
    public class DTOTransaction { 
    
   public int ID {  get; set; }

 public    string PaymentMethodID {  get; set; }


       public byte State {  get; set; }
        
      public  decimal TotolePrice {  get; set; }

     public   int CustomerID {  get; set; }

     public string TransactionGUID {  get; set; }

        public DTOTransaction(int ID,string PaymentMethodID,byte State ,decimal TotolePrice,int CustomerID, string TransactionGUID)
        {
            this.ID = ID;
            this.PaymentMethodID = PaymentMethodID;
            this.State = State;
            this.TotolePrice = TotolePrice;
            this.CustomerID = CustomerID;
            this.TransactionGUID = TransactionGUID;
        }
       
    }

    public static class clsTransaction

    {

        public static async Task<DTOTransaction?> Find(int ID)
        {


            string qery = "select top 1* From Transactions where TransactionID=@TransactionID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@TransactionID", ID);


                        using (SqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {
                                 DTOTransaction Transaction = new DTOTransaction(-1, "", 0, 0, -1, "");

                                if (!(
                                        int.TryParse(Reader["TransactionID"].ToString(), out int TransactionID) || 
                                        byte.TryParse(Reader["TransactionState"].ToString(),out byte  State) || 
                                        byte.TryParse(Reader["TransactionAtherization"].ToString(), out byte Atherization) || 
                                        int.TryParse(Reader["TransactionTotolePrice"].ToString(), out int TotolePrice) || 
                                        int.TryParse(Reader["TransactionUserID"].ToString(), out int CustomerID)||
                                        Reader["TransactionPaymentMethodID"] == null || Reader["TransactionGUID"] == null)) 
                                {

                                    Transaction.ID = TransactionID;
                                    Transaction.PaymentMethodID = Reader["TransactionPaymentMethodID"].ToString();
                                    Transaction.State = State;
                                    Transaction.TotolePrice = TotolePrice;
                                    Transaction.CustomerID =CustomerID ;
                                    Transaction.TransactionGUID = Reader["TransactionGUID"].ToString();
                                
                                    return Transaction;
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
        public static async Task<List<DTOTransaction>?> Get()
        {
            string qery = "select*From Transactions";

            List<DTOTransaction> Transaction = new List<DTOTransaction>();

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
                                      int.TryParse(Reader["TransactionID"].ToString(), out int TransactionID) ||
                                      byte.TryParse(Reader["TransactionState"].ToString(), out byte State) ||
                                      int.TryParse(Reader["TransactionTotolePrice"].ToString(), out int TotolePrice) ||
                                      int.TryParse(Reader["TransactionUserID"].ToString(), out int CustomerID) ||
                                      Reader["TransactionPaymentMethodID"] == null ||
                                      Reader["TransactionGUID"] == null))

                                {
                                    Transaction.Add(new DTOTransaction(TransactionID, Reader["TransactionPaymentMethodID"].ToString(), State,TotolePrice ,CustomerID, Reader["TransaactionGUID"].ToString()));

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




            return Transaction;
        }

        public static async Task<int> Add(DTOTransaction Transaction)
        {

            string qery = @"insert into Transaction(TransactionPaymentMethodID,TransactionState ,TransactionTotolePrice,TransactionUserID,TransactionGUID)

          values(@TransactionPaymentMethodID,@TransactionState,@TransactionTotolePrice,@TransactionUserID,@TransactionGUID)
        ;Select SCOPE_IDENTITY()";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@TransactionPaymentMethodID", Transaction.PaymentMethodID);
                        command.Parameters.AddWithValue("@TransactionState", Transaction.State);
                        command.Parameters.AddWithValue("@TransactionTotolePrice", Transaction.TotolePrice);
                        command.Parameters.AddWithValue("@TransactionUserID", Transaction.CustomerID);
                        command.Parameters.AddWithValue("@TransactionGUID", Transaction.TransactionGUID);
                      



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

        public static async Task<bool> Update(DTOTransaction Transaction)
        {

            string qery = @"Update Transactions set 
                       
              TransactionPaymentMethodID=  @TransactionPaymentMethodID,
              TransactionState=   @TransactionState,
              TransactionTotolePrice=      @TransactionTotolePrice,
              TransactionUserID=      @TransactionUserID,
              TransactionGUID=    @TransactionGUID,
              

where TransactionD=@TransactionID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@TransactionID", Transaction.ID);
                        command.Parameters.AddWithValue("@TransactionUserID", Transaction.CustomerID);
                        command.Parameters.AddWithValue("@TransactionState", Transaction.State);
                        command.Parameters.AddWithValue("@TransactionTotolePrice", Transaction.TotolePrice);
                        command.Parameters.AddWithValue("@TransactionGUID", Transaction.TransactionGUID);
                        command.Parameters.AddWithValue("@TransactionPaymentMehodID", Transaction.PaymentMethodID);
                       

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

            string qery = @"Delete from Transactions  where TransactionID=@TransactionID";


            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@TransactionID", ID);


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
