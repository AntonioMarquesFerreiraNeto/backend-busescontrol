﻿using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository.Interfaces
{
    public interface IContratoRepository
    {
        public Contrato CreateContrato(Contrato contrato, List<ClientesContrato> lista);
        public Contrato UpdateContrato(Contrato contrato, List<ClientesContrato> lista);
        public Contrato InativarContrato(int id);
        public Contrato AtivarContrato(int id);
        public Contrato AprovarContrato(int id);
        public Contrato RevogarContrato(int id);
        public Contrato GetContratoById(int id);
        public List<Contrato> GetContratosAtivos(int paginaAtual, bool statusPag);
        public List<Contrato> GetContratosInativos(int paginaAtual, bool statusPag);
        public List<Contrato> GetAllContratosAtivos();
        public List<Contrato> GetAllContratosInativos();
        public int ReturnQtPaginasAtivos();
        public int ReturnQtPaginasInativos();
    }
}
