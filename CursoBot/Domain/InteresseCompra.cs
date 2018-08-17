using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CursoBot.Domain
{
    [Serializable]
    public class InteresseCompra
    {
        public string Produto { get; set; }
        public string Marca { get; set; }
        public double ValorMinimo { get; set; }
        public double ValorMaximo { get; set; }
        public string Detalhes { get; set; }
    }
}