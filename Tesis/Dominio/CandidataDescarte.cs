namespace Tesis.Dominio
{
    // Una vaca que el sistema propone revisar para descarte, con los motivos por los
    // que aparece en la lista.
    //
    // El sistema no decide: junta lo que ya sabe -produccion, servicios sin preniez,
    // dias abiertos, historia sanitaria, cantidad de partos- y se lo pone delante a la
    // encargada, que es quien conoce al animal. Por eso son motivos en texto y no un
    // puntaje: lo que sirve es entender por que aparece.
    public class CandidataDescarte
    {
        private Hembra mHembra;
        private List<string> mMotivos;

        public Hembra Hembra { get { return mHembra; } set { mHembra = value; } }
        public List<string> Motivos { get { return mMotivos; } set { mMotivos = value; } }

        public CandidataDescarte(Hembra pHembra, List<string> pMotivos)
        {
            mHembra = pHembra;
            mMotivos = pMotivos;
        }
    }
}
