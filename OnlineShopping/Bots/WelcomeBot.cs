using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using OnlineShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopping.Bots
{
    public class WelcomeBot : ActivityHandler
    {
        // Messages sent to the user.
        private const string WelcomeMessage = "I am Your online Shopping Assistant " +
                                                "I will introduce you to the Online shops in Kenya";
        private BotState _userState;

        // Initializes a new instance of the "WelcomeUserBot" class.
        public WelcomeBot(UserState userState)
        {
            _userState = userState;
        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync($"Hi there -. {WelcomeMessage}", cancellationToken: cancellationToken);
                }
            }
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeUserStateAccessor = _userState.CreateProperty<WelcomeState>(nameof(WelcomeState));
            var didBotWelcomeUser = await welcomeUserStateAccessor.GetAsync(turnContext, () => new WelcomeState());

            if (didBotWelcomeUser.DidBotWelcome == false)
            {
                didBotWelcomeUser.DidBotWelcome = true;

                // the channel should sends the user name in the 'From' object
                var userName = turnContext.Activity.From.Name;

                //await turnContext.SendActivityAsync($"You are seeing this message because this was your first message ever to this bot.", cancellationToken: cancellationToken);
                await turnContext.SendActivityAsync($"Would you like to go ahead and view?, if so, type yes.", cancellationToken: cancellationToken);
            }
            else
            {
                // This example hardcodes specific utterances. You should use LUIS or QnA for more advance language understanding.
                var text = turnContext.Activity.Text.ToLowerInvariant();
                switch (text)
                {
                    case "hello":
                    case "hi":
                        await turnContext.SendActivityAsync($"You said {text}.", cancellationToken: cancellationToken);
                        break;
                    case "yes":
                    case "help":
                        await SendIntroCardAsync(turnContext, cancellationToken);
                        break;
                    default:
                        await turnContext.SendActivityAsync($"Thank you for visiting us, Dont forget to stop by next time for great deals!", cancellationToken: cancellationToken);
                        break;
                }
            }

            // Save any state changes.
            await _userState.SaveChangesAsync(turnContext);
        }

        private static async Task SendIntroCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard();
            card.Title = "Welcome to Your Online Shopping Assistant!";
            card.Text = @"Your one stop for online shopping links.";
            card.Images = new List<CardImage>() { new CardImage("https://aka.ms/bf-welcome-card-image") };
            card.Buttons = new List<CardAction>()
            {
                new CardAction(ActionTypes.OpenUrl, "Tuskys Supermarket", null, "Tuskys", "Tuskys", "https://www.tuskys.com/"),
                new CardAction(ActionTypes.OpenUrl, "Naivas SuperMarket", null, "Naivas SuperMarket", "Naivas SuperMarket", "https://www.naivas.co.ke/"),
                new CardAction(ActionTypes.OpenUrl, "Jumia Online Shopping", null, "Jumia Online Shopping", "Jumia Online Shopping", "https://www.jumia.co.ke/"),
            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }
    }
}
