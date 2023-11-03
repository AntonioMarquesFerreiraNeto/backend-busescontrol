using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository.Interfaces
{
    public interface IOnibusRepository
    {
        public Onibus CreateOnibus(Onibus onibus);
        public Onibus DeleteOnibus(int? id);
        public Onibus UpdateOnibus(Onibus onibus);
        public Onibus InativarOnibus(int? id);
        public Onibus AtivarOnibus(int? id);
        public int QtPaginasAtivas(string? pesquisa);
        public int QtPaginasInativas(string? pesquisa);
        public List<Onibus> PaginateListAtivos(int paginaAtual, string? pesquisa);
        public List<Onibus> PaginateListInativos(int paginaAtual, string? pesquisa);
        public List<Onibus> GetAll();
        public List<Onibus> GetAllDisponiveis();
        public Onibus GetOnibusById(int? id);
        public void HabilitarDisponibilidade(int? id);
        public void DesabilitarDisponibilidade(int? id);
    }
}
