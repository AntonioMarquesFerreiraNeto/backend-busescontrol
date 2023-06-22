using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository {
    public interface IOnibusRepository {
        public Onibus CreateOnibus(Onibus onibus);
        public Onibus DeleteOnibus(int? id);
        public Onibus UpdateOnibus(Onibus onibus);
        public Onibus InativarOnibus(int? id);
        public Onibus AtivarOnibus(int? id);
        public int QtPaginasAtivas();
        public int QtPaginasInativas();
        public List<Onibus> PaginateListAtivos(int paginaAtual, bool statusPaginacao);
        public List<Onibus> PaginateListInativos(int paginaAtual, bool statusPaginate);
        public List<Onibus> GetAll();
        public Onibus GetOnibusById(int? id);
    }
}
