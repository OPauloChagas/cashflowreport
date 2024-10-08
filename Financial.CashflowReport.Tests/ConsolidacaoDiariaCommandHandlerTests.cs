using Financial.CashFlowReport.Business.CommandHandlers;
using Financial.CashFlowReport.Business.Commands;
using Financial.CashFlowReport.Business.Interface;
using Financial.CashFlowReport.Server.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace Financial.CashflowReport.Tests
{
    public class ConsolidacaoDiariaCommandHandlerTests
    {
        private readonly Mock<IRelatorioGrpcService> _mockGrpcService;
        private readonly Mock<IRelatorioMongoService> _mockMongoService;
        private readonly Mock<IRabbitMQService> _mockRabbitMQService;
        private readonly Mock<ILogger<ConsolidacaoDiariaCommandHandler>> _mockLogger;
        private readonly ConsolidacaoDiariaCommandHandler _handler;

        public ConsolidacaoDiariaCommandHandlerTests()
        {
            _mockGrpcService = new Mock<IRelatorioGrpcService>();
            _mockMongoService = new Mock<IRelatorioMongoService>();
            _mockRabbitMQService = new Mock<IRabbitMQService>();
            _mockLogger = new Mock<ILogger<ConsolidacaoDiariaCommandHandler>>();

            _handler = new ConsolidacaoDiariaCommandHandler(
                _mockGrpcService.Object,
                _mockMongoService.Object,
                _mockRabbitMQService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_DeveChamarRelatorioGrpcServiceQuandoRelatorioNaoExisteNoMongo()
        {
            // Arrange
            var request = new ConsolidacaoDiariaCommand("2024-10-07");

            // Simula que não existe relatório no MongoDB
            _mockMongoService.Setup(m => m.GetRelatorioDiarioByDateAsync(It.IsAny<string>()))
                             .ReturnsAsync((RelatorioDiario)null);

            var grpcResponse = new RelatorioResponse
            {
                Data = "2024-10-07",
                SaldoTotal = 1000,
                RelatorioLancamentos = {
                    new Lancamento { Id = Guid.NewGuid().ToString(), Tipo = "credito", Valor = 500, Descricao = "Crédito Teste", Data = "2024-10-07" },
                    new Lancamento { Id = Guid.NewGuid().ToString(), Tipo = "debito", Valor = 200, Descricao = "Débito Teste", Data = "2024-10-07" }
                }
            };


            _mockGrpcService.Setup(g => g.ObterRelatorioConsolidadoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(grpcResponse);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal(1000, result.SaldoTotal);
            _mockGrpcService.Verify(g => g.ObterRelatorioConsolidadoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockMongoService.Verify(m => m.InsertOrUpdateLancamentoAsync(It.IsAny<LancamentoDataModel>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveRetornarRelatorioDoMongoSeExistir()
        {
            // Arrange
            var request = new ConsolidacaoDiariaCommand("2024-10-07");

            // Simula um relatório existente no MongoDB
            var relatorioDiario = new RelatorioDiario
            {
                Data = "2024-10-07",
                TotalCreditos = 500,
                TotalDebitos = 200,
                Lancamentos =
            [
                new LancamentoDataModel { Id = Guid.NewGuid(), Tipo = "credito", Valor = 500, Descricao = "Crédito Teste", Data = "2024-10-07" },
                new LancamentoDataModel { Id = Guid.NewGuid(), Tipo = "debito", Valor = 200, Descricao = "Débito Teste", Data = "2024-10-07" }
            ]
            };

            _mockMongoService.Setup(m => m.GetRelatorioDiarioByDateAsync(It.IsAny<string>()))
                             .ReturnsAsync(relatorioDiario);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal(300, result.SaldoTotal);
            _mockGrpcService.Verify(g => g.ObterRelatorioConsolidadoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockMongoService.Verify(m => m.GetRelatorioDiarioByDateAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveRetornarErroQuandoGrpcFalha()
        {
            // Arrange
            var request = new ConsolidacaoDiariaCommand("2024-10-07");

            // Simula que não existe relatório no MongoDB
            _mockMongoService.Setup(m => m.GetRelatorioDiarioByDateAsync(It.IsAny<string>()))
                             .ReturnsAsync((RelatorioDiario)null);

            // Simula uma falha no gRPC
            _mockGrpcService.Setup(g => g.ObterRelatorioConsolidadoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                            .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "Erro de gRPC")));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Sucesso);
            Assert.Equal("Erro ao consultar relatório consolidado.", result.Mensagem);
            _mockGrpcService.Verify(g => g.ObterRelatorioConsolidadoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
