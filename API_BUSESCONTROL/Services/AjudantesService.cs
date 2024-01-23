namespace API_BUSESCONTROL.Services {
    public class AjudantesService {  

        public List<EstadoAndUF> ReturnListEstadoUF() {
            List<EstadoAndUF> list = new List<EstadoAndUF>()!;
            list.Add(new EstadoAndUF { UF = "AC", Estado = "Acre" });
            list.Add(new EstadoAndUF { UF = "AL", Estado = "Alagoas" });
            list.Add(new EstadoAndUF { UF = "AP", Estado = "Amapá" });
            list.Add(new EstadoAndUF { UF = "AM", Estado = "Amazonas" });
            list.Add(new EstadoAndUF { UF = "BA", Estado = "Bahia" });
            list.Add(new EstadoAndUF { UF = "CE", Estado = "Ceará" });
            list.Add(new EstadoAndUF { UF = "DF", Estado = "Distrito Federal" });
            list.Add(new EstadoAndUF { UF = "ES", Estado = "Espírito Santo" });
            list.Add(new EstadoAndUF { UF = "GO", Estado = "Goiás" });
            list.Add(new EstadoAndUF { UF = "MA", Estado = "Maranhão" });
            list.Add(new EstadoAndUF { UF = "MT", Estado = "Mato Grosso" });
            list.Add(new EstadoAndUF { UF = "MS", Estado = "Mato Grosso do Sul" });
            list.Add(new EstadoAndUF { UF = "MG", Estado = "Minas Gerais" });
            list.Add(new EstadoAndUF { UF = "PA", Estado = "Pará" });
            list.Add(new EstadoAndUF { UF = "PB", Estado = "Paraíba" });
            list.Add(new EstadoAndUF { UF = "PR", Estado = "Paraná" });
            list.Add(new EstadoAndUF { UF = "PE", Estado = "Pernambuco" });
            list.Add(new EstadoAndUF { UF = "PI", Estado = "Piauí" });
            list.Add(new EstadoAndUF { UF = "RJ", Estado = "Rio de Janeiro" });
            list.Add(new EstadoAndUF { UF = "RN", Estado = "Rio Grande do Norte" });
            list.Add(new EstadoAndUF { UF = "RS", Estado = "Rio Grande do Sul" });
            list.Add(new EstadoAndUF { UF = "RO", Estado = "Rondônia" });
            list.Add(new EstadoAndUF { UF = "RR", Estado = "Roraima" });
            list.Add(new EstadoAndUF { UF = "SC", Estado = "Santa Catarina" });
            list.Add(new EstadoAndUF { UF = "SP", Estado = "São Paulo" });
            list.Add(new EstadoAndUF { UF = "SE", Estado = "Sergipe" });
            list.Add(new EstadoAndUF { UF = "TO", Estado = "Tocantins" });
            return list;
        }
        public class EstadoAndUF {
            public string Estado { get; set; }
            public string UF { get; set; }
        }

    }
}
