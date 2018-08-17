using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace CursoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {         
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    await Conversation.SendAsync(activity, () => new Dialogs.RootLuisDialog());
                    break;
                case ActivityTypes.ConversationUpdate:
                    IConversationUpdateActivity update = activity;
                    using (var scope = Microsoft.Bot.Builder.Dialogs.Internals.DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                    {
                        var client = scope.Resolve<IConnectorClient>();
                        if (update.MembersAdded.Any())
                        {
                            foreach (var newMember in update.MembersAdded)
                            {
                                if (newMember.Id != activity.Recipient.Id)
                                {
                                    var reply = activity.CreateReply();
                                    reply.Text = $"Olá {newMember.Name}! \n Qual produto você está interessado em comprar?";
                                    await client.Conversations.ReplyToActivityAsync(reply);
                                }
                            }
                        }
                    }

                    break;
                default:
                    break;
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
       
    }
}