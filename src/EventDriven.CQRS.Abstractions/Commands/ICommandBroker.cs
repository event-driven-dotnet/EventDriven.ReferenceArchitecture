using System.Threading.Tasks;

namespace EventDriven.CQRS.Abstractions.Commands
{
    /// <summary>
    ///     Broker of commands.
    /// </summary>
    public interface ICommandBroker
    {
        /// <summary>
        ///     Function to execute the provided command by sending them to the appropriate handler.
        /// </summary>
        /// <param name="command"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        Task<TResult> InvokeAsync<TCommand, TResult>(TCommand command) where TResult : CommandResult
                                                                       where TCommand : ICommand<TResult>;

        /// <summary>
        ///     Function to execute the provided command by sending them to the appropriate handler.
        /// </summary>
        /// <param name="command"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        TResult Invoke<TCommand, TResult>(TCommand command) where TResult : CommandResult
                                                            where TCommand : ICommand<TResult> =>
            InvokeAsync<TCommand, TResult>(command)
               .Result;
    }
}