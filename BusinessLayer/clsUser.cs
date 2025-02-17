using ConnectionLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsUser
    {
         enum enMode { Add, Update }
 
        enMode _Mode;
        
        int _UserID;
      
        public int UserID { get { return _UserID; } }

        public int PersonID { get; set; }
 
        public string UserName { get; set; }

        public string PassWord {  get; set; }

        public DTOUser.enRole Role { get; set; }
        
        public byte Atherization { get { return 1; } set { value = 2; } }
 
        public DTOUser DTOUser { get { return new DTOUser(this._UserID,this.PersonID,this.Role,this.Atherization,this.UserName,this.PassWord,this._CreateAt.ToLongDateString()) ; } }

        DateTime _CreateAt { get {  return DateTime.Now ; } }

        public clsUser(int PersonID,string UserName,string PassWord,byte Athorization ,DTOUser.enRole Role)
        {
            _UserID = -1;
            this.PersonID = PersonID;
            this.UserName = UserName;
            this.PassWord = PassWord;
            this.Role = Role;
            this.Atherization = Athorization;
            _Mode = enMode.Add;
        }

        clsUser(int ID, int PersonID, string UserName, string PassWord, byte Athorization , DTOUser.enRole Role)
        {
            _UserID = ID;
            this.PersonID = PersonID;
            this.UserName = UserName;
            this.PassWord = PassWord;
            this.Role = Role;
            this.Atherization = Athorization;
            _Mode = enMode.Update;

        }

        public static async Task<List<DTOUser>?> GetAll()
        {
            return await ConnectionLayer.clsUser.GetUsers();
            
            
        }

        public static async Task<clsUser?>Find(int ID)
        {
            DTOUser? user= await ConnectionLayer.clsUser.Find(ID);

            if (user == null) {return null;}

            return new clsUser(user.UserID, user.PersonID, user.UserName, user.UserPassword, user.UserAtherization,user.UserRole);
        }

        bool CleanCustomerAthorization()
        {

            this.Atherization = 0;
            return this.Atherization == 0;
        }
        bool CleanCustomerRole()
        {

            this.Role = DTOUser.enRole.Customer;
            return this.Role== DTOUser.enRole.Customer;
        }

        async Task<bool> _Add()
        {
            this._UserID =await  ConnectionLayer.clsUser.AddUser(this.DTOUser);
            return _UserID != -1;
        }

        async Task<bool> _Update()
        {

           return await ConnectionLayer.clsUser.UpdateUser(this.DTOUser);
          
        }



        public async Task<bool> SaveCustomer()
        {
            bool result = false;

             this.CleanCustomerAthorization();
            this.CleanCustomerRole();

            if (this._Mode == enMode.Add) { 
            

                result=await _Add();
                if(result)this._Mode = enMode.Update;
            
            }

            else
            {
                result = await _Update();

            }
            return result;
        }

        public async Task<bool> SaveAthorizedUser(int IssuerAdmineID)
        {
            //Valaidate Admine 

            bool result = false;

        

            if (this._Mode == enMode.Add)
            {


                result = await _Add();
                if (result) this._Mode = enMode.Update;

            }

            else
            {
                result = await _Update();

            }
            return result;
        }

        

    }
} 
