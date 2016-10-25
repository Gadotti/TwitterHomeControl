using GHIElectronics.NETMF.FEZ;
using Microsoft.SPOT.Hardware;

namespace TwitterHomeControl.Business
{
    public class Saidas
    {
        public OutputPort Tomada1 { get; set; }
        public OutputPort Tomada2 { get; set; }
        public OutputPort Tomada3 { get; set; }
        public OutputPort Tomada4 { get; set; }
        public OutputPort Tomada5 { get; set; }

        public Saidas()
        {
            //Inicia as saídas e indica o estado inicial
            Tomada1 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di3, false);
            Tomada2 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di7, false);
            Tomada3 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di5, false);
            Tomada4 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di8, false);
            Tomada5 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di2, false); //Relé invertido
        }
    }
}
