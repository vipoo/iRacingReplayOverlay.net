// This file is part of iRacingReplayDirector.
//
// Copyright 2016 Dean Netherton
// https://github.com/vipoo/iRacingReplayDirector.net

// To enable AWS logging, update this file with an aws key
// with permission to write to your specific log group/stream
// Recommended that you do not 'commit' your keys.  
// Also apply your favourite obfuscation methods
// to avoid commit changes:
// git update-index --assume-unchanged Aws/AwsKeys.cs

using System;

namespace iRacingReplayDirector
{
    public class AwsKeys
    {
        public static bool HaveKeys
        {
            get { return false; }
        }

        public static string AccessKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static String SecretKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static string GroupName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static string StreamName
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
