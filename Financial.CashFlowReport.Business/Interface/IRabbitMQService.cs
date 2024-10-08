namespace Financial.CashFlowReport.Business.Interface
{
    public interface IRabbitMQService
    {
        void StartListening(string queueName, Action<string> messageProcessor);
    }
}
