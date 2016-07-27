using System;
using System.Collections.Generic;

namespace WebApplication2
{
    using System.Threading.Tasks;

    public class QuestionDelegator
    {
        public List<Func<string, Task<string>>> QuestionHandlers;
        private readonly Func<string, Task<string>> defaultQuestionHandler;

        /// <summary>
        /// Constructor of QuestionDelegator class
        /// </summary>
        /// <param name="defaultHandler">Handler for default type of question. Returns an answer</param>
        public QuestionDelegator(Func<string, Task<string>> defaultHandler)
        {
            this.QuestionHandlers = new List<Func<string, Task<string>>>();
            this.defaultQuestionHandler = defaultHandler;
        }

        public async Task<string> HandleQuestion(string question)
        {
            foreach (var handler in this.QuestionHandlers)
            {
                handler(question);
            }
            return await this.defaultQuestionHandler(question);
        }
    }
}