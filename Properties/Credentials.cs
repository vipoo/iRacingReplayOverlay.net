// This file is part of iRacingReplayOverlay.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net
//
// iRacingReplayOverlay is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingReplayOverlay is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with iRacingReplayOverlay.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace iRacingReplayOverlay
{
    public class Credentials
    {
        public string UserName { get; set; }

        [XmlIgnore]
        public string FreePassword { get; set; }

        public string HashedPassword
        {
            get
            {
                var data = Encoding.Unicode.GetBytes(FreePassword);
                var encryptedData = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encryptedData);
            }
            set
            {
                var data = Convert.FromBase64String(value);
                var decryptedData = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
                FreePassword = Encoding.Unicode.GetString(decryptedData);
            }
        }

        [XmlIgnore]
        public bool Blank
        {
            get
            {
                return UserName == null || UserName.Length == 0 || FreePassword == null && FreePassword.Length == 0;
            }
        }
    }
}
