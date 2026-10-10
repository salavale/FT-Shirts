using System.Collections;
using System.Net.Sockets; //Necessary for IEnumerable
namespace FTShirts.Web.Models
{
    public class Catalogue : IEnumerable<Shirt>
    {
        //We can consider making it private readonly
        //to avoid overwrite the list.
        private List<Shirt> _shirts;

        //Void constructor
        public Catalogue() { 
            _shirts = new List<Shirt>();
        }
        public Catalogue(IEnumerable<Shirt> shirts)
        {
            _shirts = new List<Shirt>(shirts);
        }

        public void AddShirt(Shirt shirt) 
        { 
            _shirts.Add(shirt);
        }
        public Shirt GetShirt (int index) 
        { 
            //if index if OOB we return null
            if( index < 0 || index>= _shirts.Count)
            {
                return null;
            }
            return _shirts[index];
        }
        public Shirt GetShirtByID(int id)
        {
            foreach (var shirt in _shirts)
            {
                if (shirt.Id == id) return shirt;
            }
            //if there is no shirt with Id=id
            return null;
        }
        public IEnumerator<Shirt> GetEnumerator() 
        {
            //like a pointer to the first element of _shirts
            return _shirts.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
