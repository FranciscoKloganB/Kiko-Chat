Cliente
	FormChat tem como atributos:
		- TcpChannel ch;
		- SC sc = new SC();
	FormChat tem como métodos:
		public void AdMsg(string msg) { tbConv.Text += m + "\n"; }
--	
	Quando se faz connect:
		ch = new TcpChannel(Int32.Parse(tbPort.Text));
		ChannelServices.RegisterChannel(ch, false);
		ISS ss = (ISS) Activator.GetObject(typeof(ISS),"tcp://localhost:8000/mcm");
		RemotingServices.Marshal(sc, "clientChat", typeof(SC));
		ss.Reg(tbNick.Text, "tcp://localhost:" + tbPort.Text + "/clientChat");
--
	class Client :  MarshalByReference, IClientObject {
		FormChat fc;
		public void RecvBcast (string msg) { fc.Invoke(fc.AdMsg, msg); }
	}
--

Servidor
        class Server : MarshalByReference, IServerObject {
	  string Reg(string nick, ...)
	  void Send(string nick, string msg, ...)
	}


