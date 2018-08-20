using System;
using System.Threading.Tasks;
using CursoBot.Domain;
using CursoBot.Forms;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace CursoBot.Dialogs
{
    [LuisModel("0c36959c-aa62-4716-b528-4b331bce1e95", "29b5a3055dad4270ad39d74fd5c7626d")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        // Entidades
        private const string entityProduto = "Produto";

        private const string entityMarca = "Marca";

        private InteresseCompra interesseCompra;

        public RootLuisDialog()
        {
            this.interesseCompra = new InteresseCompra();
        }

        // Quando o bot não entende
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Desculpe, não consegui entender '{result.Query}'. Digite 'help' se você precisar de ajuda.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        // Intenções configuradas
        [LuisIntent("InteresseCompra")]
        public async Task InteresseCompra(IDialogContext context, LuisResult result)
        {
            var message = context;

            if (!GetEntities(context, result))
                return;

            var interesseCompraForm = new InteresseCompraForm();

            var hotelsFormDialog = new FormDialog<InteresseCompraForm>(interesseCompraForm, this.BuildIntencaoCompraForm, FormOptions.PromptInStart, result.Entities);

            context.Call(hotelsFormDialog, this.ResumeFormDialog);
        }

        private bool GetEntities(IDialogContext context, LuisResult result)
        {
            EntityRecommendation produtoEntityRecommendation;
            result.TryFindEntity(entityProduto, out produtoEntityRecommendation);

            EntityRecommendation marcaEntityRecommendation;
            result.TryFindEntity(entityMarca, out marcaEntityRecommendation);


            if (produtoEntityRecommendation == null)
            {
                string message = $"Desculpe, não consegui entender '{result.Query}'... Escreva novamente o nome do produto que tem interesse em comprar, caso precise de ajuda, escreva 'ajuda'";

                context.PostAsync(message);

                context.Wait(this.MessageReceived);

                return false;
            }
            else
            {
                interesseCompra.Produto = produtoEntityRecommendation != null ? produtoEntityRecommendation.Entity : string.Empty;

                interesseCompra.Marca = marcaEntityRecommendation != null ? marcaEntityRecommendation.Entity : string.Empty;

                return true;
            }
        }

        private async Task ResumeFormDialog(IDialogContext context, IAwaitable<InteresseCompraForm> result)
        {
            try
            {
                var cadastro = await result;

                interesseCompra.ValorMaximo = cadastro.ValorMaximo;
                interesseCompra.ValorMinimo = cadastro.ValorMinimo;
                interesseCompra.Detalhes = cadastro.Detalhes;

                var resultMessage = context.MakeMessage();

                resultMessage.Text = $"Cadastrado com sucesso!";

                await context.PostAsync(resultMessage);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "Você cancelou a operação.";
                }
                else
                {
                    reply = $"Oops! Aconteceu algo de errado :( Detalhes técnicos: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }


        private IForm<InteresseCompraForm> BuildIntencaoCompraForm()
        {
            OnCompletionAsyncDelegate<InteresseCompraForm> processHotelsSearch = async (context, state) =>
            {
                var message = $"Registrando a intenção de compra...";

                await context.PostAsync(message);
            };

            return new FormBuilder<InteresseCompraForm>()
                .AddRemainingFields()
                .Field(nameof(InteresseCompraForm.Detalhes), (state) => state.EhDetalhado == IsDetalhado.Sim)              
                .Message("Obrigado =]")
                .OnCompletion(processHotelsSearch)
                .Build();
        }


    }
}
