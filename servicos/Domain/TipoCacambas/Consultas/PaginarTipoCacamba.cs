﻿using Domain.Compartilhado;
using Domain.TipoCacambas.Comandos;
using Domain.TipoCacambas.Interface;
using Domain.TipoCacambas.Visualizacoes;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Runtime.InteropServices;

namespace Domain.TipoCacambas.Consultas
{
    public record PaginarTipoCacamba (
        int PageIndex = 0, int PageSize = 10, string Sort = "asc"
    ): IRequest<Resposta>;
    public sealed class PaginarTipoCacambaHandler : IRequestHandler<PaginarTipoCacamba, Resposta>
    {

        private readonly ILogger<PaginarTipoCacambaHandler> _logger;
        private readonly ITipoCacambaRepositorio _repositorio;

        public PaginarTipoCacambaHandler(ILogger<PaginarTipoCacambaHandler> logger,
                                         ITipoCacambaRepositorio repositorio)
        {
            _logger = logger;
            _repositorio = repositorio;
        }

        public async Task<Resposta> Handle(PaginarTipoCacamba request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("********* Retornando os tipos de caçamba - paginação ************");

            var query = _repositorio.ListarTodos()
                        .Select(VisualizarTipoCacambaExtensao.ToView());

            if (request.Sort == "desc")
            {
                query.OrderByDescending(p => p.Volume);
            }else
                query.OrderBy(p => p.Volume);


            var pagination = new Paginacao<VisualizarTipoCacamba>(query, request.PageIndex, request.PageSize);

            //Task<Resposta> t = Task.Run(() => { return new Resposta("", true, pagination); }) ;

            Resposta resposta = await Task.FromResult(new Resposta("", true, pagination));

            return resposta; 
        }

    }
}
