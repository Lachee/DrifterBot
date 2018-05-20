using RconSharp;
using RconSharp.Net45;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrifterBot
{
	//25575
	class Rcon
	{
		private bool _hasPassword;
		private string _password;
		private string _address;
		private int _port;

		public string Address => _address;
		public int Port => _port;

		public Rcon(string address, int port)
		{
			this._address = address;
			this._port = port;
			_hasPassword = false;
			_password = null;
		}
		public Rcon(string address, int port, string password)
		{
			this._address = address;
			this._port = port;
			this._password = password;
			this._hasPassword = true;
		}

		public async Task<RconResponse> Execute(string command)
		{
			// create an instance of the socket. In this case i've used the .Net 4.5 object defined in the project
			INetworkSocket socket = new RconSocket();

			// create the RconMessenger instance and inject the socket
			RconMessenger messenger = new RconMessenger(socket);

			try
			{
				// initiate the connection with the remote server
				bool isConnected = await messenger.ConnectAsync(Address, Port);
				if (!isConnected)
				{
					Disconnect(socket, messenger);
					return new RconResponse()
					{
						Success = false,
						Message = "",
						Error = "Failed to connect to the RCON server"
					};
				}

				//Authicate with the server
				if (_hasPassword)
				{
					bool authenticated = await messenger.AuthenticateAsync(_password);
					if (!authenticated)
					{
						Disconnect(socket, messenger);
						return new RconResponse()
						{
							Success = false,
							Message = "",
							Error = "Failed to authicated to RCON server"
						};
					}
				}
			
				// if we fall here, we're good to go! from this point on the connection is authenticated and you can send commands 
				// to the server

				//Attempt to send the command.
				var response = await messenger.ExecuteCommandAsync(command);

				//It should be something. If its empty we have failed
				if (string.IsNullOrEmpty(response))
				{
					return new RconResponse()
					{
						Success = false,
						Message = "",
						Error = "RCON returned no response."
					};
				}

				//Check if the response is valid
				bool success = !string.IsNullOrEmpty(response);
				return new RconResponse()
				{
					Success = success,
					Message = response,
					Error = ""
				};
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return new RconResponse()
				{
					Success = false,
					Message = "",
					Error = "An exception occured while performing RCON: " + e.Message
				};
			}
			finally
			{
				Disconnect(socket, messenger);
			}
		}

		private void Disconnect(INetworkSocket socket, RconMessenger messenger)
		{
			try
			{
				if (messenger != null)
					messenger.CloseConnection();

				if (socket != null)
					socket.CloseConnection();

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}


	/// <summary>
	/// The response structure of a RCON to the Starbound server. This cannot be relied on due to the nature of Starbounds RCON.
	/// </summary>
	public struct RconResponse
	{
		/// <summary>Was the RCON successful? In order for a RCON to be successful, its response must begin with 'OK:'.</summary>
		public bool Success { get; set; }

		/// <summary>The message minecraft responded with.</summary>
		/// <seealso cref="Text"/>
		/// <seealso cref="TaggedText"/>
		public string Message { get; set; }

		/// <summary>
		/// The error message.
		/// </summary>
		public string Error { get; set; }

		/// <summary>
		/// Converts the RCON response into a nicely formatted string
		/// </summary>
		/// <returns>String in the format of: <c>[success: {Success}] {Text}</c></returns>
		public override string ToString()
		{
			return "[success: " + Success + "] " + Message;
		}
	}
}

