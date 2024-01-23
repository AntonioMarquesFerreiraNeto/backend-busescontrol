using System.ComponentModel.DataAnnotations;
using static API_BUSESCONTROL.Models.Enums.LembreteEnum;

namespace API_BUSESCONTROL.Models {

    public class Lembrete {
        
        public int Id { get; set; }      
        
        [Required(ErrorMessage = "*")]
        public string? Conteudo { get; set; }
        
        public DateTime? Data { get; set; }
        
        public TypeLembrete TypeLembrete { get; set; }
       
        public NivelAcesso NivelAcesso { get; set; }
        
        public Funcionario? Funcionario { get; set; }

        public Funcionario? Remetente { get; set; }
        
        public int? FuncionarioId { get; set; }

        public int? RemetenteId { get; set; }
      
    }

}
