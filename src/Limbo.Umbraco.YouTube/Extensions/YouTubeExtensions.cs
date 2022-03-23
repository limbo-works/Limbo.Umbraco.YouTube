using System.Collections.Generic;
using Skybrud.Essentials.Http.Collections;
using Skybrud.Essentials.Strings;

namespace Limbo.Umbraco.YouTube.Extensions {
    
    internal static class YouTubeExtensions {
        
        public static void Deconstruct<T>(this IList<T> list, out T first, out T second) {
            // TODO: Move to Skybrud.Essentials
            first = list.Count > 0 ? list[0] : default;
            second = list.Count > 1 ? list[1] : default;
        }

        public static bool TryGetBoolean(this IHttpQueryString query, string key, out bool result) {
            // TODO: Move to Skybrud.Essentials.Http
            return StringUtils.TryParseBoolean(query[key], out result);
        }

        public static bool TryGetInt32(this IHttpQueryString query, string key, out int result) {
            // TODO: Move to Skybrud.Essentials.Http
            return int.TryParse(query[key], out result);
        }

        public static bool TryGetDouble(this IHttpQueryString query, string key, out double result) {
            // TODO: Move to Skybrud.Essentials.Http
            return double.TryParse(query[key], out result);
        }

    }

}