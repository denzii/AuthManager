using AuthServer.Models.Entities;
using AuthServer.Models.Handlers.Interfaces;
using AuthServer.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;

namespace AuthServer.Models.Handlers
{
	public class CipherHandler : ICipherHandler
	{
		// public string GetHash(RegistrationRequest request) 
		// {
		// 	string saltedPassword = $"{request.Email}{request.FirstName}{request.LastName}{request.Password}";

		// 	return HashPassword(saltedPassword);
		// }

		// public bool ValidateUserPassword(User user, LoginRequest request)
		// {
		// 	string saltedPassword = $"{request.Email}{user.FirstName}{user.LastName}{request.Password}";

		// 	bool isPasswordValid = user.Hash == HashPassword(saltedPassword);

		// 	return isPasswordValid;
		// }

		// private string HashPassword(string saltedPassword)
		// {
		// 	SHA256Managed SHA256 = new SHA256Managed();

		// 	string hash = String.Empty;
		// 	byte[] hashBytes = SHA256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword)); //ASCII is inconsistent, use UTF8

		// 	foreach (byte b in hashBytes)
		// 	{
		// 		hash += b.ToString("x2"); //write each byte as hexadecimal
		// 	}

		// 	return hash;
		// }

	}
}
