using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsCatygory
    {
        enum enMode { Add,Update}
        enMode _Mode;
        int _ID;
        public int ID { get { return _ID; } }
        public string Name { get;set; }

        public DTOCatygory DTO { get { return new DTOCatygory(this.ID, this.Name); } }
        public clsCatygory(string Name)
        {

            this.Name = Name;
        }
         clsCatygory(int ID,string Name)
        {
            this._ID = ID;
            this.Name = Name;
        }


        async Task<bool> _Update()
        {

            return await ConnectionLayer.clsCatygorie.Update(DTO);

        }
        async Task<bool>_Add()
        {
            this._ID = await ConnectionLayer.clsCatygorie.Add(this.DTO);
            return _ID != -1;
        }

        public async Task<bool> Save()
        {
            bool result = false;

            if (this._Mode == enMode.Add)
            {
                result =await _Add();

                if (result) _Mode = enMode.Update;
            }
            else
            {
                result=await _Update();
            }

            return result;

        }

        public static async Task<bool> Delete(int ID)
        {
            return await ConnectionLayer.clsCatygorie.Delete(ID);

        }

        public static async Task<List<DTOCatygory>?> GetAll()
        {
            return await ConnectionLayer.clsCatygorie.GetAll();
        }

    }

}
