using System;

namespace Krypta.Models
{
    public class CryptoDatePrice
    {
        public DateTime Date { get; }
        public double Price { get; }
        
        public CryptoDatePrice(DateTime date, double price)
        {
            Date = date;
            Price = price;
        }
    }
}