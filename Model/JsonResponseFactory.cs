using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonResponseFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static object ErrorResponse(string error)
        {
            return new { Success = false, ErrorMessage = error };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static object SuccessResponse()
        {
            return new { Success = true };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceObject"></param>
        /// <returns></returns>
        public static object SuccessResponse(object referenceObject)
        {
            return new { Success = true, Object = referenceObject };
        }

    }
}
