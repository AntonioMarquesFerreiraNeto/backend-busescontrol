using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository {
    public interface IFuncionarioRepository {
        public Funcionario CreateFuncionario(Funcionario funcionario);
        public Funcionario GetFuncionarioById(int? id);
        public Funcionario UpdateFuncionario(Funcionario funcionario);
        public Funcionario InativarFuncionario(int? id);
        public Funcionario AtivarFuncionario(int? id);
        public Funcionario InativarUsuario(int? id);
        public Funcionario AtivarUsuario(int? id);
        public List<Funcionario> PaginateListAtivos(int paginaAtual, bool statusPaginacao);
        public List<Funcionario> PaginateListInativos(int paginaAtual, bool statusPaginacao);
        public List<Funcionario> GetAllMotoristas();
        public int QtPaginasAtivas();
        public int QtPaginasInativas();
    }
}
