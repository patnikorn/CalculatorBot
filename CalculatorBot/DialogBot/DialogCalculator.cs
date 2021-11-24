using CalculatorBot.Bots;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CalculatorBot.DialogBot
{
    public class DialogCalculator : ComponentDialog
    {
        private readonly IStatePropertyAccessor<Calculator> _CalculatorAccessor;

        public DialogCalculator(UserState userState)
            : base(nameof(DialogCalculator))
        {
            _CalculatorAccessor = userState.CreateProperty<Calculator>("EchoBot");

            var waterfallSteps = new WaterfallStep[]
            {
                Number1,
                Number2,
                Result,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new NumberPrompt<double>(nameof(NumberPrompt<double>)));

            InitialDialogId = nameof(WaterfallDialog);

        }

        private async Task<DialogTurnResult> Number1(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your first number : "),
            };

            return await stepContext.PromptAsync(nameof(NumberPrompt<double>), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> Number2(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["number1"] = (double)stepContext.Result;

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your second number : "),
            };

            return await stepContext.PromptAsync(nameof(NumberPrompt<double>), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> Result(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["number2"] = (double)stepContext.Result;

            var Calculator = await _CalculatorAccessor.GetAsync(stepContext.Context, () => new Calculator(), cancellationToken);

            Calculator.Number1 = (double)stepContext.Values["number1"];
            Calculator.Number2 = (double)stepContext.Values["number2"];
            Calculator.Result = Calculator.Number1 + Calculator.Number2;

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Your result is : { Calculator.Result }"),
            };

            return await stepContext.PromptAsync(nameof(NumberPrompt<double>), promptOptions, cancellationToken);

        }

    }
}

