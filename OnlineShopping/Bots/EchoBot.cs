// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.Number;
using OnlineShopping.Models;

namespace OnlineShopping.Bots
{
    public class EchoBot : ActivityHandler
    {
        private const string WelcomeMessage = "I am Your online Shopping Assistant " +
                                                "I will introduce you to the Online shops in Kenya";
        private readonly BotState _userState;
        private readonly BotState _conversationState;

        public EchoBot(ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;
        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync($"Hi there. {WelcomeMessage}", cancellationToken: cancellationToken);
                }
            }
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            var conversationStateAccessors = _conversationState.CreateProperty<ConversationFlow>(nameof(ConversationFlow));
            var flow = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationFlow(), cancellationToken);

            var userStateAccessors = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            var profile = await userStateAccessors.GetAsync(turnContext, () => new UserProfile(), cancellationToken);

            await FillOutUserProfileAsync(flow, profile, turnContext, cancellationToken);

            // Save changes.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
        private static async Task FillOutUserProfileAsync(ConversationFlow flow, UserProfile profile, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var input = turnContext.Activity.Text?.Trim();
            string message;

                if (ValidateName(input, out var name, out message))
            {
                profile.Name = name;
                await turnContext.SendActivityAsync($"Hi {profile.Name}.", null, null, cancellationToken);
                await turnContext.SendActivityAsync("Would you like to check out what we have online?", null, null, cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
            }
        }  
        private static bool ValidateName(string input, out string name, out string message)
        {
            name = null;
            message = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                message = "Please enter a name that contains at least one character.";
            }
            else
            {
                name = input.Trim();
            }

            return message is null;
        
        }
    }
}
        
