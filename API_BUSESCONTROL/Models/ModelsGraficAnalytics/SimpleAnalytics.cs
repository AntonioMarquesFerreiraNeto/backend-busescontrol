using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Models.ModelsGraficAnalytics;

namespace API_BUSESCONTROL.Models {

    public class SimpleAnalytics {
        
        public List<string> LabelsDate { get; set; } = new List<string>();

        public List<SimpleReceitas>? SimpleReceitasList { get; set; } = new List<SimpleReceitas>();

        public List<SimpleDespesas>? SimpleDespesasList { get; set; } = new List<SimpleDespesas>();
    
    }
}
