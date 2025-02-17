using ConnectionLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public  class clsTransaction
    {
        enum enMode{Add,Update}

        enMode _Mode;

        int _ID;
        public int ID { get { return _ID; } }

        public string PaymentMethodID { get; set; }

        public DTOTransaction.enState State { get; set; }

        public decimal TotolePrice { get; set; }

        public int CustomerID { get; set; }

        public Guid TransactionGUID { get; set; }

        public DTOTransaction dtoTransaction { get { return new DTOTransaction(this._ID, this.PaymentMethodID, this.State, this.TotolePrice, this.CustomerID, this.TransactionGUID.ToString()); } }

         clsTransaction(int ID, string PaymentMethodID, DTOTransaction.enState State, decimal TotolePrice, int CustomerID, string TransactionGUID)
        {
            this._ID = ID;
            this.PaymentMethodID = PaymentMethodID;
            this.State = State;
            this.TotolePrice = TotolePrice;
            this.CustomerID = CustomerID;
            this.TransactionGUID =Guid.TryParse(TransactionGUID,out Guid GUID)?GUID:Guid.Empty ;

            this._Mode = enMode.Update;
        
        }


        public clsTransaction( string PaymentMethodID, DTOTransaction.enState State, decimal TotolePrice, int CustomerID, string TransactionGUID)
        {
            this._ID = -1;
            this.PaymentMethodID = PaymentMethodID;
            this.State = State;
            this.TotolePrice = TotolePrice;
            this.CustomerID = CustomerID;
            this.TransactionGUID = Guid.TryParse(TransactionGUID, out Guid GUID) ? GUID : Guid.Empty;
            this._Mode = enMode.Add;
        }

        async Task<bool> _Add()
        {

             this._ID= await ConnectionLayer.clsTransaction.Add(this.dtoTransaction);
            return _ID != -1;
        }
        async Task<bool> _Update()
        {

          return await ConnectionLayer.clsTransaction.Update(this.dtoTransaction);
        }

        public static async Task<List<DTOTransaction>?> GetAll()
        {
            return await ConnectionLayer.clsTransaction.GetAll();
        }
        public static async Task<bool> Delete(int ID)
        {
            return await ConnectionLayer.clsTransaction.Delete(ID);
        }
        public  async Task<bool> Save()
        {
            bool result = false;
            if (_Mode == enMode.Add)
            {
                result =await _Add();
                if(result)
                {
                    _Mode = enMode.Update;

                }
            }
            else
            {
               result= await _Update();
            }

            return result;
        }
   


    
    
    }
}
