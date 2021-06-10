namespace EventDriven.CQRS.Abstractions.Commands {

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MediatR;

    /// <inheritdoc />
    public class CommandBroker : ICommandBroker {

        private readonly IMediator mediatr;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mediatr"></param>
        public CommandBroker(IMediator mediatr) => this.mediatr = mediatr;

        /// <inheritdoc />
        public async Task<TResult> InvokeAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult> where TResult : CommandResult {
            try {
                return await mediatr.Send(command);
            } catch (Exception e) {
                return new CommandResult(CommandOutcome.NotHandled, new Dictionary<string, string[]>{{e.GetType().Name, new [] {e.Message}}}) as // TODO: clean this up...

            TResult;
            }
        }

    }

}