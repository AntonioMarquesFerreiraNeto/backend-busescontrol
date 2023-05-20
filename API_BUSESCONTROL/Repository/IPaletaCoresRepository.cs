

using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository {
    public interface IPaletaCoresRepository {
        public List<PaletaCores> ListPaletaCores();
        public PaletaCores CreatePaletaCores(PaletaCores paletaCores);
        public PaletaCores DeletePaletaCores(int? id);
    }
}
