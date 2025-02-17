using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsProduct
    {
        enum enMode { Add,Update}
        int _ID;
        public int ID { get { return _ID; }  }
        public int CategoryID { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public Guid Imagepath { get; set; }

        enMode _Mode;

        public DTOProduct DTOProduct { get { return new DTOProduct(this.ID, this.CategoryID, this.Name, this.Price, this.Imagepath.ToString()); } }

        public clsProduct(int CategoryID,string Name,decimal Price,string ImagePath)
        {
            this._ID = -1;
            this.CategoryID= CategoryID;
            this.Name= Name;
            this.Price= Price;
            this.Imagepath= Guid.TryParse(ImagePath,out Guid GUIDPath)? GUIDPath:Guid.Empty ;
            _Mode = enMode.Add;


        }

         clsProduct(int ID, int CategoryID, string Name, decimal Price, string ImagePath)
        {
            this._ID= ID;
            this.CategoryID = CategoryID;
            this.Name = Name;
            this.Price = Price;
            this.Imagepath = Guid.TryParse(ImagePath, out Guid GUIDPath) ? GUIDPath : Guid.Empty;

            _Mode = enMode.Update;


        }

         public static async Task<clsProduct?>Find(int ID)
        {
            DTOProduct? p= await ConnectionLayer.clsProduct.Find(ID);
            if(p==null)return null;
            return new clsProduct(p.ID,p.CategoryID,p.Name,p.Price,p.Imagepath);
        }

        async Task<bool> _Add()
        {

            this._ID = await ConnectionLayer.clsProduct.Add(this.DTOProduct);

            return _ID != -1;
        }

        async Task<bool> _Update()
        {


            return await ConnectionLayer.clsProduct.Update(this.DTOProduct);

        }


        public async Task<bool> Save()
        {
            bool result = false;

            if (_Mode == enMode.Add)
            {

                result = await _Add();
                if (result) { _Mode = enMode.Update; }



            }
            else
            {
                result = await _Update();
            }

            return result;
        }


        public static async Task<bool>Delete(int ID)
        {

            return await ConnectionLayer.clsProduct.Delete(ID);

        }

        public static async Task<List<DTOProduct>?>GetAll()
        {

            return await ConnectionLayer.clsProduct.GetAll();

        }

    }
}
