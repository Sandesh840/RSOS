using Application.DTOs.Authentication;
using Application.DTOs.Base;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Implementation.Services
{
    public class LoginDictionary
    {
        DateTime endOfDay = DateTime.Today.AddDays(1).AddTicks(-1);
        private readonly Dictionary<string, AuthenticationResponseDTO> _dictionary;
        public LoginDictionary()
        {
            _dictionary = new Dictionary<string, AuthenticationResponseDTO>();
        }

        public void AddOrUpdate(string key, AuthenticationResponseDTO responseDTO)
        {
            _dictionary.Add(key, responseDTO);
        }

        public AuthenticationResponseDTO Get(string key)
        {
            return _dictionary[key];
        }

        public bool IsExist(string key)
        {
            if( _dictionary.ContainsKey(key) )
            {
                return true;
            }
            return false;
        }
    }

}
