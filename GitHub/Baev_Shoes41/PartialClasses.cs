using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baev_Shoes41
{
    internal class PartialClasses
    {
    }
    public partial class Baev_Shoes41Entities
    {
        private static Baev_Shoes41Entities _context;
        public static Baev_Shoes41Entities GetContext()
        {
            if (_context == null)
                _context = new Baev_Shoes41Entities();
            return _context;
        }
    }

    public partial class Product
    {
        public int Quantity { get; set; }
    }

    public partial class PickupPoint
    {
        public string FullAddress
        {
            get
            {
                return $"{PickupPointPI}, г. {PuckupPointCity}, ул. {PickupPointStreet}, {PickupPointHouse}";
            }
        }
    }
}

