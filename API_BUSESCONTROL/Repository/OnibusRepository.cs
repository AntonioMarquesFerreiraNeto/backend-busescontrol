using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API_BUSESCONTROL.Repository {

    public class OnibusRepository : IOnibusRepository {

        private readonly BancoContext _bancoContext;

        public OnibusRepository(BancoContext bancoContext) {
            _bancoContext = bancoContext;
        }

        public Onibus CreateOnibus(Onibus onibus) {
            try {
                if (ValidationDuplicate(onibus)) throw new Exception("Ônibus já se encontra registrado!");
                onibus = SetTrimFrota(onibus);
                _bancoContext.Add(onibus);
                _bancoContext.SaveChanges();
                return onibus;
            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }

        public Onibus GetOnibusById(int? id) {
            Onibus onibus = _bancoContext.Onibus.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Desculpe, ônibus não encontrado!");
            return onibus;
        }

        public List<Onibus> OnibusAtivosAll() {
            return _bancoContext.Onibus.Where(x => x.StatusOnibus == StatusFrota.Ativo).ToList();
        }

        public List<Onibus> GetAllDisponiveis() {
            return _bancoContext.Onibus.Where(x => x.StatusOnibus == StatusFrota.Ativo && x.Disponibilidade == Disponibilidade.Disponivel).ToList();
        }

        public List<Onibus> OnibusInativosAll() {
            return _bancoContext.Onibus.Where(x => x.StatusOnibus == StatusFrota.Inativo).ToList();
        }

        public Onibus UpdateOnibus(Onibus onibus) {
            try {
                Onibus onibusDB = GetOnibusById(onibus.Id);
                if (onibusDB == null) throw new Exception("Desculpe, ônibus não encontrado!");
                if (ValidationDuplicateEdit(onibus, onibusDB)) throw new Exception("Ônibus já se encontra registrado!");
                onibusDB.Marca = onibus.Marca!.Trim();
                onibusDB.NameBus = onibus.NameBus!.Trim();
                onibusDB.Placa = onibus.Placa!.ToUpper().Trim();
                onibusDB.DataFabricacao = onibus.DataFabricacao;
                onibusDB.Renavam = onibus.Renavam!.Trim();
                onibusDB.Assentos = onibus.Assentos!.Trim();
                onibusDB.CorBus = onibus.CorBus!.Trim();
                onibusDB.Chassi = onibus.Chassi!.ToUpper().Trim();

                _bancoContext.Onibus.Update(onibusDB);
                _bancoContext.SaveChanges();
                return onibusDB;

            }
            catch (Exception error) {
                throw new Exception(error.Message);
            }
        }

        public Onibus DeleteOnibus(int? id) {
            Onibus onibus = _bancoContext.Onibus.FirstOrDefault(x => x.Id == id);
            if (onibus == null) throw new Exception("Desculpe, ônibus não encontrado!");
            _bancoContext.Onibus.Remove(onibus);
            _bancoContext.SaveChanges();
            return onibus;
        }

        public Onibus InativarOnibus(int? id) {
            try {
                Onibus onibusInativar = _bancoContext.Onibus.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Desculpe, ônibus não encontrado!");
                if (_bancoContext.Contrato.Any(x => x.OnibusId == id && x.Andamento == Andamento.EmAndamento)) {
                    throw new Exception("Ônibus vinculado em contratos em andamento!");
                }
                onibusInativar.StatusOnibus = StatusFrota.Inativo;
                onibusInativar.Disponibilidade = Disponibilidade.Indisponivel;
                _bancoContext.Onibus.Update(onibusInativar);
                _bancoContext.SaveChanges();
                return onibusInativar;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public Onibus AtivarOnibus(int? id) {
            try {
                Onibus onibusAtivar = _bancoContext.Onibus.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Desculpe, ônibus não encontrado!");
                onibusAtivar.StatusOnibus = StatusFrota.Ativo;
                _bancoContext.Onibus.Update(onibusAtivar);
                _bancoContext.SaveChanges();
                return onibusAtivar;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<Onibus> PaginateListAtivos(int paginaAtual, string? pesquisa) {
            string pesquisaPlaca = pesquisa.Replace("-", "");
            if (paginaAtual < 1) throw new Exception("Desculpe, ação inválida!");
            return _bancoContext.Onibus
                .Include(x => x.Contratos).AsNoTracking()
                .Where(x => x.StatusOnibus == StatusFrota.Ativo && (x.Marca!.Contains(pesquisa) || x.NameBus!.Contains(pesquisa) || x.CorBus!.Contains(pesquisa) || x.Placa!.Contains(pesquisaPlaca) || x.DataFabricacao!.Contains(pesquisa)))
                .Skip((paginaAtual - 1) * 15)
                .Take(15)
                .ToList();
        }

        public List<Onibus> PaginateListInativos(int paginaAtual, string? pesquisa) {
            string pesquisaPlaca = pesquisa.Replace("-", "");
            if (paginaAtual < 1) throw new Exception("Desculpe, ação inválida!");
            return _bancoContext.Onibus
                .Include(x => x.Contratos).AsNoTracking()
              .Where(x => x.StatusOnibus == StatusFrota.Inativo && (x.Marca!.Contains(pesquisa) || x.NameBus!.Contains(pesquisa) || x.CorBus!.Contains(pesquisa) || x.Placa!.Contains(pesquisaPlaca) || x.DataFabricacao!.Contains(pesquisa)))
                .Skip((paginaAtual - 1) * 15)
                .Take(15)
                .ToList();
        }

        public int QtPaginasAtivas(string? pesquisa) {
            string pesquisaPlaca = pesquisa.Replace("-", "");
            var qtOnibus = _bancoContext.Onibus.Count(x => x.StatusOnibus == StatusFrota.Ativo && (x.Marca!.Contains(pesquisa) || x.NameBus!.Contains(pesquisa) || x.CorBus!.Contains(pesquisa) || x.Placa!.Contains(pesquisaPlaca) || x.DataFabricacao!.Contains(pesquisa)));
            int qtPaginas = (int)Math.Ceiling((double)qtOnibus / 15);
            return (qtPaginas == 0) ? 1 : qtPaginas;
        }

        public int QtPaginasInativas(string? pesquisa) {
            string pesquisaPlaca = pesquisa.Replace("-", "");
            var qtOnibus = _bancoContext.Onibus.Count(x => x.StatusOnibus == StatusFrota.Inativo && (x.Marca!.Contains(pesquisa) || x.NameBus!.Contains(pesquisa) || x.CorBus!.Contains(pesquisa) || x.Placa!.Contains(pesquisaPlaca) || x.DataFabricacao!.Contains(pesquisa)));
            int qtPaginas = (int)Math.Ceiling((double)qtOnibus / 15);
            return (qtPaginas == 0) ? 1 : qtPaginas;
        }

        public List<Onibus> GetAll() {
            return _bancoContext.Onibus.Where(x => x.StatusOnibus == StatusFrota.Ativo).ToList();
        }

        public void HabilitarDisponibilidade(int? id) {
            Onibus onibus = GetOnibusById(id);
            onibus.Disponibilidade = Disponibilidade.Disponivel;
            _bancoContext.Onibus.Update(onibus);
            _bancoContext.SaveChanges();
        }

        public void DesabilitarDisponibilidade(int? id) {
            Onibus onibus = GetOnibusById(id);
            onibus.Disponibilidade = Disponibilidade.Indisponivel;
            _bancoContext.Onibus.Update(onibus);
            _bancoContext.SaveChanges();
        }

        public bool ValidationDuplicate(Onibus onibus) {
            List<Onibus> listFrota = _bancoContext.Onibus.ToList();
            if (listFrota.Any(x => x.Placa == onibus.Placa || x.Chassi == onibus.Chassi || x.Renavam == onibus.Renavam)) {
                return true;
            }
            return false;
        }

        public bool ValidationDuplicateEdit(Onibus onibus, Onibus onibusDB) {
            List<Onibus> listFrota = _bancoContext.Onibus.ToList();
            if (listFrota.Any(x => (onibus.Placa != onibusDB.Placa && onibus.Placa == x.Placa) || (onibus.Renavam != onibusDB.Renavam && onibus.Renavam == x.Renavam) || (onibus.Chassi != onibusDB.Chassi && onibus.Chassi == x.Chassi))) {
                return true;
            }
            return false;
        }

        public Onibus SetTrimFrota(Onibus onibus) {
            onibus.Marca = onibus.Marca!.Trim();
            onibus.NameBus = onibus.NameBus!.Trim();
            //Aplica o ToUpper na placa e o método trim.
            onibus.Placa = onibus.Placa!.ToUpper().Trim();
            onibus.DataFabricacao = onibus.DataFabricacao;
            onibus.Renavam = onibus.Renavam!.Trim();
            onibus.Assentos = onibus.Assentos!.Trim();
            onibus.CorBus = onibus.CorBus!.Trim();
            //Aplica o ToUpper no chassi e o método trim.
            onibus.Chassi = onibus.Chassi!.ToUpper().Trim();
            return onibus;
        }
    }
}
