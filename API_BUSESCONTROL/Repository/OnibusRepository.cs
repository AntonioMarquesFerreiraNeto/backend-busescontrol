using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
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
            Onibus onibus = _bancoContext.Onibus.FirstOrDefault(x => x.Id == id);
            if (onibus == null) throw new Exception("Desculpe, ônibus não encontrado!");
            return onibus;
        }

        public List<Onibus> OnibusAtivosAll() {
            return _bancoContext.Onibus.Where(x => x.StatusOnibus == OnibusStatus.Ativo).ToList();
        }
        public List<Onibus> OnibusInativosAll() {
            return _bancoContext.Onibus.Where(x => x.StatusOnibus == OnibusStatus.Inativo).ToList();
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
                Onibus onibusInativar = _bancoContext.Onibus.FirstOrDefault(x => x.Id == id);
                if (onibusInativar == null) throw new Exception("Desculpe, ônibus não encontrado!");
                onibusInativar.StatusOnibus = OnibusStatus.Inativo;
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
                Onibus onibusAtivar = _bancoContext.Onibus.FirstOrDefault(x => x.Id == id);
                if (onibusAtivar == null) throw new Exception("Desculpe, ônibus não encontrado!");
                onibusAtivar.StatusOnibus = OnibusStatus.Ativo;
                _bancoContext.Onibus.Update(onibusAtivar);
                _bancoContext.SaveChanges();
                return onibusAtivar;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
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

        public List<Onibus> PaginateListAtivos(int paginaAtual, bool statusPaginate) {
            if (statusPaginate == true) {
                int indiceInicial = (paginaAtual - 1) * 10;
                return _bancoContext.Onibus.Where(x => x.StatusOnibus == OnibusStatus.Ativo).Skip(indiceInicial).Take(10).ToList();
            }
            else {
                if (paginaAtual < 2) {
                    return _bancoContext.Onibus.Where(x => x.StatusOnibus == OnibusStatus.Ativo).Skip(0).Take(10).ToList();
                }
                int indiceInicial = (paginaAtual - 2) * 10;
                return _bancoContext.Onibus.Where(x => x.StatusOnibus == OnibusStatus.Ativo).Skip(indiceInicial).Take(10).ToList();
            }
        }
        public List<Onibus> PaginateListInativos(int paginaAtual, bool statusPaginate) {
            if (statusPaginate == true) {
                int indiceInicial = (paginaAtual - 1) * 10;
                return _bancoContext.Onibus.Where(x => x.StatusOnibus == OnibusStatus.Inativo).Skip(indiceInicial).Take(10).ToList();
            }
            else {
                if (paginaAtual < 2) {
                    return _bancoContext.Onibus.Where(x => x.StatusOnibus == OnibusStatus.Inativo).Skip(0).Take(10).ToList();
                }
                int indiceInicial = (paginaAtual - 2) * 10;
                return _bancoContext.Onibus.Where(x => x.StatusOnibus == OnibusStatus.Inativo).Skip(indiceInicial).Take(10).ToList();
            }
        }

        public int QtPaginasAtivas() {
            var qtOnibus = _bancoContext.Onibus.Count(x => x.StatusOnibus == OnibusStatus.Ativo);
            //Arredonda o resultado para cima, caso  o mesmo seja flutuante.
            int qtPaginas = (int)Math.Ceiling((double)qtOnibus / 10);
            return qtPaginas;
        }

        public int QtPaginasInativas() {
            var qtOnibus = _bancoContext.Onibus.Count(x => x.StatusOnibus == OnibusStatus.Inativo);
            //Arredonda o resultado para cima, caso  o mesmo seja flutuante.
            int qtPaginas = (int)Math.Ceiling((double)qtOnibus / 10);
            return qtPaginas;
        }
    }
}
