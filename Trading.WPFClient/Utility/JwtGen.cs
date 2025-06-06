﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Trading.WPFClient.Utility
{
    public class JwtGen
    {
        //just random names got from internet
        public static string[] firstNames = {
            "Liam","Noah","William","James","Logan","Benjamin","Mason","Elijah","Oliver","Jacob","Lucas",
            "Michael","Alexander","Ethan","Daniel","Matthew","Aiden", "Henry","Joseph","Jackson","Samuel",
            "Sebastian","David","Carter","Wyatt","Jayden","John","Owen","Dylan","Luke","Gabriel","Anthony",
            "Isaac","Grayson", "Jack","Julian","Levi","Christopher","Joshua","Andrew","Lincoln","Mateo","Ryan",
            "Jaxon", "Nathan","Aaron","Isaiah","Thomas","Charles","Caleb","Josiah","Christian","Hunter","Eli",
            "Jonathan","Connor","Landon","Adrian","Asher", "Cameron","Leo","Theodore","Jeremiah","Hudson",
            "Robert","Easton","Nolan","Nicholas","Ezra","Colton","Angel","Brayden","Jordan","Dominic","Austin","Ian",
            "Adam","Elias","Jaxson","Greyson","Jose","Ezekiel","Carson","Evan","Maverick","Bryson","Jace",
            "Cooper","Xavier","Parker", "Jason","Santiago","Chase","Sawyer","Gavin","Leonardo","Kayden",
            "Ayden","Jameson","Kevin","Bentley","Zachary","Everett","Axel","Tyler"
        };
        public static string GenerateName()
        {
            var random = new Random();
            string firstName = firstNames[random.Next(0, firstNames.Length)];

            return $"{firstName}";
        }
        /// <summary>
        /// Generates a symmetric key,and then generate jwt
        /// secretkey, issure, and audience are hard coded in both server and client sides
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GenerateJwtToken(string name)
        {
            string secretKey = "YourWeakSecretKeyIsLongEnoughToGenerateToken";
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.NameIdentifier,name),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "WhoCreateToken",
                Audience = "WhoIsThisTokenIntendedFor"
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
