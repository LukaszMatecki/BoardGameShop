using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.Dtos
{
    public class OrderHistoryDto
    {
        public int IdZamowienia { get; set; }

        public DateTime DataZamowienia { get; set; }

        public decimal CalkowitaKwota { get; set; }

        public string Status { get; set; } = null!;

        public string MetodaPlatnosci { get; set; } = null!;

        public string MetodaDostawy { get; set; } = null!;

        public List<OrderProductDto> Produkty { get; set; } = [];
    }
}