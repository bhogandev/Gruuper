using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BVCommon
{
    public class FirebaseContextFunctions
    {
        private static FireSharp.FirebaseClient context;

        public static bool WriteToDb(string entityType, string entityObject, FireSharp.FirebaseClient _context)
        {
            //Determine Entity Type
            Type t = BVContextHelper.GetType(entityType);

            bool result = FirebaseContextHelper.WriteObject(_context, t, entityObject);

            return result;
            
        }

        public static bool DeleteFromDb(string entityType, string entityObject, FireSharp.FirebaseClient _context)
        {
            //Determine Entity Type
            Type t = BVContextHelper.GetType(entityType);

            bool result = FirebaseContextHelper.DeleteObject(context, t, entityObject);

            return result;
        }

        public static bool UpdateToDb(string entityType, string entityObject, FireSharp.FirebaseClient _context)
        {
            //Determine Entity Type
            Type t = BVContextHelper.GetType(entityType);

            bool result = FirebaseContextHelper.UpdateObject(_context, t, entityObject);

            return result;
        }

    }
}
