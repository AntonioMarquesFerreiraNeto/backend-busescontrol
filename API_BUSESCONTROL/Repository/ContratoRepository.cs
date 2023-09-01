using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.EntityFrameworkCore;

namespace API_BUSESCONTROL.Repository {
    public class ContratoRepository : IContratoRepository {

        private readonly BancoContext _bancoContext;

        public ContratoRepository(BancoContext bancoContext) {
            _bancoContext = bancoContext;
        }

        public List<Contrato> ListContratosAprovados() {
            return _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.Aprovado)
                        .Include("Motorista")
                        .Include("Onibus")
                        .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                        .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                        .Include(x => x.Financeiros).ThenInclude(x => x.Parcelas)
                        .Include(x => x.Financeiros).ThenInclude(x => x.PessoaFisica)
                        .Include(x => x.Financeiros).ThenInclude(x => x.PessoaJuridica)
                        .Include(x => x.Rescisoes).ThenInclude(x => x.PessoaFisica)
                        .Include(x => x.Rescisoes).ThenInclude(x => x.PessoaJuridica)
                        .AsNoTracking()
                        .ToList();
        }
        public List<Contrato> ListContratosEmAnalise() {
            return _bancoContext.Contrato.Where(x => x.Aprovacao == StatusAprovacao.EmAnalise && x.StatusContrato == ContratoStatus.Ativo)
                        .Include("Motorista")
                        .Include("Onibus")
                        .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                        .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                        .Include(x => x.Financeiros).ThenInclude(x => x.Parcelas)
                        .Include(x => x.Financeiros).ThenInclude(x => x.PessoaFisica)
                        .Include(x => x.Financeiros).ThenInclude(x => x.PessoaJuridica)
                        .Include(x => x.Rescisoes).ThenInclude(x => x.PessoaFisica)
                        .Include(x => x.Rescisoes).ThenInclude(x => x.PessoaJuridica)
                        .AsNoTracking()
                        .ToList();
        }

        public Contrato CreateContrato(Contrato contrato, List<ClientesContrato> lista) {
            try {
                AddClientesContrato(contrato, lista);
                contrato.SetValoresParcelas(lista.Count);
                _bancoContext.Contrato.Add(contrato);
                _bancoContext.SaveChanges();
                return contrato;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }
        public void AddClientesContrato(Contrato contrato, List<ClientesContrato> lista) {
            var list = lista;
            foreach (var item in list.ToList()) {
                var data = new ClientesContrato {
                    Contrato = contrato,
                    PessoaJuridicaId = item.PessoaJuridicaId,
                    PessoaFisicaId = item.PessoaFisicaId
                };
                _bancoContext.ClientesContrato.Add(data);
            }
        }

        public Contrato UpdateContrato(Contrato contrato, List<ClientesContrato> lista) {
            try {
                Contrato contratoDB = _bancoContext.Contrato.FirstOrDefault(x => x.Id == contrato.Id) ?? throw new Exception("Contrato não encontrado!");
                if (contrato.Aprovacao == StatusAprovacao.Aprovado) throw new Exception("Contrato aprovado não pode ser editado!");
                contratoDB.OnibusId = contrato!.OnibusId;
                contratoDB.MotoristaId = contrato!.MotoristaId;
                contratoDB.QtParcelas = (contrato.Pagament == ModelPagament.Avista) ? 1 : contrato.QtParcelas;
                contratoDB.DataEmissao = contrato.DataEmissao;
                contratoDB.DataVencimento = contrato.DataVencimento;
                contratoDB.Detalhamento = contrato.Detalhamento;
                contratoDB.Pagament = contrato.Pagament;
                contratoDB.ValorMonetario = contrato.ValorMonetario;
                contratoDB.SetValoresParcelas(lista.Count);
                UpdateClientesContrato(contrato, lista);
                _bancoContext.Contrato.Update(contratoDB);
                _bancoContext.SaveChanges();
                return contratoDB;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }
        public void UpdateClientesContrato(Contrato contrato, List<ClientesContrato> lista) {
            var clientesContratoDB = _bancoContext.ClientesContrato.Where(x => x.ContratoId == contrato.Id).ToList();

            //Remove clientes que não estão mais no contrato recebido.
            foreach (var dataRemove in clientesContratoDB) {
                if (!lista.Any(x => x.PessoaFisicaId == dataRemove.PessoaFisicaId)) {
                    _bancoContext.ClientesContrato.Remove(dataRemove);
                }
                if (!lista.Any(x => x.PessoaJuridicaId == dataRemove.PessoaJuridicaId)) {
                    _bancoContext.ClientesContrato.Remove(dataRemove);
                }
            }
            //Adicionar os clientes que não estão no banco de dados. 
            foreach (var item in lista) {
                var encontrado = clientesContratoDB.Any(x => (
                    (x.PessoaFisicaId != null && item.PessoaFisicaId != null && x.PessoaFisicaId == item.PessoaFisicaId) ||
                    (x.PessoaJuridicaId != null && item.PessoaJuridicaId != null && x.PessoaJuridicaId == item.PessoaJuridicaId)
                ) && x.ContratoId == contrato.Id);

                if (!encontrado) {
                    var data = new ClientesContrato {
                        ContratoId = contrato.Id,
                        PessoaFisicaId = item.PessoaFisicaId,
                        PessoaJuridicaId = item.PessoaJuridicaId
                    };
                    _bancoContext.ClientesContrato.Add(data);
                }
            }
        }

        public Contrato GetContratoById(int id) {
            return _bancoContext.Contrato
                .Include(x => x.Motorista)
                .Include(x => x.Onibus)
                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id) ?? throw new Exception("Desculpe, contrato não encontrado!");
        }
        public ClientesContrato GetClientesContratoById(int id) {
            return _bancoContext.ClientesContrato
                .Include(x => x.Contrato)
                .Include(x => x.PessoaFisica)
                .Include(x => x.PessoaJuridica)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id) ?? throw new Exception("Desculpe, registro não encontrado.");
        }
        public List<Contrato> GetAllContratosAtivos() {
            return _bancoContext.Contrato
                .Include(x => x.Motorista)
                .Include(x => x.Onibus)
                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                .AsNoTracking()
                .Where(x => x.StatusContrato == ContratoStatus.Ativo).ToList();
        }
        public List<Contrato> GetAllContratosInativos() {
            return _bancoContext.Contrato
                .Include(x => x.Motorista)
                .Include(x => x.Onibus)
                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                .AsNoTracking()
                .Where(x => x.StatusContrato == ContratoStatus.Inativo).ToList();
        }

        public List<Contrato> GetContratosAtivos(int paginaAtual, FiltroContrato filtro, string pesquisa) {
            if (paginaAtual < 1) throw new Exception("Desculpe, ação inválida!");
            switch (filtro) {
                case FiltroContrato.Todos:
                    return _bancoContext.Contrato
                                .Where(x => x.StatusContrato == ContratoStatus.Ativo && x.Id.ToString().Contains(pesquisa))
                                .OrderByDescending(x => x.Id)
                                .Skip((paginaAtual - 1) * 10).Take(10)
                                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                                .Include(x => x.Motorista)
                                .Include(x => x.Onibus)
                                .AsNoTracking()
                                .ToList();
                case FiltroContrato.Aguardando:
                    return _bancoContext.Contrato
                                .Where(x => x.StatusContrato == ContratoStatus.Ativo && x.Andamento == Andamento.Aguardando && x.Id.ToString().Contains(pesquisa))
                                .OrderByDescending(x => x.Id)
                                .Skip((paginaAtual - 1) * 10).Take(10)
                                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                                .Include(x => x.Motorista)
                                .Include(x => x.Onibus)
                                .AsNoTracking()
                                .ToList();
                case FiltroContrato.EmAndamento:
                    return _bancoContext.Contrato
                                .Where(x => x.StatusContrato == ContratoStatus.Ativo && x.Andamento == Andamento.EmAndamento && x.Id.ToString().Contains(pesquisa))
                                .OrderByDescending(x => x.Id)
                                .Skip((paginaAtual - 1) * 10).Take(10)
                                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                                .Include(x => x.Motorista)
                                .Include(x => x.Onibus)
                                .AsNoTracking()
                                .ToList();
                case FiltroContrato.Encerrado:
                    return _bancoContext.Contrato
                                .Where(x => x.StatusContrato == ContratoStatus.Ativo && x.Andamento == Andamento.Encerrado && x.Id.ToString().Contains(pesquisa))
                                .OrderByDescending(x => x.Id)
                                .Skip((paginaAtual - 1) * 10).Take(10)
                                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                                .Include(x => x.Motorista)
                                .Include(x => x.Onibus)
                                .AsNoTracking()
                                .ToList();
                default:
                    return _bancoContext.Contrato
                                .Where(x => x.StatusContrato == ContratoStatus.Ativo && x.Aprovacao == (StatusAprovacao)filtro && x.Id.ToString().Contains(pesquisa))
                                .OrderByDescending(x => x.Id)
                                .Skip((paginaAtual - 1) * 10).Take(10)
                                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
                                .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
                                .Include(x => x.Motorista)
                                .Include(x => x.Onibus)
                                .AsNoTracking()
                                .ToList();

            }

        }
        public int ReturnQtPaginasAtivos(FiltroContrato filtro, string pesquisa) {
            var qtItens = 0;
            switch (filtro) {
                case FiltroContrato.Todos:
                    qtItens = _bancoContext.Contrato.Count(x => x.StatusContrato == ContratoStatus.Ativo && x.Id.ToString().Contains(pesquisa));
                    break;
                case FiltroContrato.Aguardando:
                    qtItens = _bancoContext.Contrato.Count(x => x.StatusContrato == ContratoStatus.Ativo && x.Andamento == Andamento.Aguardando && x.Id.ToString().Contains(pesquisa));
                    break;
                case FiltroContrato.EmAndamento: 
                           qtItens = _bancoContext.Contrato.Count(x => x.StatusContrato == ContratoStatus.Ativo && x.Andamento == Andamento.EmAndamento && x.Id.ToString().Contains(pesquisa));
                    break;
                case FiltroContrato.Encerrado:
                    qtItens = _bancoContext.Contrato.Count(x => x.StatusContrato == ContratoStatus.Ativo && x.Andamento == Andamento.Encerrado && x.Id.ToString().Contains(pesquisa));
                    break;
                default:
                    qtItens = _bancoContext.Contrato.Count(x => x.StatusContrato == ContratoStatus.Ativo && x.Aprovacao == (StatusAprovacao)filtro && x.Id.ToString().Contains(pesquisa));
                    break;
            }
            int qtPaginas = (int)Math.Ceiling((double)qtItens / 10);
            return (qtPaginas == 0) ? 1 : qtPaginas;
        }

        public List<Contrato> GetContratosInativos(int paginaAtual, string pesquisa) {
            if (paginaAtual < 1) throw new Exception("Desculpe, ação inválida!");
            return _bancoContext.Contrato
              .Where(x => x.StatusContrato == ContratoStatus.Inativo && x.Id.ToString().Contains(pesquisa))
              .OrderByDescending(x => x.Id)
              .Skip((paginaAtual - 1) * 10).Take(10)
              .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaFisica)
              .Include(x => x.ClientesContrato).ThenInclude(x => x.PessoaJuridica)
              .Include(x => x.Motorista)
              .Include(x => x.Onibus)
              .AsNoTracking()
              .ToList();
        }
        public int ReturnQtPaginasInativos(string pesquisa) {
            int qtItens = _bancoContext.Contrato.Count(x => x.StatusContrato == ContratoStatus.Inativo && x.Id.ToString().Contains(pesquisa));
            int qtPaginas = (int)Math.Ceiling((double)qtItens / 10);
            return (qtPaginas == 0) ? 1 : qtPaginas;
        }

        public Contrato AtivarContrato(int id) {
            throw new NotImplementedException();
        }

        public Contrato InativarContrato(int id) {
            Contrato contratoDB = _bancoContext.Contrato.FirstOrDefault(x => x.Id == id);
            if (contratoDB!.Aprovacao == StatusAprovacao.Aprovado) throw new Exception("Não é possível inativar contratos aprovados.");
            contratoDB.StatusContrato = ContratoStatus.Inativo;
            _bancoContext.Contrato.Update(contratoDB);
            _bancoContext.SaveChanges();
            return contratoDB;
        }

        public Contrato AprovarContrato(int id) {
            Contrato contratoDB = GetContratoById(id);
            if (contratoDB.Aprovacao == StatusAprovacao.Aprovado) {
                throw new Exception("Desculpe, contrato não encontrado!");
            }
            if (contratoDB.StatusContrato == ContratoStatus.Inativo) {
                throw new Exception("Não é possível aprovar contratos inativos!");
            }
            if (ValidarClientInativos(contratoDB)) {
                throw new Exception("Não é possível aprovar contrato de clientes inativos!");
            }
            if (contratoDB.Motorista!.Status == FuncionarioStatus.Inativo) {
                throw new Exception("Não é possível aprovar contrato com motorista vinculado inativado!");
            }
            if (contratoDB.Onibus!.StatusOnibus == StatusFrota.Inativo) {
                throw new Exception("Não é possível aprovar contrato com ônibus vinculado inativado!");
            }
            contratoDB.Aprovacao = StatusAprovacao.Aprovado;
            contratoDB.Andamento = Andamento.EmAndamento;
            contratoDB.DataEmissao = DateTime.Now.Date;
            _bancoContext.Contrato.Update(contratoDB);
            CreateFinanceiroContrato(contratoDB);
            _bancoContext.SaveChanges();
            return contratoDB;
        }
        public bool ValidarClientInativos(Contrato value) {
            List<PessoaFisica> pessoaFisicas = _bancoContext.PessoaFisica.Where(x => x.Status == ClienteStatus.Inativo)
                .AsNoTracking().Include(x => x.ClientesContrato).ThenInclude(x => x.Contrato).ToList();

            List<PessoaJuridica> pessoaJuridicas = _bancoContext.PessoaJuridica.Where(x => x.Status == ClienteStatus.Inativo)
                .AsNoTracking().Include(x => x.ClientesContrato).ThenInclude(x => x.Contrato).ToList();
            foreach (var item in pessoaFisicas) {
                if (item.ClientesContrato.Any(x => x.ContratoId == value.Id)) {
                    return true;
                }
            }
            foreach (var item in pessoaJuridicas) {
                if (item.ClientesContrato.Any(x => x.ContratoId == value.Id)) {
                    return true;
                }
            }
            return false;
        }

        public void CreateFinanceiroContrato(Contrato contrato) {
            //Incluindo financeiro no clientesContrato.
            foreach (var clientesContrato in contrato.ClientesContrato!) {
                Financeiro financeiroDB = new Financeiro();
                financeiroDB.Contrato = contrato;
                financeiroDB.PessoaFisica = clientesContrato.PessoaFisica;
                financeiroDB.PessoaJuridica = clientesContrato.PessoaJuridica;
                financeiroDB.DataVencimento = contrato.DataVencimento;
                financeiroDB.DespesaReceita = DespesaReceita.Receita;
                financeiroDB.ValorTotDR = contrato.ValorParcelaContratoPorCliente * contrato.QtParcelas;
                financeiroDB.ValorParcelaDR = contrato.ValorParcelaContratoPorCliente;
                financeiroDB.DataEmissao = contrato.DataEmissao;
                financeiroDB.QtParcelas = contrato.QtParcelas;
                financeiroDB.Detalhamento = contrato.Detalhamento;
                financeiroDB.Pagament = contrato.Pagament;
                financeiroDB.FinanceiroStatus = FinanceiroStatus.Ativo;
                _bancoContext.Financeiro.Add(financeiroDB);
                for (int parcelas = 1; parcelas <= contrato.QtParcelas; parcelas++) {
                    Parcela parcela = new Parcela {
                        Financeiro = financeiroDB, StatusPagamento = SituacaoPagamento.AguardandoPagamento,
                        DataVencimentoParcela = contrato.DataEmissao!.Value.AddMonths(parcelas - 1), NomeParcela = parcelas.ToString()
                    };
                    if (parcelas == 1) {
                        parcela.DataVencimentoParcela = contrato.DataEmissao.Value.AddDays(3);
                    }
                    _bancoContext.Parcela.Add(parcela);
                }
            }
        }

        public Contrato RevogarContrato(int id) {
            Contrato contratoDB = _bancoContext.Contrato.FirstOrDefault(x => x.Id == id);
            if (contratoDB == null) throw new Exception("Desculpe, contrato não encontrado!");
            if (contratoDB.Aprovacao == StatusAprovacao.Aprovado) {
                throw new Exception("Contratos aprovados não podem ser negados!");
            }
            contratoDB.Aprovacao = StatusAprovacao.Negado;
            _bancoContext.Contrato.Update(contratoDB);
            _bancoContext.SaveChanges();
            return contratoDB;
        }
    }
}
