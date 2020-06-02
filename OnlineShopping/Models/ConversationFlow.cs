using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopping.Models
{
    public class ConversationFlow
    {
        // Identifies the last question asked.
        public enum Question
        {
            Name,
            Age,
            Date,
            None, // Our last action did not involve a question.
        }

        // The last question asked.
        public Question LastQuestionAsked { get; set; } = Question.None;
        public bool DidBotWelcome { get; set; } = false;
        public bool PromptedUserForName { get; set; } = false;
    }
}
