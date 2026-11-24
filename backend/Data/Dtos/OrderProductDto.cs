using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.Dtos
{
    public class OrderProductDto
    {
        public int IdProduktu { get; set; }

        public string NazwaProduktu { get; set; } = null!;

        public int Ilosc { get; set; }

        public decimal CenaZakupu { get; set; }
    }
}