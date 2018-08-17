using System;
using Microsoft.Bot.Builder.FormFlow;

namespace CursoBot.Forms
{
    [Serializable]
    public class InteresseCompraForm
    {
        [Prompt("Qual é o nome do produto que tem interesse de comprar?")]     
        public string Produto { get; set; }

        [Prompt("Qual é o valor mínimo desejado?")]       
        public double ValorMinimo { get; set; }

        [Prompt("Qual é o valor máximo desejado?")]        
        public double ValorMaximo { get; set; }

        [Prompt("Deseja especificar mais detalhes sobre o item? {||}", ChoiceStyle = ChoiceStyleOptions.Buttons)]
        public IsDetalhado EhDetalhado { get; set; }

        [Prompt("Digite palavras chaves para localização do item")]
        [Template(TemplateUsage.NoPreference, "None")]
        public string Detalhes { get; set; }
      
    }

    [Serializable]
    public enum IsDetalhado
    {
        Sim = 1,
        Não = 2
    }
}