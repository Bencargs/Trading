using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trader
{
    public class TraderModel
    {
        public User Account { get; set; }
        public Order[] Orders { get; set; }
        public BankAccount BankAccount { get; set; }
        public Dictionary<Stock, int> Holdings { get; set; }
        public IEnumerable<CandleStick> Chart { get; set; }
    }

    public class CandleStick
    {
        public GlyphType Type { get; set; }
        public decimal PriceStart { get; set; }
        public decimal PriceEnd { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public enum GlyphType
        {
            Rising,
            Falling
        }
    }
}
