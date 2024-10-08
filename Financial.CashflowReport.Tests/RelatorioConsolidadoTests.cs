namespace Financial.CashflowReport.Tests
{
    public class RelatorioConsolidadoTests
    {

        private readonly HttpClient _client;

        public RelatorioConsolidadoTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7114/api/Consolidacao/consolidacao") 
            };
        }

        [Fact]
        public async Task Simular_50RequisicoesPorSegundo_NoRelatorioConsolidado()
        {
            int numeroDeRequisicoes = 50;
            var tasks = new Task<HttpResponseMessage>[numeroDeRequisicoes];

            for (int i = 0; i < numeroDeRequisicoes; i++)
            {
                // Cria uma requisição para cada task
                tasks[i] = _client.GetAsync("?data=2024-10-07"); 
            }

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                var response = await task;
                Assert.True(response.IsSuccessStatusCode, "A requisição falhou");
            }
        }
    }
}