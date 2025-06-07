namespace Natia.UI.EventArgument
{
    public class Eventargsforsendportinfo : EventArgs
    {
        public int port { get; set; }

        public Eventargsforsendportinfo(int port)
        {
            this.port = port;
        }
    }
}
