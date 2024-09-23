// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: report.proto
// </auto-generated>
#pragma warning disable 0414, 1591, 8981, 0612
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Financial.CashFlowReport.Server.Protos {
  /// <summary>
  /// Servi�o para gera��o de relat�rios consolidados (Consultas)
  /// </summary>
  public static partial class RelatorioService
  {
    static readonly string __ServiceName = "RelatorioService";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Financial.CashFlowReport.Server.Protos.RelatorioRequest> __Marshaller_RelatorioRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Financial.CashFlowReport.Server.Protos.RelatorioRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Financial.CashFlowReport.Server.Protos.RelatorioResponse> __Marshaller_RelatorioResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Financial.CashFlowReport.Server.Protos.RelatorioResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Financial.CashFlowReport.Server.Protos.RelatorioRequest, global::Financial.CashFlowReport.Server.Protos.RelatorioResponse> __Method_ObterRelatorioConsolidado = new grpc::Method<global::Financial.CashFlowReport.Server.Protos.RelatorioRequest, global::Financial.CashFlowReport.Server.Protos.RelatorioResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "ObterRelatorioConsolidado",
        __Marshaller_RelatorioRequest,
        __Marshaller_RelatorioResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Financial.CashFlowReport.Server.Protos.ReportReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of RelatorioService</summary>
    [grpc::BindServiceMethod(typeof(RelatorioService), "BindService")]
    public abstract partial class RelatorioServiceBase
    {
      /// <summary>
      /// Obter o relat�rio di�rio consolidado de lan�amentos
      /// </summary>
      /// <param name="request">The request received from the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>The response to send back to the client (wrapped by a task).</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Financial.CashFlowReport.Server.Protos.RelatorioResponse> ObterRelatorioConsolidado(global::Financial.CashFlowReport.Server.Protos.RelatorioRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for RelatorioService</summary>
    public partial class RelatorioServiceClient : grpc::ClientBase<RelatorioServiceClient>
    {
      /// <summary>Creates a new client for RelatorioService</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public RelatorioServiceClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for RelatorioService that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public RelatorioServiceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected RelatorioServiceClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected RelatorioServiceClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      /// Obter o relat�rio di�rio consolidado de lan�amentos
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Financial.CashFlowReport.Server.Protos.RelatorioResponse ObterRelatorioConsolidado(global::Financial.CashFlowReport.Server.Protos.RelatorioRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ObterRelatorioConsolidado(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Obter o relat�rio di�rio consolidado de lan�amentos
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Financial.CashFlowReport.Server.Protos.RelatorioResponse ObterRelatorioConsolidado(global::Financial.CashFlowReport.Server.Protos.RelatorioRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_ObterRelatorioConsolidado, null, options, request);
      }
      /// <summary>
      /// Obter o relat�rio di�rio consolidado de lan�amentos
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Financial.CashFlowReport.Server.Protos.RelatorioResponse> ObterRelatorioConsolidadoAsync(global::Financial.CashFlowReport.Server.Protos.RelatorioRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ObterRelatorioConsolidadoAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Obter o relat�rio di�rio consolidado de lan�amentos
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Financial.CashFlowReport.Server.Protos.RelatorioResponse> ObterRelatorioConsolidadoAsync(global::Financial.CashFlowReport.Server.Protos.RelatorioRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_ObterRelatorioConsolidado, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override RelatorioServiceClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new RelatorioServiceClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(RelatorioServiceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_ObterRelatorioConsolidado, serviceImpl.ObterRelatorioConsolidado).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, RelatorioServiceBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_ObterRelatorioConsolidado, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Financial.CashFlowReport.Server.Protos.RelatorioRequest, global::Financial.CashFlowReport.Server.Protos.RelatorioResponse>(serviceImpl.ObterRelatorioConsolidado));
    }

  }
}
#endregion
