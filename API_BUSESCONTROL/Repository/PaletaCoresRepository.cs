using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace API_BUSESCONTROL.Repository
{
    public class PaletaCoresRepository : IPaletaCoresRepository {

        private readonly BancoContext _bancoContext;

        public PaletaCoresRepository(BancoContext bancoContext) {
            _bancoContext = bancoContext;
        }
        public PaletaCores CreatePaletaCores(PaletaCores paletaCores) {
            try {
                if (ValidDuplicate(paletaCores.Cor)) throw new Exception("Cor já se encontra registrada!");
                paletaCores.Cor = paletaCores.Cor.ToLower();
                paletaCores.Cor = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(paletaCores.Cor.ToLower());
                _bancoContext.PaletaCores.Add(paletaCores);
                _bancoContext.SaveChanges();
                return paletaCores;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
        public bool ValidDuplicate(string cor) {
            List<PaletaCores> paletas = ListPaletaCores();
            if (paletas.Any(x => x.Cor?.ToUpper() == cor.ToUpper())) {
                return true;
            }
            return false;
        }

        public PaletaCores DeletePaletaCores(int? id) {
            try {
                PaletaCores paletaCores = _bancoContext.PaletaCores.FirstOrDefault(x => x.id == id);
                if (paletaCores == null) throw new Exception("Desculpe, cor não encontrada!");
                if (_bancoContext.Onibus.Any(x => x.CorBus!.ToLower() == paletaCores.Cor!.ToLower())) {
                    throw new Exception("Desculpe, cor de ônibus vinculada!");
                }
                _bancoContext.PaletaCores.Remove(paletaCores);
                _bancoContext.SaveChanges();
                return paletaCores;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<PaletaCores> ListPaletaCores() {
            return _bancoContext.PaletaCores.OrderByDescending(x => x.id).ToList();
        }
    }
}
