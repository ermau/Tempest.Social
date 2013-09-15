using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Tempest.Providers.Network;

namespace Tempest.Social.Server
{
	class Program
	{
		private static readonly PublicKeyIdentityProvider IdentityProvider = new PublicKeyIdentityProvider();

		static void Main (string[] args)
		{
			Console.WriteLine ("Getting server key...");

			RSAAsymmetricKey key = GetKey ("server.key");

			Console.WriteLine ("Got it.");

			var provider = new NetworkConnectionProvider (
				new[] { SocialProtocol.Instance },
				new Target (Target.AnyIP, 42912),
				10000,
				key);

			SocialServer server = new SocialServer (new MemoryWatchListProvider(), IdentityProvider);
			server.ConnectionMade += OnConnectionMade;
			server.AddConnectionProvider (provider);
			server.Start();

			Console.WriteLine ("Server ready.");

			while (true)
				Thread.Sleep (1000);
		}

		private static void OnConnectionMade (object sender, ConnectionMadeEventArgs e)
		{
			Console.WriteLine ("Connection made");
		}

		sealed class RSAParametersSerializer
			: ISerializer<RSAParameters>
		{
			public static readonly RSAParametersSerializer Instance = new RSAParametersSerializer();

			public static void Serialize (IValueWriter writer, RSAParameters element)
			{
				Instance.Serialize (null, writer, element);
			}

			public static RSAParameters Deserialize (IValueReader reader)
			{
				return Instance.Deserialize (null, reader);
			}

			public void Serialize (ISerializationContext context, IValueWriter writer, RSAParameters element)
			{
				writer.WriteBytes (element.D);
				writer.WriteBytes (element.DP);
				writer.WriteBytes (element.DQ);
				writer.WriteBytes (element.Exponent);
				writer.WriteBytes (element.InverseQ);
				writer.WriteBytes (element.Modulus);
				writer.WriteBytes (element.P);
				writer.WriteBytes (element.Q);
			}

			public RSAParameters Deserialize (ISerializationContext context, IValueReader reader)
			{
				return new RSAParameters {
					D = reader.ReadBytes(),
					DP = reader.ReadBytes(),
					DQ = reader.ReadBytes(),
					Exponent = reader.ReadBytes(),
					InverseQ = reader.ReadBytes(),
					Modulus = reader.ReadBytes(),
					P = reader.ReadBytes(),
					Q = reader.ReadBytes()
				};
			}
		}

		private static RSAAsymmetricKey GetKey (string keypath)
		{
			if (!File.Exists (keypath)) {

				var rsa = new RSACrypto();
				RSAParameters parameters = rsa.ExportKey (true);
				
				using (var stream = File.OpenWrite (keypath)) {
					var writer = new StreamValueWriter (stream);
					RSAParametersSerializer.Serialize (writer, parameters);
				}
			}

			RSAAsymmetricKey key;
			using (var stream = File.OpenRead (keypath)) {
				var reader = new StreamValueReader (stream);
				RSAParameters parameters = RSAParametersSerializer.Deserialize (reader);
				key = new RSAAsymmetricKey (parameters);
			}

			return key;
		}
	}
}
