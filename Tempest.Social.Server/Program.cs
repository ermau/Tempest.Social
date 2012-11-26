﻿using System;
using System.IO;
using System.Net;
using System.Text;
using Tempest.Providers.Network;

namespace Tempest.Social.Server
{
	class Program
	{
		private static readonly PublicKeyIdentityProvider IdentityProvider = new PublicKeyIdentityProvider();

		static void Main (string[] args)
		{
			Console.WriteLine ("Getting server key...");

			IAsymmetricKey key = GetKey ("server.key");

			Console.WriteLine ("Got it.");

			var provider = new NetworkConnectionProvider (
				new[] { SocialProtocol.Instance },
				new IPEndPoint (IPAddress.Any, 53121),
				10000,
				() => new RSACrypto(),
				key);

			SocialServer server = new SocialServer (new MemoryWatchListProvider(), IdentityProvider);
			server.AddConnectionProvider (provider);
			server.Start();

			Console.WriteLine ("Server ready.");

			while (Console.ReadLine() != "exit") ;
		}

		private static IAsymmetricKey GetKey (string path)
		{
			IAsymmetricKey key = null;
			if (!File.Exists (path))
			{
				RSACrypto crypto = new RSACrypto();
				key = crypto.ExportKey (true);
				using (FileStream stream = File.Create (path))
					key.Serialize (null, new StreamValueWriter (stream));
			}

			if (key == null)
			{
				using (FileStream stream = File.OpenRead ("client.key"))
					key = new RSAAsymmetricKey (null, new StreamValueReader (stream));
			}

			return key;
		}
	}
}
