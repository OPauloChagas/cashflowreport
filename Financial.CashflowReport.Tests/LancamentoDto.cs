using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial.CashflowReport.Tests
{
    public class LancamentoDto
    {
        public string Id { get; set; }
        public string Tipo { get; set; }
        public double Valor { get; set; }
        public string Descricao { get; set; }
        public string Data { get; set; }
    }

}
